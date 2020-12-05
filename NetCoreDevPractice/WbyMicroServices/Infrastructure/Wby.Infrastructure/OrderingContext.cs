using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Wby.Domain.OrderAggregate;
using Wby.Infrastructure.Core;
using Wby.Infrastructure.EntityConfigurations;

namespace Wby.Infrastructure
{
    public class OrderingContext : EFContext
    {
        public OrderingContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus)
            : base(options, mediator, capBus)
        {

        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //注册领域模型与数据库的映射关系
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
