using Grpc.Core;
using GrpcServices;
using System;
using System.Threading.Tasks;

namespace GrpcServerDemo.GrpcServices
{
    public class OrderService : OrderGrpc.OrderGrpcBase
    {
        public override Task<CreateOrderResult> CreateOrder(CreateOrderCommand request, ServerCallContext context)
        {
            //这里抛出异常，测试异常拦截器能否拦截
            //throw new Exception("Order error");

            //添加创建订单的内部逻辑，录入将订单信息存储到数据库
            return Task.FromResult(new CreateOrderResult { OrderId = 24 });
        }
    }
}
