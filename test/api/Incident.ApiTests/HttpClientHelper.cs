using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Incident.ApiTests
{
   public class HttpClientHelper
    {
        private readonly HttpClient _client;
        private readonly string _address;
        public HttpClientHelper()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
            });

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            config.Providers.First().TryGet("RemoteServicesConfig:DefaultUrl", out _address);
        }

        public async Task<HttpResponseMessage> PostAsync(string apiUri, HttpContent content, Dictionary<string, string> headers)
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
            var tokenHeader = $"Bearer {Token}";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", tokenHeader }, { "Accept-Language", "en-us" }
            };
            return headers;
        }

        private const string Token =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlVkd2xoSF9OZVU2LUtOS3BGOHNpbUFqeExubyIsImtpZCI6IlVkd2xoSF9OZVU2LUtOS3BGOHNpbUFqeExubyJ9.eyJpc3MiOiJodHRwczovL2MyZmFjeWhmYW9ia2Rrdi9UaGlua3RlY3R1cmVJZGVudGl0eVNlcnZpY2UiLCJhdWQiOiJodHRwczovL2MyZmFjeWhmYW9ia2Rrdi9UaGlua3RlY3R1cmVJZGVudGl0eVNlcnZpY2UvcmVzb3VyY2VzIiwiZXhwIjoxNTY5NTgyOTQ2LCJuYmYiOjE1Njk1NDY5NDYsImNsaWVudF9pZCI6IlByb1dhdGNoV2ViQ2xpZW50Iiwic2NvcGUiOlsib2ZmbGluZV9hY2Nlc3MiLCJvcGVuaWQiLCJwcm93YXRjaCJdLCJzdWIiOiJBZG1pbmlzdHJhdG9yIiwiYXV0aF90aW1lIjoxNTY5NTQ2OTQ2LCJpZHAiOiJpZHNydiIsImV4dGVybmFsX3Byb3ZpZGVyX3VzZXJfaWQiOiJBZG1pbmlzdHJhdG9yIiwiYW1yIjpbInBhc3N3b3JkIl19.peYihBbr6T5Fnn12JjjIdgiQ0UuJB45kSQLEzlGHNiVWokrn0jFSJ44yIC5xubo8h8r-R17rmU2LJrnh7X8Myb0-a71NFcC8uqShNrR4NNRM1RrCv6RJBzeDuwGjrMEyoKsDimQuHu54M1HJ-vUvatdQzqQEN6ULqIfAD0sZXktPFxMXlbMUfgqLws5NshTyo937n3_wmadxF2Q83kQgUA_R4Aiz9JoE_radBU6L6JdR43DKCR2LAAHUOEN6fAN6k2kjaNtzinYvcghvIBK1r_ujx65qJ6PWpsvYFMOwNgW4Qws4kGq_2uk1j8st_gxkcRwo67YzH_rtYFwvI-lkAw";
    }
}
