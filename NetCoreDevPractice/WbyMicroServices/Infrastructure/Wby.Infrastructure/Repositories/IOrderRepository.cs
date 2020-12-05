using System;
using System.Collections.Generic;
using System.Text;
using Wby.Domain.OrderAggregate;
using Wby.Infrastructure.Core;

namespace Wby.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<Order, long>
    {
    }
}
