using System.Net.Http;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Infra.Client.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Incident.ApiTests
{
    public class DIFixture
    {
        public ServiceProvider ServiceProvider { get; }
        public DIFixture()
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

            serviceCollection.AddRemoteService(typeof(IIncidentApi).Assembly, o =>
            {
                var proWatchClient = serviceCollection.BuildServiceProvider().GetRequiredService<ProWatchClient>();
                o.DefaultTokenRetriever = proWatchClient.GetDefaultToken;
                config.GetSection("RemoteServicesConfig").Bind(o);
            });

            serviceCollection.AddHttpClient(typeof(IIncidentApi).GUID.ToString())
                .ConfigureHttpClient((provider, client) =>
                {
                    var service = provider.GetRequiredService<ProWatchClient>();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(service.LoginLanaugeSetting());
                });
            serviceCollection.AddHttpClient(typeof(IWorkflowDesignApi).GUID.ToString())
                .ConfigureHttpClient((provider, client) =>
                {
                    var service = provider.GetRequiredService<ProWatchClient>();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(service.LoginLanaugeSetting());
                });
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }

    [CollectionDefinition(Colections.DICollection)]
    public class DICollection : ICollectionFixture<DIFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public static class Colections
    {
        public const string DICollection= "DI-Collection";
    }
}
