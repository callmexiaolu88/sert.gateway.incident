using Honeywell.Facade.Services.Incident.Api;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.GateWay.Incident.Repository.WorkflowDesign;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.Core.Modular.Steps;
using Honeywell.Infra.Services.LiveData.Api;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Repository
{
    public class ModuleOfRepository : Module<IServiceCollection>
    {
        public override void ConfigureServices(ConfigureServicesContext<IServiceCollection> context)
        {
            context.Services.AddScoped<IIncidentRepository, IncidentRepository>();
            context.Services.AddScoped<IWorkflowDesignRepository, WorkflowDesignRepository>();
            context.Services.AddRemoteService(typeof(IWorkflowDesignMicroApi).Assembly);
            context.Services.AddRemoteService(typeof(IWorkflowMicroApi).Assembly);
            context.Services.AddRemoteService(typeof(IIncidentMicroApi).Assembly);
            context.Services.AddRemoteService(typeof(IIncidentFacadeApi).Assembly);
            context.Services.AddRemoteService(typeof(ILiveDataApi).Assembly);
            context.Services.AddHttpContextAccessor();
        }
    }
}
