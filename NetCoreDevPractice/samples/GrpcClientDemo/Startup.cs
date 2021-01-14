using System;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
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
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //注册OrderGrpcClient
            //services.AddGrpcClient<OrderGrpcClient>(options =>
            //{
            //    options.Address = new Uri("https://localhost:5001");
            //    //options.Address = new Uri("http://localhost:5002");  //使用不加密的http2协议
            //});

            ////配置HttpClient，绕过证书使用
            // services.AddGrpcClient<OrderGrpcClient>(options =>
            //{
            //    options.Address = new Uri("https://localhost:5001");
            //})
            //.ConfigurePrimaryHttpMessageHandler(provider =>
            // {
            //     var handler = new SocketsHttpHandler();
            //     handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true; //允许无效、或自签名证书
            //     return handler;
            // });


            //services.AddGrpcClient<Greeter.GreeterClient>();


            #region Polly使用
            services.AddGrpcClient<OrderGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
            })
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


            //定义熔断策略
            services.AddHttpClient("orderclientv3").AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    //当服务发生10次请求异常时，就熔断10s
                    handledEventsAllowedBeforeBreaking: 10, //报错10次开始熔断
                    durationOfBreak: TimeSpan.FromSeconds(10),  //熔断时间
                    onBreak: (r, t) => { },  //发生熔断时触发的一个事件
                    onReset: () => { },     //熔断恢复时触发的一个事件
                    onHalfOpen: () => { }   //熔断恢复之前，进行验证服务是否可用的事件
                ));


            services.AddHttpClient("orderclientv3").AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                //高级熔断策略
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.8,  //失败比例
                    samplingDuration: TimeSpan.FromSeconds(30),    //采样时间
                    minimumThroughput: 100,   //最小的存储量


                    durationOfBreak: TimeSpan.FromSeconds(20),   //熔断时长
                    onBreak: (r, t) => { },  //发生熔断时触发的一个事件
                    onReset: () => { },     //熔断恢复时触发的一个事件
                    onHalfOpen: () => { }   //熔断恢复之前，进行验证服务是否可用的事件
                ));

            //==========熔断组合策略的使用=============
            var breakPol = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.8,  //失败比例
                    samplingDuration: TimeSpan.FromSeconds(30),    //采样时间
                    minimumThroughput: 100,   //最小的存储量
                    durationOfBreak: TimeSpan.FromSeconds(20),   //熔断时长
                    onBreak: (r, t) => { },  //发生熔断时触发的一个事件
                    onReset: () => { },     //熔断恢复时触发的一个事件
                    onHalfOpen: () => { }   //熔断恢复之前，进行验证服务是否可用的事件
                );

            var messager = new HttpResponseMessage()
            {
                Content = new StringContent("熔断异常")
            };
            var fallback = Policy<HttpResponseMessage>.Handle<BrokenCircuitException>().FallbackAsync(messager);
            var retry = Policy<HttpResponseMessage>.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1));
            var fallbackBreak = Policy.WrapAsync(fallback, retry, breakPol);
            services.AddHttpClient("httpV3").AddPolicyHandler(fallbackBreak);
            //==========组合策略使用结束==============

            //============限流组合策略使用============
            var bulk = Policy.BulkheadAsync<HttpResponseMessage>(
                    maxParallelization: 30,  //请求并发数
                    maxQueuingActions: 20,   //最大的队列数：请求超过最大并发数是，后面请求队列数。
                    onBulkheadRejectedAsync: contxt => Task.CompletedTask   //请求限流后处理逻辑
                );
            var messager2 = new HttpResponseMessage()
            {
                Content = new StringContent("限流异常")
            };
            var fallbackbulk = Policy<HttpResponseMessage>.Handle<BulkheadRejectedException>().FallbackAsync(messager2);
            var fallbackAndBulk = Policy.WrapAsync(fallbackbulk, bulk);
            services.AddHttpClient("httpv4").AddPolicyHandler(fallbackAndBulk);
            //=========限流组合策略使用结束===========

            #endregion

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
                //这里调用GrpcClient的CreateOrder方法
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
