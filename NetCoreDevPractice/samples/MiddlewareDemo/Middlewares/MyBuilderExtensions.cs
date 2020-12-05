using MiddlewareDemo.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class MyBuilderExtensions
    {
        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder app)
        {
            //注册自己的中间件
            return app.UseMiddleware<MyMiddleware>();
        }
    }
}
