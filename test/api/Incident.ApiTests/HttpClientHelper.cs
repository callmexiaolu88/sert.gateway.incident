using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Incident.ApiTests
{
    public class HttpClientHelper : ProWatchClient
    {
        private readonly HttpClient _client;
        private IConfiguration _iConfiguration;

        public HttpClientHelper(IConfiguration configuration, HttpClient httpClient) : base(configuration, httpClient)
        {
            _client = httpClient;
            _iConfiguration = configuration;
        }

        public async Task<HttpResponseMessage> PostAsync(string apiUri, HttpContent content)
        {
            _client.DefaultRequestHeaders.Clear();
            var headers = InitHeader();
            foreach (var header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            string address=_iConfiguration.GetValue<string>("RemoteServicesConfig:DefaultUrl");
            var url = new Uri($"{address}{apiUri}");
            var response = await _client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"access {apiUri} execute error.");
            }

            return response;
        }

        private Dictionary<string, string> InitHeader()
        {
            var tokenHeader = $"Bearer {GetDefaultToken()}";
            var headers = new Dictionary<string, string>
            {
                {"Authorization", tokenHeader}, {"Accept-Language", "en-us"}
            };
            return headers;
        }
    }

}
