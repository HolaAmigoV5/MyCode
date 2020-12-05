using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace JwtBearerSample
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; set; }

        // Shared between users in memory
        public IList<Todo> Todos { get; } = new List<Todo>();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    // You also need to update /wwwroot/app/scripts/app.js
                    o.Authority = Configuration["oidc:authority"];
                    o.Audience = Configuration["oidc:clientid"];
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();

            // [Authorize] would usually handle this
            app.Use(async (context, next) =>
            {
                // Use this if there are multiple authentication schemes
                var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (authResult.Succeeded && authResult.Principal.Identity.IsAuthenticated)
                {
                    await next();
                }
                else if (authResult.Failure != null)
                {
                    // Rethrow, let the exception page handle it.
                    ExceptionDispatchInfo.Capture(authResult.Failure).Throw();
                }
                else
                {
                    await context.ChallengeAsync();
                }
            });

            // MVC would usually handle this:
            app.Map("/api/TodoList", todoApp =>
            {
                todoApp.Run(async context =>
                {
                    var response = context.Response;
                    if (context.Request.Method.Equals("POST", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var reader = new StreamReader(context.Request.Body);
                        var body = await reader.ReadToEndAsync();
                        using (var json = JsonDocument.Parse(body))
                        {
                            var obj = json.RootElement;
                            var todo = new Todo() { Description = obj.GetProperty("Description").GetString(), Owner = context.User.Identity.Name };
                            Todos.Add(todo);
                        }
                    }
                    else
                    {
                        response.ContentType = "application/json";
                        response.Headers[HeaderNames.CacheControl] = "no-cache";
                        await response.StartAsync();
                        Serialize(Todos, response.BodyWriter);
                        await response.BodyWriter.FlushAsync();
                    }
                });
            });
        }

        private void Serialize(IList<Todo> todos, IBufferWriter<byte> output)
        {
            var writer = new Utf8JsonWriter(output);
            writer.WriteStartArray();
            foreach (var todo in todos)
            {
                writer.WriteStartObject();
                writer.WriteString("Description", todo.Description);
                writer.WriteString("Owner", todo.Owner);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.Flush();
        }
    }
}
