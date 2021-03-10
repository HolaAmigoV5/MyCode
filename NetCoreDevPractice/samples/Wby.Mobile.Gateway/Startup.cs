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
            //ע��Ocelot
            services.AddOcelot(Configuration);

            #region �����֤
            //����ģ�������ز����������֤
            //��ȡsecrityKeyע�뵽�����У�Ȼ�����������֤
            var secrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
            services.AddSingleton(secrityKey);

            //ѡ��Cookie��ΪĬ�������֤����
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //����Cookie��֤����
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            })
            //����JWT��֤����
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //�Ƿ���֤ǩ����
                    ValidateAudience = true, //�Ƿ���֤�ͻ���
                    ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
                    ClockSkew = TimeSpan.FromSeconds(30),  //ʧЧʱ���ƫ��ʱ�䣬��ʾʧЧ��30s�ڻ�����ʹ��
                    ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey

                    ValidAudience = "localhost", //�ͻ���
                    ValidIssuer = "localhost", //ǩ����
                    IssuerSigningKey = secrityKey //�õ�SecurityKey
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

            //�����֤�м����ע��˳�򣬱�����UseEndpoints֮ǰ
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //ʹ��Ocelot��Ҫ�������ϣ���������õ�API��Ȼ��Ч��ֻ��������Ч��ӳ�����ǵ���������
            app.UseOcelot().Wait();
        }
    }
}
