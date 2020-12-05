using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientFactoryDemo
{
    public class TypedOrderServiceClient
    {
        HttpClient _client;
        public TypedOrderServiceClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string> Get()
        {
            return await _client.GetStringAsync("/OrderService"); //这里使用相对路径来访问
        }
    }
}
