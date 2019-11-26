using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Core.Modular.Steps;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class ModuleOfApplicationTest
    {
        [Fact]
        public void TestModuleOfApp()
        {
            var sc = new ServiceCollection();
            sc.AddLogging();
            var moduleOfApplication = new ModuleOfApplication(sc);

            var context = new ConfigureServicesContext<IServiceCollection>(sc);
            moduleOfApplication.ConfigureServices(context);
            Assert.Contains(sc, s => s.ServiceType == typeof(IIncidentAppService));
            Assert.Contains(sc, s => s.ServiceType == typeof(IConfigureOptions<AutoMapperOptions>));
        }
    }
}
