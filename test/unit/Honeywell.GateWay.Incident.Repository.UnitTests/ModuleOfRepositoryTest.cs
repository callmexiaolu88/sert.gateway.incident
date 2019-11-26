using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Infra.Core.Modular.Steps;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Honeywell.Infra.Services.LiveData.Api;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class ModuleOfRepositoryTest
    {
        [Fact]
        public void InitializeDependencyInjectTest_Successful()
        {

            var sc = new ServiceCollection();
            sc.AddLogging();

            var configuration = new ConfigurationBuilder().Build();
            sc.AddSingleton<IConfiguration>(configuration);
            
            var module = new ModuleOfRepository(sc);

            var context = new ConfigureServicesContext<IServiceCollection>(sc);
            module.ConfigureServices(context);

            Assert.NotNull(sc);
            Assert.Contains(sc, s => s.ServiceType == typeof(IIncidentRepository));
            Assert.Contains(sc, s => s.ServiceType == typeof(IWorkflowDesignRepository));
            Assert.Contains(sc, s => s.ServiceType == typeof(IConfiguration));
            Assert.Contains(sc, s => s.ServiceType == typeof(IDeviceApi));
            Assert.Contains(sc, s => s.ServiceType == typeof(IWorkflowDesignMicroApi));
            Assert.Contains(sc, s => s.ServiceType == typeof(IWorkflowMicroApi));
            Assert.Contains(sc, s => s.ServiceType == typeof(IIncidentMicroApi));
            Assert.Contains(sc, s => s.ServiceType == typeof(IIncidentFacadeApi));
            Assert.Contains(sc, s => s.ServiceType == typeof(ILiveDataApi));
        }
    }
}
