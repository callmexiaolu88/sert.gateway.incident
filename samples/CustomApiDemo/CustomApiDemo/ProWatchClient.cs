using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CustomApiDemo
{
    public class ProWatchClient
    {
        private readonly HttpHelper _httpHelper;
        private string _token;

        public ProWatchClient(HttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        private async Task<TokenResult> RequestTokenAsync(string host, string username, string password)
        {
            var aaUrl =
                $"{host}/AuthenticationAuthorizationService/api/AuthenticationAuthorization/AuthenticateClientRequest";
            var bodyContent = MockDefaultHttpContent(username, password);
            var response= await _httpHelper.ProWatchPostAsync<RequestContent, TokenResult>(aaUrl, bodyContent);
            return response;
        }

        internal AuthenticationHeaderValue GenerateTokenAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                var host = ConfigurationManager.AppSettings["ProWatchIsomUrl"];
                var userName = ConfigurationManager.AppSettings["UserName"];
                var password = ConfigurationManager.AppSettings["Password"];
                _token = GetTokens(host, userName, password);
            }

            return new AuthenticationHeaderValue("Bearer", _token);

        }

        internal StringWithQualityHeaderValue LoginLanaugeSetting()
        {
            return new StringWithQualityHeaderValue(ConfigurationManager.AppSettings["DefaultLanguage"]);
        }

        private string GetTokens(string host, string username, string password)
        {
            var tokenObject = RequestTokenAsync(host, username, password).Result;
            return tokenObject.AccessToken;
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
