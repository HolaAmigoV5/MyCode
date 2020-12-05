using System;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Wby.Ordering.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OcelotTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Abc()
        {
            return Content("Port:5001, Wby.Ordering.API");
        }

        [HttpGet]
        public IActionResult ShowRequestUri()
        {
            return Content(Request.GetDisplayUrl());
        }

        [HttpGet]
        public IActionResult ShowHeaders()
        {
            var sb = new StringBuilder();
            foreach (var item in Request.Headers)
            {
                sb.AppendLine($"{item.Key}:{item.Value}");
            }
            return Content(sb.ToString());
        }

        [HttpGet]
        public IActionResult Error()
        {
            throw new Exception("这是模拟异常");
        }
    }
}
