using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RoutingDemo.Controllers
{
    //RouteAttribute方式实现
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //类型约束
        [HttpGet("{id:isLong}")]
        public bool OrderExist([FromRoute]string id)
        {
            return true;
        }

        //范围约束
        [HttpGet("{id:max(20)}")]
        public bool Max([FromRoute] long id, [FromServices]LinkGenerator linkGenerator)
        {
            //通过Action获取路径
            var path = linkGenerator.GetPathByAction(HttpContext,
                action: "Reque",
                controller: "Order",
                values: new { name = "abc" });
            //通过Action获取Uri
            var uri = linkGenerator.GetUriByAction(HttpContext,
                action: "Reque",
                controller: "Order",
                values: new { name = "abc" });

            //var a = linkGenerator.GetPathByAction("Reque", "Order");

            return true;
        }

        //是否必选
        [HttpGet("{name:required}")]
        [Obsolete] //定义废弃接口
        public bool Reque(string name)
        {
            return true;
        }

        //正则表达式约束
        [HttpGet("{number:regex(^\\d{{3}}$)}")]
        public bool Number(string number)
        {
            return true;
        }
    }
}
