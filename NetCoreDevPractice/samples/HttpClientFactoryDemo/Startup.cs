using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HttpClientFactoryDemo
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
            services.AddControllers();

            //工厂模式
            services.AddHttpClient();                   //HttpClientFactory注册
            services.AddScoped<OrderServiceClient>();   //注册OrderServiceClient


            //命名客户端
            services.AddSingleton<RequestIdDelegatingHandler>();  //注册RequestIdDelegatingHandler
            services.AddHttpClient("NamedOrderServiceClient", client =>
            {
                client.DefaultRequestHeaders.Add("client-name", "namedclient");
                client.BaseAddress = new Uri("https://localhost:5003");
            }).SetHandlerLifetime(TimeSpan.FromMinutes(20))

            //管道中添加自己的请求处理逻辑，继承自DelegatingHandler
            .AddHttpMessageHandler(provider => provider.GetService<RequestIdDelegatingHandler>());  
            services.AddScoped<NamedOrderServiceClient>();  //注册NamedOrderServiceClient


              
            //类型化客户端。推荐使用这种模式
            services.AddHttpClient<TypedOrderServiceClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5003");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
