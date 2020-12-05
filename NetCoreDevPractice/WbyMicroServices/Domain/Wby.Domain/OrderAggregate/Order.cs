using Wby.Domain.Events;

namespace Wby.Domain.OrderAggregate
{
    public class Order : Entity<long>, IAggregateRoot
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public Address Address { get; private set; }
        public int ItemCount { get; private set; }

        protected Order() { }

        public Order(string userId, string userName, int itemCount, Address address)
        {
            UserId = userId;
            UserName = userName;
            Address = address;
            ItemCount = itemCount;

            AddDomainEvent(new OrderCreatedDomainEvent(this));
        }

        public void ChangeAddress(Address address)
        {
            Address = address;
        }
    }
}
