using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Incident.ApiTests.ProWatchClient;

namespace Incident.ApiTests
{
    public class HttpClientHelper
    {
        private readonly HttpClient _client;
        private string _address;
        private string _host;
        private string _username;
        private string _password;

        public HttpClientHelper()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
            });

            InitParameterData();
        }

        public async Task<HttpResponseMessage> PostAsync(string apiUri, HttpContent content,
            Dictionary<string, string> headers)
        {
            _client.DefaultRequestHeaders.Clear();
            foreach (var header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var url = new Uri($"{_address}{apiUri}");
            var response = await _client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"access {apiUri} execute error.");
            }

            return response;
        }

        public Dictionary<string, string> InitHeader()
        {
            var tokenHeader = $"Bearer {GetToken()}";
            var headers = new Dictionary<string, string>
            {
                {"Authorization", tokenHeader}, {"Accept-Language", "en-us"}
            };
            return headers;
        }

        private void InitParameterData()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            config.Providers.First().TryGet("RemoteServicesConfig:DefaultUrl", out _address);
            config.Providers.First().TryGet("ProWatchIsomUrl", out _host);
            config.Providers.First().TryGet("UserName", out _username);
            config.Providers.First().TryGet("Password", out _password);
        }

        private string GetToken()
        {
            var tokenString = JsonConvert.DeserializeObject<TokenResult>(RequestTokenAsync(_host, _username, _password).Result);
            return tokenString.AccessToken;
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

        private RequestContent MockDefaultHttpContent(string username, string password)
        {
            var content = new RequestContent
            {
                Username = username,
                Password = password,
                workspace = "",
                ClientID = "ProWatchWebClient",
                ClientSecret = "secret",
                Scope = "prowatch offline_access"
            };
            return content;
        }
    }
}
