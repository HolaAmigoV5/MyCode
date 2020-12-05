using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wby.API.Application.Commands
{
    public class CreateOrderCommand : IRequest<long>
    {
        public long ItemCount { get; private set; }
        public CreateOrderCommand(int itemCount)
        {
            ItemCount = itemCount;
        }
    }
}
