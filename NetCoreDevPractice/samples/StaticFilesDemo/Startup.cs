using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StaticFilesDemo
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
            //services.AddDirectoryBrowser();  //注册目录浏览
        }

        const int BufferSize = 64 * 1024;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            //显示文件目录
            //app.UseDirectoryBrowser();

            //使用自定义目录访问静态文件
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = "/File",
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "File"))
            //});

            //使用默认页面
            //app.UseDefaultFiles();

            //使用静态文件
            app.UseStaticFiles();

            //对非以"/api"开头的请求进行重写，映射到静态文件上
            app.MapWhen(context =>
            {
                //检索不是以"/api"开头的请求
                return !context.Request.Path.Value.StartsWith("/api");
            }, appBuilder =>
            {
                //写法一：推荐使用
                //var option = new RewriteOptions();
                //option.AddRewrite(".*", "/index.html", true);
                //appBuilder.UseRewriter(option);
                //appBuilder.UseStaticFiles();

                //写法二:
                appBuilder.Run(async c =>
                {
                    //静态文件读取出来
                    var file = env.WebRootFileProvider.GetFileInfo("index.html");

                    //输出
                    c.Response.ContentType = "text/html";
                    using (var fileStream = new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read))
                    {
                        await StreamCopyOperation.CopyToAsync(fileStream, c.Response.Body, null, BufferSize, c.RequestAborted);
                    }
                });

            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
