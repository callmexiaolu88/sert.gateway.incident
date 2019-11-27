using Honeywell.GateWay.Incident.Application;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.Core.Modular.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    [DependsOn(typeof(ModuleOfApplication))]
    public class ModuleOfStub : Module<IServiceCollection>
    {
        public override void ConfigureServices(ConfigureServicesContext<IServiceCollection> context)
        {
            var provider = context.Services.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();
            var isStubModel = config.GetValue<bool>("StubModel");

            if (isStubModel)
            {
                var incidentDescriptor =
                    new ServiceDescriptor(
                        typeof(IIncidentAppService),
                        typeof(StubIncidentAppService),
                        ServiceLifetime.Transient);
                context.Services.Replace(incidentDescriptor);

                var workflowDescriptor =
                    new ServiceDescriptor(
                        typeof(IWorkflowDesignAppService),
                        typeof(StubWorkflowDesignAppService),
                        ServiceLifetime.Transient);
                context.Services.Replace(workflowDescriptor);
            }
        }
    }
}
