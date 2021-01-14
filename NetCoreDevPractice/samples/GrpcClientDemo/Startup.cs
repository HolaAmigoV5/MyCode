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
            //����ʹ�ò����ܵ�http/2Э��
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //ע��OrderGrpcClient
            //services.AddGrpcClient<OrderGrpcClient>(options =>
            //{
            //    options.Address = new Uri("https://localhost:5001");
            //    //options.Address = new Uri("http://localhost:5002");  //ʹ�ò����ܵ�http2Э��
            //});

            ////����HttpClient���ƹ�֤��ʹ��
            // services.AddGrpcClient<OrderGrpcClient>(options =>
            //{
            //    options.Address = new Uri("https://localhost:5001");
            //})
            //.ConfigurePrimaryHttpMessageHandler(provider =>
            // {
            //     var handler = new SocketsHttpHandler();
            //     handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true; //������Ч������ǩ��֤��
            //     return handler;
            // });


            //services.AddGrpcClient<Greeter.GreeterClient>();


            #region Pollyʹ��
            services.AddGrpcClient<OrderGrpcClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
            })
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


            //�����۶ϲ���
            services.AddHttpClient("orderclientv3").AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    //��������10�������쳣ʱ�����۶�10s
                    handledEventsAllowedBeforeBreaking: 10, //����10�ο�ʼ�۶�
                    durationOfBreak: TimeSpan.FromSeconds(10),  //�۶�ʱ��
                    onBreak: (r, t) => { },  //�����۶�ʱ������һ���¼�
                    onReset: () => { },     //�۶ϻָ�ʱ������һ���¼�
                    onHalfOpen: () => { }   //�۶ϻָ�֮ǰ��������֤�����Ƿ���õ��¼�
                ));


            services.AddHttpClient("orderclientv3").AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                //�߼��۶ϲ���
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.8,  //ʧ�ܱ���
                    samplingDuration: TimeSpan.FromSeconds(30),    //����ʱ��
                    minimumThroughput: 100,   //��С�Ĵ洢��


                    durationOfBreak: TimeSpan.FromSeconds(20),   //�۶�ʱ��
                    onBreak: (r, t) => { },  //�����۶�ʱ������һ���¼�
                    onReset: () => { },     //�۶ϻָ�ʱ������һ���¼�
                    onHalfOpen: () => { }   //�۶ϻָ�֮ǰ��������֤�����Ƿ���õ��¼�
                ));

            //==========�۶���ϲ��Ե�ʹ��=============
            var breakPol = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.8,  //ʧ�ܱ���
                    samplingDuration: TimeSpan.FromSeconds(30),    //����ʱ��
                    minimumThroughput: 100,   //��С�Ĵ洢��
                    durationOfBreak: TimeSpan.FromSeconds(20),   //�۶�ʱ��
                    onBreak: (r, t) => { },  //�����۶�ʱ������һ���¼�
                    onReset: () => { },     //�۶ϻָ�ʱ������һ���¼�
                    onHalfOpen: () => { }   //�۶ϻָ�֮ǰ��������֤�����Ƿ���õ��¼�
                );

            var messager = new HttpResponseMessage()
            {
                Content = new StringContent("�۶��쳣")
            };
            var fallback = Policy<HttpResponseMessage>.Handle<BrokenCircuitException>().FallbackAsync(messager);
            var retry = Policy<HttpResponseMessage>.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1));
            var fallbackBreak = Policy.WrapAsync(fallback, retry, breakPol);
            services.AddHttpClient("httpV3").AddPolicyHandler(fallbackBreak);
            //==========��ϲ���ʹ�ý���==============

            //============������ϲ���ʹ��============
            var bulk = Policy.BulkheadAsync<HttpResponseMessage>(
                    maxParallelization: 30,  //���󲢷���
                    maxQueuingActions: 20,   //���Ķ����������󳬹���󲢷����ǣ����������������
                    onBulkheadRejectedAsync: contxt => Task.CompletedTask   //�������������߼�
                );
            var messager2 = new HttpResponseMessage()
            {
                Content = new StringContent("�����쳣")
            };
            var fallbackbulk = Policy<HttpResponseMessage>.Handle<BulkheadRejectedException>().FallbackAsync(messager2);
            var fallbackAndBulk = Policy.WrapAsync(fallbackbulk, bulk);
            services.AddHttpClient("httpv4").AddPolicyHandler(fallbackAndBulk);
            //=========������ϲ���ʹ�ý���===========

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
                //�������GrpcClient��CreateOrder����
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
