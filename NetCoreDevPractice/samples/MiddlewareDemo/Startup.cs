using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiddlewareDemo
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //注册中间件
            app.Use(async (context, next) =>
            {
                //await context.Response.WriteAsync("Hello");
                await next();
                await context.Response.WriteAsync("Hello222");
            });

            //对特殊路径(如/abc)指定注册中间件
            app.Map("/abc", abcBuilder =>
            {
                abcBuilder.Use(async (context, next) =>
                {
                    //一旦已经开始输出，则不能再修改响应头的内容
                    //await context.Response.WriteAsync("Hello");
                    await next();
                    await context.Response.WriteAsync("Hello222");
                });
            });

            //复杂条件判断，注册中间件
            app.MapWhen(context =>
            {
                //请求参数查询包含"abc"时才执行后续操作
                return context.Request.Query.Keys.Contains("abc");
            }, builder =>
            {
                //这里Run()和User()类似，但有区别
                builder.Run(async context =>
                {
                    await context.Response.WriteAsync("new abc");
                });
            });


            //注册自定义中间件
            app.UseMyMiddleware();

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
