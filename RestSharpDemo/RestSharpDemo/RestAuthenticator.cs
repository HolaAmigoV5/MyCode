using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json.Serialization;

namespace RestSharpDemo
{
    internal class RestAuthenticator : AuthenticatorBase
    {
        readonly string _baseUrl;
        readonly string _clientId;
        readonly string _clientSecret;

        public RestAuthenticator(string baseUrl, string clientId, string clientSecret) : base("")
        {
            _baseUrl = baseUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            Token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, Token);
        }

        async Task<string> GetToken()
        {
            var options = new RestClientOptions(_baseUrl);
            using var client = new RestClient(options)
            {
                Authenticator = new HttpBasicAuthenticator(_clientId, _clientSecret)
            };
            var request = new RestRequest("oauth2/token").AddParameter("grant_type", "client_credentials");
            var response = await client.PostAsync<TokenResponse>(request);
            return $"{response!.TokenType}{response!.AccessToken}";
        }
    }

    record TokenResponse
    {
        [JsonPropertyName("token_type")]
        public string? TokenType { get; init; }
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; init; }
    }
}
