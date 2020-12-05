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
            #region 验证
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.LoginPath = "/home/login";

                        //Cookie的身份验证设置为false。
                        //options.Cookie.HttpOnly = false;

                        //这里Cookie身份验证设置为true，防止跨站脚本攻击
                        options.Cookie.HttpOnly = true;
                    });
            #endregion

            services.AddControllersWithViews();

            #region 进行防跨站脚本攻击
            //携带有HeaderName = "X-CSRF-TOKEN"才可以正常访问
            services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

            //开启全局AntiforgeryToken验证。也可以在方法上标记[ValidateAntiForgeryToken]特性进行局部验证
            //services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
            #endregion

            #region 跨域请求CORS
            services.AddCors(options =>
               {
                   //注册一个叫api的跨域策略
                   options.AddPolicy("api", builder =>
                   {
                       //设置跨域的源，允许携带任何Header，允许身份认证信息，允许的Headers列表
                       builder.WithOrigins("https://localhost:5001").AllowAnyHeader().AllowCredentials().WithExposedHeaders("abc");

                       //实际开发中，需要用SetIsOriginAllowed设置指定的源。
                       builder.SetIsOriginAllowed(orgin => true).AllowCredentials().AllowAnyHeader();
                   });
               });
            #endregion

            #region 缓存
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

            //启用跨域中间件
            app.UseCors();

            //身份验证中间件。注意顺序，必须在UseEndpoints之前
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
