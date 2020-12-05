using DotNetCore.CAP;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Wby.Domain.OrderAggregate;
using Wby.Infrastructure.Repositories;

namespace Wby.API.Application.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, long>
    {
        IOrderRepository _orderRepository;
        ICapPublisher _capPublisher;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, ICapPublisher capPublisher)
        {
            _orderRepository = orderRepository;
            _capPublisher = capPublisher;
        }
        public async Task<long> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var address = new Address("wby", "wuhan", "310000");
            var order = new Order("xiaohong1999", "xiaohong", 24, address);

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return order.Id;
        }
    }
}
