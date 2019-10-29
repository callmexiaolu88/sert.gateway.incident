using Honeywell.Facade.Services.Incident.Api;
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.GateWay.Incident.Repository.WorkflowDesign;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Infra.Client.WebApi.Config;
using Honeywell.Infra.Core.Modular;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Repository
{
    public class ModuleOfRepository : Module<IServiceCollection>
    {
        public ModuleOfRepository(IServiceCollection icoContainer) : base(icoContainer) { }


        public override void InitializeDependencyInject()
        {
            IocContainer.AddScoped<IDeviceRepository, DeviceRepository>();
            IocContainer.AddScoped<IIncidentRepository, IncidentRepository>();
            IocContainer.AddScoped<IWorkflowDesignRepository, WorkflowDesignRepository>();
            var config = IocContainer.BuildServiceProvider().GetService<IConfiguration>();
            var assembly = typeof(IDeviceApi).Assembly;
            IocContainer.AddRemoteService(assembly,
                o =>
                {
                    var configuration = new RemoteServiceConfiguration(config["ProWatchAddress"]);
                    o.ConfigRemoteService(assembly, configuration);
                });

            IocContainer.AddRemoteService(typeof(IWorkflowDesignMicroApi).Assembly);
            IocContainer.AddRemoteService(typeof(IWorkflowMicroApi).Assembly);
            IocContainer.AddRemoteService(typeof(IIncidentMicroApi).Assembly);
            IocContainer.AddRemoteService(typeof(IIncidentFacadeApi).Assembly);
            IocContainer.AddHttpContextAccessor();
        }
    }
}
