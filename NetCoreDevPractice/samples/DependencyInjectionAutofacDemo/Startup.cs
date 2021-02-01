using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using DependencyInjectionAutofacDemo.Services;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;

namespace DependencyInjectionAutofacDemo
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
            services.AddControllers().AddControllersAsServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<MyService>().As<IMyService>();

            //命名注册
            //builder.RegisterType<MyServiceV2>().Named<IMyService>("service2");

            //属性注入
            //builder.RegisterType<MyNameService>();  //MyNameService注入
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired();

            //AOP注入：不改变原有类时，方法执行中嵌入新的逻辑
            //builder.RegisterType<Interceptor>(); //拦截器注入到容器中
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired()
            //    .InterceptedBy(typeof(Interceptor)).EnableInterfaceInterceptors();

            //子容器
            builder.RegisterType<MyNameService>().InstancePerMatchingLifetimeScope("myscope");
        }

        public ILifetimeScope AutofacContainer { get; private set; }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //获取容器，从容器中拿到服务对象，调用对象
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            //var service1 = this.AutofacContainer.Resolve<IMyService>();
            //service1.ShowCode();

            //var service = this.AutofacContainer.ResolveNamed<IMyService>("service2");
            //service.ShowCode();

            //子容器
            using (var myscope = AutofacContainer.BeginLifetimeScope("myscope"))
            {
                var service0 = myscope.Resolve<MyNameService>();
                using (var scope = myscope.BeginLifetimeScope())
                {
                    var service1 = scope.Resolve<MyNameService>();
                    var service2 = scope.Resolve<MyNameService>();
                    System.Console.WriteLine($"service1=service2:{service1 == service2}");
                    System.Console.WriteLine($"service1=service0:{service1 == service0}");
                }
            }

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
