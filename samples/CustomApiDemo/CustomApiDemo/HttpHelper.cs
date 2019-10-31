using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CustomApiDemo
{
    public class HttpHelper
    {
        private ProWatchClient _proWatchClient;

        static HttpHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public HttpHelper()
        {
            _proWatchClient = new ProWatchClient(this);
        }

        public static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.AcceptLanguage.Add(
                new StringWithQualityHeaderValue(ConfigurationManager.AppSettings["DefaultLanguage"]));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<TResult> PostAsync<T, TResult>(string requestUri, T value)
        {
            var client = CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = _proWatchClient.GenerateTokenAsync();
            var httpResponse = client.PostAsJsonAsync(generatorIncidentUri(requestUri), value);
            httpResponse.Result.EnsureSuccessStatusCode();
            var result = await httpResponse.Result.Content.ReadAsAsync<TResult>();
            client.Dispose();
            return result;
        }

        public async Task<TResult> PostAsync<TResult>(string requestUri)
        {
            var client = CreateHttpClient();
            client.DefaultRequestHeaders.Authorization = _proWatchClient.GenerateTokenAsync();
            var httpResponse = client.PostAsync(generatorIncidentUri(requestUri), null);
            httpResponse.Result.EnsureSuccessStatusCode();
            var result = await httpResponse.Result.Content.ReadAsAsync<TResult>();
            client.Dispose();
            return result;
        }

        public async Task<TResult> ProWatchPostAsync<T, TResult>(string requestUri, T value)
        {
            var client = CreateHttpClient();
            var httpResponse = client.PostAsJsonAsync(requestUri, value);
            httpResponse.Result.EnsureSuccessStatusCode();
            var result = await httpResponse.Result.Content.ReadAsAsync<TResult>();
            client.Dispose();
            return result;
        }

        private Uri generatorIncidentUri(string requestUri)
        {
            string baseUri = ConfigurationManager.AppSettings["SercurityConsoleUrl"];
            var uriBuilder = new UriBuilder(baseUri);
            if (uriBuilder.Path.EndsWith("/"))
            {
                if (requestUri.StartsWith("/"))
                {
                    requestUri = requestUri.Substring(1);
                }
            }
            else
            {
                if (!requestUri.StartsWith("/"))
                {
                    requestUri = "/" + requestUri;
                }
            }

            uriBuilder.Path = uriBuilder.Path + requestUri;
            return uriBuilder.Uri;
        }
    }
}
