using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Wby.API.Application.Commands;
using Wby.API.Application.Queries;

namespace Wby.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IMediator _mediator;
        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<long> CreateOrder([FromBody]CreateOrderCommand cmd)
        {
            //使用MediaR实现命令的构造和命令的处理分离开
            return await _mediator.Send(cmd, HttpContext.RequestAborted);
        }
        
        [HttpGet]
        public async Task<List<string>> QueryOrder([FromQuery]MyOrderQuery myOrderQuery)
        {
            //使用MediaR实现查询和查理处理分离
            return await _mediator.Send(myOrderQuery);
        }
    }
}
