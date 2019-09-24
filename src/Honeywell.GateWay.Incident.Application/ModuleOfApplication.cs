using Honeywell.Facade.Services.Incident.Api;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;

namespace Honeywell.GateWay.Incident.Application
{
    public class ModuleOfApplication : Module<IServiceCollection>
    {
        public ModuleOfApplication(IServiceCollection icoContainer) : base(icoContainer) { }

        public override void InitializeDependencyInject()
        {
            IocContainer.Configure<AutoMapperOptions>(options =>
            {
                options.AddProfile<AutoMapperProfile>();
            });

            IocContainer.AddScoped<IIncidentAppService, IncidentAppService>();
            IocContainer.AddRemoteService(typeof(IWorkflowDesignApi).Assembly);
            IocContainer.AddRemoteService(typeof(IWorkflowInstanceApi).Assembly);
            IocContainer.AddRemoteService(typeof(IIncidentMicroApi).Assembly);
            IocContainer.AddRemoteService(typeof(IIncidentFacadeApi).Assembly);


            IocContainer.AddHttpContextAccessor();
        }
    }
}
