using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using gService = Wby.Ordering.API.Grpc;

namespace Wby.Mobile.ApiAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderService _orderService;
        gService.OrderService.OrderServiceClient _client;
        public OrderController(IOrderService orderService, gService.OrderService.OrderServiceClient client)
        {
            _orderService = orderService;
            _client = client;
        }

        [HttpGet]
        public ActionResult GetOrders([FromQuery]gService.SearchRequest request)
        {
            //添加其他服务的调用
            
            var data = _client.Search(request);
            return Ok(data.Orders);
        }
    }
}
