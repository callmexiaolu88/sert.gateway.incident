using System.Net.Http;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Infra.Client.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Incident.SystemTests
{
    public class TestOfBase
    {
        protected ServiceProvider ServiceProvider { get; }

        protected TestOfBase()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddHttpClient<ProWatchClient>()
                .ConfigurePrimaryHttpMessageHandler(opt => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyErrors) => true
                });

            serviceCollection.AddRemoteService(typeof(IIncidentApi).Assembly);
            serviceCollection.AddHttpClient(typeof(IIncidentApi).GUID.ToString())
                .ConfigureHttpClient((provider, client) =>
                {
                    var service = provider.GetRequiredService<ProWatchClient>();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(service.LoginLanaugeSetting());
                    client.DefaultRequestHeaders.Authorization = service.GenerateTokenAsync();
                });
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
