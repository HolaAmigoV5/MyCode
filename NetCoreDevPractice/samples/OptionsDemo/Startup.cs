using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OptionsDemo.Services;

namespace OptionsDemo
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
            //���ú�ѡ���
            //AddScoped����IOptionsSnapshot��IOptionsMonitor ʵ�ֶ�ȡ��������
            //services.Configure<OrderServiceOptions>(Configuration.GetSection("OrderService"));
            //services.AddSingleton<OrderServiceOptions>();
            //services.AddSingleton<IOrderService, OrderService>();

            //services.AddScoped<IOrderService, OrderService>();

            //�����з���ע�����services����չ����AddOrderService�С���������
            services.AddOrderService(Configuration.GetSection("OrderService"));
            services.AddControllers();
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
