using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Wby.Ordering.API.Grpc
{
    public class OrderServiceImpl : OrderService.OrderServiceBase
    {
        ILogger _logger;

        public OrderServiceImpl(ILogger<OrderServiceImpl> logger)
        {
            _logger = logger;
        }

        public override async Task<Int64Value> CreateOrder(CreateOrderCommand request, ServerCallContext context)
        {
            _logger.LogInformation($"Port:5001, Call OrderServiceImpl.CreateOrder");
            return await Task.FromResult(new Int64Value { Value = 12L });
        }

        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new SearchResponse());
        }
    }
}
