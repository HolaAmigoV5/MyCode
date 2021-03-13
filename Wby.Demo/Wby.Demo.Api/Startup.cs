using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using Wby.Demo.Api.ApiManager;
using Wby.Demo.Api.Extensions;
using Wby.Demo.EFCore;
using Wby.Demo.EFCore.Context;
using Wby.Demo.Shared.DataInterfaces;
using Wby.Demo.Shared.DataModel;

namespace Wby.Demo.Api
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
            //ÅäÖÃ¿çÓò
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyOrigin();
                });
            });
            services.AddControllers();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            var migrationAssembly = typeof(WbyContext).GetTypeInfo().Assembly.GetName();
            var migrationAssemblyName = migrationAssembly.Name;

            services.AddDbContext<WbyContext>(options => {
                //Ç¨ÒÆÖÁMySql
                //var connectionString = Configuration.GetConnectionString("MySqlNoteConnection");
                //options.UseMySQL(connectionString, sql => sql.MigrationsAssembly(migrationAssemblyName));

                //Ç¨ÒÆÖÁMSSql
                var connectionString = Configuration.GetConnectionString("MSSqlNoteConnection");
                options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssemblyName));
            }).AddUnitOfWork<WbyContext>()
            .AddCustomRepository<User, CustomUserRepository>()
            .AddCustomRepository<UserLog, CustomUserLogRepository>()
            .AddCustomRepository<Menu, CustomMenuRepository>()
            .AddCustomRepository<Group, CustomGroupRepository>()
            .AddCustomRepository<AuthItem, CustomAuthItemRepository>()
            .AddCustomRepository<Basic, CustomBasicRepository>();

            services.AddTransient<IDataInitializer, DataInitializer>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<IMenuManager, MenuManager>();
            services.AddTransient<IGroupManager, GroupManager>();
            services.AddTransient<IBasicManager, BasicManager>();
            services.AddTransient<IAuthItemManager, AuthManager>();

            var autoMapper = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new AutoMappingFile());
            }).CreateMapper();

            services.AddSingleton(autoMapper);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = ".NET Core WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //³õÊ¼»¯Êý¾Ý¿â
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var databaseInitializer = serviceScope.ServiceProvider.GetService<IDataInitializer>();
                await databaseInitializer.InitSampleDataAsync();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();
            app.UseHttpsRedirection();
            app.UseCors("any");

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(Options =>
            {
                Options.ShowExtensions();
                Options.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET Core WebApi v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSwagger();
            });
        }
    }
}
