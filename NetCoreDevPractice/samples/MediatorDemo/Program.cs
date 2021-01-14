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
            //注册MediatR
            var services = new ServiceCollection();
            services.AddMediatR(typeof(Program).Assembly);

            //获取MediaR
            var serviceProvider = services.BuildServiceProvider();
            var mediator = serviceProvider.GetService<IMediator>();


            //使用MediaR实现命令的构造和命令的处理分离开
            //await mediator.Publish(new MyEvent { EventName = "event01" });
            await mediator.Send(new MyCommand { CommandName = "cmd01" });

            Console.ReadLine();
        }
    }

    #region MediatR实现CQRS模式
    internal class MyCommand : IRequest<long>
    {
        public string CommandName { get; set; }
    }

    //对于多个Handler，只会处理最后一个注册的IRequestHandler
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

    #region MediatR处理领域事件
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
