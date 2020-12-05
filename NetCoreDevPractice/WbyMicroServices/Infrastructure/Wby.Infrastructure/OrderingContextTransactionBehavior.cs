using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Wby.Infrastructure.Core.Behaviors;

namespace Wby.Infrastructure
{
    public class OrderingContextTransactionBehavior<TRequest, TResponse>
        : TransactionBehavior<OrderingContext, TRequest, TResponse>
    {
        public OrderingContextTransactionBehavior(OrderingContext dbContext,
            ILogger<OrderingContextTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
        {

        }
    }
}
