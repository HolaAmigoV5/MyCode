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
            //services.AddDirectoryBrowser();  //ע��Ŀ¼���
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

            //��ʾ�ļ�Ŀ¼
            //app.UseDirectoryBrowser();

            //ʹ���Զ���Ŀ¼���ʾ�̬�ļ�
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = "/File",
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "File"))
            //});

            //ʹ��Ĭ��ҳ��
            //app.UseDefaultFiles();

            //ʹ�þ�̬�ļ�
            app.UseStaticFiles();

            //�Է���"/api"��ͷ�����������д��ӳ�䵽��̬�ļ���
            app.MapWhen(context =>
            {
                //����������"/api"��ͷ������
                return !context.Request.Path.Value.StartsWith("/api");
            }, appBuilder =>
            {
                //д��һ���Ƽ�ʹ��
                //var option = new RewriteOptions();
                //option.AddRewrite(".*", "/index.html", true);
                //appBuilder.UseRewriter(option);
                //appBuilder.UseStaticFiles();

                //д����:
                appBuilder.Run(async c =>
                {
                    //��̬�ļ���ȡ����
                    var file = env.WebRootFileProvider.GetFileInfo("index.html");

                    //���
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
