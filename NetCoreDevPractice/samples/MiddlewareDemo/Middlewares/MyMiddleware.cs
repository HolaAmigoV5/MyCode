using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareDemo.Middlewares
{
    public class MyMiddleware : IMiddleware
    {
        ILogger _logger;
        RequestDelegate _next;
        public MyMiddleware(RequestDelegate next, ILogger<MyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //类中有InvokeAsync方法返回Task。即可作为一个中间件注册进去

        public async Task InvokeAsync(HttpContext context)
        {
            using (_logger.BeginScope("TraceIdentifier:{TraceIdentifier}", context.TraceIdentifier))
            {
                _logger.LogDebug("开始执行");
                await _next(context);
                _logger.LogDebug("执行结束");
            }
        }


        //这里实现IMiddleware接口
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
