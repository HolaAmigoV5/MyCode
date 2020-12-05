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
            //����ʹ�ò����ܵ�http/2Э��
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


            services.AddGrpcClient<OrderGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
                //options.Address = new Uri("http://localhost:5002");
            })
            #region Pollyʹ��
                //ע��Polly�������Դ���������HttpRequestException,500,408����
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(20))
                //���ִ���ȴ�2S�������ԣ�����20��
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(20, i => TimeSpan.FromSeconds(i * 2)))
                //һֱ���ԣ�ֱ���ɹ�
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryForeverAsync(i => TimeSpan.FromSeconds(i * 2)));

            //�����Լ��Ĳ���
            var reg = services.AddPolicyRegistry();

            //����Ӧ����201(Created)ʱ��ʹ��retryforever
            reg.Add("retryforever", Policy.HandleResult<HttpResponseMessage>(mesage =>
            {
                return mesage.StatusCode == System.Net.HttpStatusCode.Created;
            }).RetryForeverAsync());

            //ʹ���Զ������
            services.AddHttpClient("orderclient").AddPolicyHandlerFromRegistry("retryforever");
            services.AddHttpClient("orderclientv2").AddPolicyHandlerFromRegistry((registry, message) =>
            {
                return message.Method == HttpMethod.Get ? registry.Get<IAsyncPolicy<HttpResponseMessage>>("retryforever")
                    : Policy.NoOpAsync<HttpResponseMessage>();
            });

            Policy.Handle<Exception>().Fallback(mesage =>
            {
                Console.WriteLine("�����ˣ��ص�һ��");
            }); 
            #endregion

            //�ƹ�֤��ʹ��
            //.ConfigurePrimaryHttpMessageHandler(provider =>
            // {
            //     var handler = new SocketsHttpHandler();
            //     handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true; //������Ч������ǩ��֤��
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
                        Console.WriteLine("�쳣��");
                    }

                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
