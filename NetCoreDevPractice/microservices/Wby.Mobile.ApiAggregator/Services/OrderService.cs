using System.Net.Http;

namespace Wby.Mobile.ApiAggregator
{
    public class OrderService : IOrderService
    {
        //IHttpClientFactory _clientFactory;
        HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void GetOrder() { }
    }
}
