using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Wby.Mobile.ApiAggregator
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

            services.AddHttpClient<IOrderService, OrderService>();

            services.AddGrpcClient<Ordering.API.Grpc.OrderService.OrderServiceClient>(options =>
                {
                    options.Address = new Uri(Configuration.GetValue<string>("ServiceUrls:OrderingAPI"));
                }).ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    var handler = new SocketsHttpHandler();
                    handler.SslOptions.RemoteCertificateValidationCallback = (a, b, c, d) => true; //������Ч������ǩ��֤��
                    return handler;
                });

            //services.AddHttpClient<IOrderService, OrderService>().ConfigureHttpClient(client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:5001");
            //});

            #region �����֤
            //����ģ��΢����API���������֤��
            var secrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]));
            services.AddSingleton(secrityKey);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {

                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//�Ƿ���֤Issuer
                        ValidateAudience = true,//�Ƿ���֤Audience
                        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                        ValidAudience = "localhost",//Audience
                        ValidIssuer = "localhost",//Issuer
                        IssuerSigningKey = secrityKey//�õ�SecurityKey
                    };
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
