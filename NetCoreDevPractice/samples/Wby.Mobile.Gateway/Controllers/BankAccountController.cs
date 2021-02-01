using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wby.Mobile.Gateway.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        [Authorize]
        public IActionResult Cookie()
        {
            return Content("bank account");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Jwt()
        {
            return Content(User.FindFirst("Name").Value);
        }

        
        //这里可以用JWT身份认证，也可以用cookie身份认证
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + 
            CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult AnyOne()
        {
            return Content(User.FindFirst("Name").Value);
        }
    }
}
