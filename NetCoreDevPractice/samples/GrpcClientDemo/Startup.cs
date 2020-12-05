using System;
using System.Net.Http;
using GrpcServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using static GrpcServices.OrderGrpc;

namespace GrpcClientDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //允许使用不加密的http/2协议
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


            services.AddGrpcClient<OrderGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
                //options.Address = new Uri("http://localhost:5002");
            })
            #region Polly使用
                //注册Polly设置重试次数。出现HttpRequestException,500,408触发
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(20))
                //出现错误等待2S进行重试，重试20次
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(20, i => TimeSpan.FromSeconds(i * 2)))
                //一直重试，直到成功
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(i * 2)));

            //定义自己的策略
            var reg = services.AddPolicyRegistry();

            //当响应码是201(Created)时，使用retryforever
            reg.Add("retryforever", Policy.HandleResult<HttpResponseMessage>(mesage =>
            {
                return mesage.StatusCode == System.Net.HttpStatusCode.Created;
            }).RetryForeverAsync());

            //使用自定义策略
            services.AddHttpClient("orderclient").AddPolicyHandlerFromRegistry("retryforever");
            services.AddHttpClient("orderclientv2").AddPolicyHandlerFromRegistry((registry, message) =>
            {
                return message.Method == HttpMethod.Get ? registry.Get<IAsyncPolicy<HttpResponseMessage>>("retryforever")
                    : Policy.NoOpAsync<HttpResponseMessage>();
            });

            Policy.Handle<Exception>().Fallback(mesage =>
            {
                Console.WriteLine("出错了，回调一下");
            }); 
            #endregion

            //绕过证书使用
            //.ConfigurePrimaryHttpMessageHandler(provider =>
            // {
            //     var handler = new SocketsHttpHandler();
            //     handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true; //允许无效、或自签名证书
            //     return handler;
            // });


            //services.AddGrpcClient<Greeter.GreeterClient>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    OrderGrpcClient service = context.RequestServices.GetService<OrderGrpcClient>();

                    try
                    {
                        var r = service.CreateOrder(new CreateOrderCommand { BuyerId = "abc" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("异常了");
                    }

                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
