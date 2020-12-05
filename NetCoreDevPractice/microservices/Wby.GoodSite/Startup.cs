using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wby.GoodSite
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
            #region ��֤
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/home/login";

                        //Cookie�������֤����Ϊfalse��
                        //options.Cookie.HttpOnly = false;

                        //����Cookie�����֤����Ϊtrue����ֹ��վ�ű�����
                        options.Cookie.HttpOnly = true;
                    });
            #endregion

            services.AddControllersWithViews();

            #region ���з���վ�ű�����
            //Я����HeaderName = "X-CSRF-TOKEN"�ſ�����������
            services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

            //����ȫ��AntiforgeryToken��֤��Ҳ�����ڷ����ϱ��[ValidateAntiForgeryToken]���Խ��оֲ���֤
            //services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            #endregion

            #region ��������CORS
            services.AddCors(options =>
               {
                   //ע��һ����api�Ŀ������
                   options.AddPolicy("api", builder =>
                   {
                       //���ÿ����Դ������Я���κ�Header�����������֤��Ϣ�������Headers�б�
                       builder.WithOrigins("https://localhost:5001").AllowAnyHeader().AllowCredentials().WithExposedHeaders("abc");

                       //ʵ�ʿ����У���Ҫ��SetIsOriginAllowed����ָ����Դ��
                       builder.SetIsOriginAllowed(orgin => true).AllowCredentials().AllowAnyHeader();
                   });
               });
            #endregion

            #region ����
            services.AddMemoryCache();
            services.AddStackExchangeRedisCache(options =>
            {
                Configuration.GetSection("RedisCache").Bind(options);
            });
            services.AddResponseCaching();
            services.AddEasyCaching(options =>
            {
                options.UseRedis(Configuration, name: "easycaching");
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseResponseCaching();

            //���ÿ����м��
            app.UseCors();

            //�����֤�м����ע��˳�򣬱�����UseEndpoints֮ǰ
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
