using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Wby.Mobile.Gateway
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
            //注册Ocelot
            services.AddOcelot(Configuration);

            #region 身份认证
            //这里模拟在网关层配置身份验证
            //读取secrityKey注入到容器中，然后启用身份认证
            var secrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
            services.AddSingleton(secrityKey);

            //选择Cookie作为默认身份验证方案
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //设置Cookie验证方案
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            //设置JWT验证方案
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //是否验证签发者
                    ValidateAudience = true, //是否验证客户端
                    ValidateLifetime = true, //是否验证失效时间
                    ClockSkew = TimeSpan.FromSeconds(30),  //失效时间的偏离时间，表示失效的30s内还可以使用
                    ValidateIssuerSigningKey = true, //是否验证SecurityKey

                    ValidAudience = "localhost", //客户端
                    ValidIssuer = "localhost", //签发者
                    IssuerSigningKey = secrityKey //拿到SecurityKey
                };
            }); 
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

            //身份验证中间件。注意顺序，必须在UseEndpoints之前
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //使用Ocelot。要放在最后，希望网关内置的API仍然生效，只有内置生效才映射我们的网关配置
            app.UseOcelot().Wait();
        }
    }
}
