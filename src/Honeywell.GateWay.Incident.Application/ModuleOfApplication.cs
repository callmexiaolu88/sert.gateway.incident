using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.Core.Modular.Steps;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Application
{
    [DependsOn(typeof(ModuleOfRepository))]
    public class ModuleOfApplication : Module<IServiceCollection>
    {
        public override void ConfigureServices(ConfigureServicesContext<IServiceCollection> context)
        {
            context.Services.Configure<AutoMapperOptions>(options =>
            {
                options.AddProfile<AutoMapperProfile>();
            });
            context.Services.AddScoped<IIncidentAppService, IncidentAppService>();
            context.Services.AddScoped<IWorkflowDesignAppService, WorkflowDesignAppService>();
            context.Services.AddHttpContextAccessor();
        }
    }
}
