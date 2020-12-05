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
            return await _mediator.Send(cmd, HttpContext.RequestAborted);
        }
        
        [HttpGet]
        public async Task<List<string>> QueryOrder([FromQuery]MyOrderQuery myOrderQuery)
        {
            return await _mediator.Send(myOrderQuery);
        }
    }
}
