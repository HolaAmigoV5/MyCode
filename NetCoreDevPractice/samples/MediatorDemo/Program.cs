using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorDemo
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Program).Assembly);

            var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetService<IMediator>();


            await mediator.Publish(new MyEvent { EventName = "event01" });
            //await mediator.Send(new MyCommand { CommandName = "cmd01" });

            Console.ReadLine();
        }
    }

    #region MediatR实现CRPS模式
    internal class MyCommand : IRequest<long>
    {
        public string CommandName { get; set; }
    }

    internal class MyCommandHandlerV2 : IRequestHandler<MyCommand, long>
    {
        public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyCommandHandler执行命令V2：{request.CommandName}");
            return Task.FromResult(10L);
        }
    }

    internal class MyCommandHandler : IRequestHandler<MyCommand, long>
    {
        public Task<long> Handle(MyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyCommandHandler执行命令：{request.CommandName}");
            return Task.FromResult(10L);
        }
    }
    #endregion

    #region MeddiatR处理领域事件
    internal class MyEvent : INotification
    {
        public string EventName { get; set; }
    }


    //一对多关心，一个领域事件可以由多个事件处理器处理
    internal class MyEventHandler : INotificationHandler<MyEvent>
    {
        public Task Handle(MyEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyEventHandler执行：{notification.EventName}");
            return Task.CompletedTask;
        }
    }

    internal class MyEventHandlerV2 : INotificationHandler<MyEvent>
    {
        public Task Handle(MyEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"MyEventHandlerV2执行：{notification.EventName}");
            return Task.CompletedTask;
        }
    } 
    #endregion

}
