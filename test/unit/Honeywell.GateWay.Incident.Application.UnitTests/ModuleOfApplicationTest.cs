using Honeywell.GateWay.Incident.Application.Incident;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class ModuleOfApplicationTest
    {
        [Fact]
        public void TestModuleOfApp()
        {
            var serviceCollection = new ServiceCollection();
            var moduleOfApplication = new ModuleOfApplication(serviceCollection);
            moduleOfApplication.InitializeDependencyInject();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var incidentService = serviceProvider.GetService<IIncidentAppService>();
            Assert.NotNull(incidentService);
        }
    }
}
