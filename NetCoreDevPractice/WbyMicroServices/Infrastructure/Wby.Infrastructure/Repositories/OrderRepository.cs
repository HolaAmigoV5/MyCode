using System;
using System.Collections.Generic;
using System.Text;
using Wby.Domain.OrderAggregate;
using Wby.Infrastructure.Core;

namespace Wby.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, long, OrderingContext>, IOrderRepository
    {
        public OrderRepository(OrderingContext context) : base(context)
        {

        }
    }
}
