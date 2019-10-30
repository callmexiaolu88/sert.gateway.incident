using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CustomApiDemo
{
    public class HttpHelper
    {
        private const string RemoteServicesUri = "https://N0QTT7LWUNF7CYJ/webapi/";

        private const string Token =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IlVkd2xoSF9OZVU2LUtOS3BGOHNpbUFqeExubyIsImtpZCI6IlVkd2xoSF9OZVU2LUtOS3BGOHNpbUFqeExubyJ9.eyJpc3MiOiJodHRwczovL2RoOGJjZ3VhamF6MjJ5MC9UaGlua3RlY3R1cmVJZGVudGl0eVNlcnZpY2UiLCJhdWQiOiJodHRwczovL2RoOGJjZ3VhamF6MjJ5MC9UaGlua3RlY3R1cmVJZGVudGl0eVNlcnZpY2UvcmVzb3VyY2VzIiwiZXhwIjoxNTcyNDMzMDQxLCJuYmYiOjE1NzIzOTcwNDEsImNsaWVudF9pZCI6IlByb1dhdGNoV2ViQ2xpZW50Iiwic2NvcGUiOlsib2ZmbGluZV9hY2Nlc3MiLCJvcGVuaWQiLCJwcm93YXRjaCJdLCJzdWIiOiJhZG1pbmlzdHJhdG9yIiwiYXV0aF90aW1lIjoxNTcyMzk3MDQxLCJpZHAiOiJpZHNydiIsImV4dGVybmFsX3Byb3ZpZGVyX3VzZXJfaWQiOiJhZG1pbmlzdHJhdG9yIiwicm9sZSI6IkFkbWluaXN0cmF0b3IiLCJhbXIiOlsicGFzc3dvcmQiXX0.TGCnPmGolbnqBis27Ndo-_f-kYEUEhjtAsKNS-jU4Fr0IgCiR-_-j1ZD6Fr5oflj0c8A4-fC-LkDehlozeIGE_6Fy6l9I4pETSGlzvJgQAzHCm2sgp9RmOIpWV4_lT5CmKFXfq4SyHylCYeU2D5gNRWRfAQNBhAMK13ydUrtIesiT4_b_SauzQtJ3uzKdlU_EnwtN9M-pxGKiAxSsWelHUkGCLlPQqQP_d9nXd3hkfFVuIHZxIlADZEdwNUFxCv7-ZYs9IYXCnSgNHts7EZDFMEf_xGqgnzujUs3bEu3NVVPwVLPRn6jgjDbbYOQ0PRqO_6kJ0ybk3ZXJb860G9geA";
        private static HttpClient CreateHttpClient()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var client = new HttpClient
            {
                BaseAddress = new Uri(RemoteServicesUri)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            return client;
        }

        public Task<TResult> PostAsync<T, TResult>(string requestUri, T value)
        {
            var client = CreateHttpClient();
            var httpResponse = client.PostAsJsonAsync(requestUri, value);
            httpResponse.Result.EnsureSuccessStatusCode();
            var result = httpResponse.Result.Content.ReadAsAsync<TResult>();
            return result;
        }

        public Task<TResult> PostAsync<TResult>(string requestUri)
        {
            var client = CreateHttpClient();
            var httpResponse = client.PostAsync(requestUri, null);
            httpResponse.Result.EnsureSuccessStatusCode();
            var result = httpResponse.Result.Content.ReadAsAsync<TResult>();
            return result;
        }
    }
}
