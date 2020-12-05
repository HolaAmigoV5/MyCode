using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjectionDemo
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
            #region 注册服务不同生命周期的服务
            services.AddSingleton<IMySingletonService, MySingletonService>();
            services.AddScoped<IMyScopeService, MyScopeService>();
            services.AddTransient<IMyTransientService, MyTransientService>();
            #endregion

            #region 花式注册
            services.AddSingleton<IOrderService>(new OrderService()); //直接注入实例
            //services.AddSingleton<IOrderService, OrderServiceEx>();
            //services.AddSingleton<IOrderService>(serviceProvider =>
            //{
            //    //工厂模式，可以通过遍历serviceProvider对象，构造复杂对象，返回。
            //    return new OrderServiceEx();
            //});
            #endregion

            #region 尝试注册
            services.TryAddSingleton<IOrderService, OrderService>(); //已经注册过就不能再注册了
            //注册相同接口，不同实现的对象
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());

            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService>(new OrderServiceEx()));

            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService>(p =>
            //{
            //    return new OrderServiceEx();
            //})); 
            #endregion

            #region 移除和替换注册
            //services.Replace(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());
            //services.RemoveAll<IOrderService>();

            #endregion

            #region 注册泛型模板
            services.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));
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
