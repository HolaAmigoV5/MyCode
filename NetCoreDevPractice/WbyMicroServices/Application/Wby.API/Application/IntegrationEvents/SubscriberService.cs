using DotNetCore.CAP;
using MediatR;
using System;

namespace Wby.API.Application.IntegrationEvents
{
    public class SubscriberService : ISubscriberService, ICapSubscribe
    {
        IMediator _mediator;
        public SubscriberService(IMediator mediator)
        {
            _mediator = mediator;
        }

        [CapSubscribe("OrderPaymentSucceeded")]
        public void OrderPaymentSucceeded(OrderPaymentSucceededIntegrationEvent @event)
        {
            Console.WriteLine("订阅执行OrderPaymentSucceeded");
        }

        [CapSubscribe("OrderCreated")]
        public void OrderCreated(OrderCreatedIntegrationEvent @event)
        {
            Console.WriteLine($"订阅执行OrderCreated, OrderId：{@event.OrderId}");
        }
    }
}
