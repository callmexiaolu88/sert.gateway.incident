using Honeywell.Facade.Services.Incident.Api;
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.GateWay.Incident.Repository.WorkflowDesign;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Infra.Client.WebApi.Config;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.Core.Modular.Steps;
using Honeywell.Infra.Services.LiveData.Api;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Repository
{
    public class ModuleOfRepository : Module<IServiceCollection>
    {
        public ModuleOfRepository(IServiceCollection icoContainer) : base(icoContainer) { }


        public override void ConfigureServices(ConfigureServicesContext<IServiceCollection> context)
        {
            context.Services.AddScoped<IDeviceRepository, DeviceRepository>();
            context.Services.AddScoped<IIncidentRepository, IncidentRepository>();
            context.Services.AddScoped<IWorkflowDesignRepository, WorkflowDesignRepository>();
            var config = context.Services.BuildServiceProvider().GetService<IConfiguration>();
            var assembly = typeof(IDeviceApi).Assembly;
            context.Services.AddRemoteService(assembly,
                o =>
                {
                    var configuration = new RemoteServiceConfiguration(config["ProWatchAddress"]);
                    o.ConfigRemoteService(assembly, configuration);
                });

            context.Services.AddRemoteService(typeof(IWorkflowDesignMicroApi).Assembly);
            context.Services.AddRemoteService(typeof(IWorkflowMicroApi).Assembly);
            context.Services.AddRemoteService(typeof(IIncidentMicroApi).Assembly);
            context.Services.AddRemoteService(typeof(IIncidentFacadeApi).Assembly);
            context.Services.AddRemoteService(typeof(ILiveDataApi).Assembly);
            context.Services.AddHttpContextAccessor();
        }
    }
}
