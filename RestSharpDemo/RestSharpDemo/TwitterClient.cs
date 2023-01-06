using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpDemo
{
    internal class TwitterClient : ITwitterClient, IDisposable
    {
        readonly RestClient _client;
        public TwitterClient(string apiKey, string apiKeySecret)
        {
            var options = new RestClientOptions("https://api.twitter.com/2");
            _client = new RestClient(options)
            {
                Authenticator = new RestAuthenticator("https://api.twitter.com", apiKey, apiKeySecret)
            };
        }

        public async Task<TwitterUser> GetUser(string user)
        {
            var response = await _client.GetJsonAsync<TwitterSingleObject<TwitterUser>>(
                "users/by/username/{user}",new {user });
            return response!.Data;
        }

        record TwitterSingleObject<T>(T Data, string msg);

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
