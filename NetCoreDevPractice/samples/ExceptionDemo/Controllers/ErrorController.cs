using ExceptionDemo.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExceptionDemo.Controllers
{
    //[AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("/error")]
        public IActionResult Index()
        {
            //通过Features。Get()方法获取上下文错误信息
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = exceptionHandlerPathFeature?.Error;

            //特殊处理，异常尝试转化为IKnownException。模式匹配实现
            if (!(ex is IKnownException knownException))
            {
                //对于未知的业务异常，输出指定的未知错误，但是日志中记录原有异常信息
                var logger = HttpContext.RequestServices.GetService<ILogger<MyExceptionFilterAttribute>>();
                logger.LogError(ex, ex.Message);
                knownException = KnownException.Unknown;
            }
            else
            {
                //对已知的异常直接输出
                knownException = KnownException.FromKnownException(knownException);
            }


            return View(knownException);
        }
    }
}
