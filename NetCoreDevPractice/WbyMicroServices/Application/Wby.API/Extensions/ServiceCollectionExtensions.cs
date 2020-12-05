using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Wby.API.Application.IntegrationEvents;
using Wby.Domain.OrderAggregate;
using Wby.Infrastructure;
using Wby.Infrastructure.Repositories;

namespace Wby.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediaRServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OrderingContextTransactionBehavior<,>));
            return services.AddMediatR(typeof(Order).Assembly, typeof(Program).Assembly);
        }

        public static IServiceCollection AddDomainContext(this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<OrderingContext>(optionsAction);
        }

        public static IServiceCollection AddInMemoryDomainContext(this IServiceCollection services)
        {
            return services.AddDomainContext(builder => builder.UseInMemoryDatabase("domainContextDatabase"));
        }

        public static IServiceCollection AddMySqlDomainContext(this IServiceCollection services, string connectionString)
        {
            return services.AddDomainContext(builder => {
                builder.UseMySql(connectionString);
            });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISubscriberService, SubscriberService>();
            services.AddCap(options => {
                //EventBus和OrderingContext共享
                options.UseEntityFramework<OrderingContext>();
                options.UseRabbitMQ(options =>
                {
                    //使用RabbitMQ进行消息队列存储
                    configuration.GetSection("RabbitMQ").Bind(options);
                });
            });
            return services;
        }
    }
}
