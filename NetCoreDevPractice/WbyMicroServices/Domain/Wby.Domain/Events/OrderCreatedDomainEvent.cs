using Wby.Domain.OrderAggregate;

namespace Wby.Domain.Events
{
    public class OrderCreatedDomainEvent : IDomainEvent
    {
        public Order Order { get; private set; }
        public OrderCreatedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
