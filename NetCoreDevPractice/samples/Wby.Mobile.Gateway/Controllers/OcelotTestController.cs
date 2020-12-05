using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Wby.Mobile.Gateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OcelotTestController : ControllerBase
    {
        public IActionResult Abc()
        {
            return Content("Port:5003,Wby.Mobile.Gateway");
        }

        public IActionResult ShowConfig([FromServices] IConfiguration configuration)
        {
            return Content(configuration["ENV_ABC"]);
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
    }
}
