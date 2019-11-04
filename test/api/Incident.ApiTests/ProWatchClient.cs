using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Incident.ApiTests
{
    public class ProWatchClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        public ProWatchClient(IConfiguration configuration, HttpClient httpClient)
        {
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
            _configuration = configuration;
            _client = httpClient;
        }

        private async Task<string> RequestTokenAsync(string host, string username, string password)
        {
            var aaUrl = new Uri($"{host}/AuthenticationAuthorizationService/api/AuthenticationAuthorization/AuthenticateClientRequest");
            var bodyContent = MockDefaultHttpContent(username, password);
            var content = new StringContent(JsonConvert.SerializeObject(bodyContent), Encoding.UTF8, "application/json");
            var response = _client.PostAsync(aaUrl, content).Result;
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        internal AuthenticationHeaderValue GenerateTokenAsync()
        {
            var token = GetDefaultToken();
            return new AuthenticationHeaderValue("Bearer", token);
        }

        internal string GetDefaultToken()
        {
            var host = _configuration.GetValue<string>("ProWatchIsomUrl");
            var userName = _configuration.GetValue<string>("UserName");
            var password = _configuration.GetValue<string>("Password");
            return GetTokens(host, userName, password);
        }

        internal StringWithQualityHeaderValue LoginLanaugeSetting()
        {
            return new StringWithQualityHeaderValue(_configuration.GetValue<string>("DefaultLanguage"));
        }

        private string GetTokens(string host, string username, string password)
        {
            var tokenString = JsonConvert.DeserializeObject<TokenResult>(RequestTokenAsync(host, username, password).Result);
            return tokenString.AccessToken;
        }

        private RequestContent MockDefaultHttpContent(string username, string password)
        {
            var content = new RequestContent
            {
                Username = username,
                Password = password,
                Workspace = "",
                ClientId = "ProWatchWebClient",
                ClientSecret = "secret",
                Scope = "prowatch offline_access"
            };
            return content;
        }

        public class RequestContent
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Workspace { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scope { get; set; }
        }

        public class TokenResult
        {
            public string AccessToken { get; set; }
            public int StatusCode { get; set; }
            public object ErrorDescription { get; set; }
            public int TokenToLive { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
