using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wby.API.Application.IntegrationEvents
{
    public class OrderPaymentSucceededIntegrationEvent
    {
        public OrderPaymentSucceededIntegrationEvent(long orderId) => OrderId = orderId;
        public long OrderId { get; }
    }
}
