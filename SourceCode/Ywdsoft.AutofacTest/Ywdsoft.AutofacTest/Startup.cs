using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ywdsoft.Core;

namespace Ywdsoft.AutofacTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            services.AddMvc();

            //var builder = new ContainerBuilder();
            //builder.Populate(services);
            //var assembly = typeof(Startup).Assembly;
            //builder.RegisterType<LogInterceptor>();
            //builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
            //    .EnableInterfaceInterceptors();
            //var IControllerType = typeof(ControllerBase);
            //builder.RegisterAssemblyTypes(assembly).Where(t => 
            //    IControllerType.IsAssignableFrom(t) && t != IControllerType).PropertiesAutowired()
            //    .EnableClassInterceptors();
            //var Container = builder.Build();
            //return new AutofacServiceProvider(Container);

            return IocManager.Instance.Initialize(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var iocManager = app.ApplicationServices.GetService<IIocManager>();
            List<Parameter> cparams = new List<Parameter>();
            cparams.Add(new NamedParameter("name", "张三"));
            cparams.Add(new NamedParameter("sex", "男"));
            cparams.Add(new TypedParameter(typeof(int), 2));
            var testDemo = iocManager.Resolve<TestDemo>(cparams.ToArray());
            Console.WriteLine($"姓名：{testDemo.Name},年龄：{testDemo.Age},性别：{testDemo.Sex}");
            app.UseMvc();
        }
    }
}
