using Honeywell.GateWay.Incident.Application;
using Honeywell.GateWay.Incident.ApplicationStub.Incident;
using Honeywell.GateWay.Incident.ApplicationStub.Workflow;
using Honeywell.GateWay.Incident.ApplicationStub.WorkflowDesign;
using Honeywell.Infra.Core.Modular;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    [DependsOn(typeof(ModuleOfApplication))]
    public class ModuleOfStub : Module<IServiceCollection>
    {
        public ModuleOfStub(IServiceCollection icoContainer) : base(icoContainer)
        {
        }

        public override void InitializeDependencyInject()
        {
            var provider = IocContainer.BuildServiceProvider();
            var config = provider.GetService<IConfiguration>();
            var isStubModel = config.GetValue<bool>("StubModel");

            if (isStubModel)
            {
                var incidentDescriptor =
                    new ServiceDescriptor(
                        typeof(IIncidentMicroApi),
                        typeof(StubIncidentAppService),
                        ServiceLifetime.Transient);
                IocContainer.Replace(incidentDescriptor);

                var workflowDescriptor = new ServiceDescriptor(
                    typeof(IWorkflowInstanceApi),
                    typeof(StubWorkflowApiService),
                    ServiceLifetime.Transient);
                IocContainer.Replace(workflowDescriptor);

                var workflowDesignDescriptor = new ServiceDescriptor(
                    typeof(IWorkflowDesignApi),
                    typeof(StubWorkflowDesignAppService),
                    ServiceLifetime.Transient);
                IocContainer.Replace(workflowDesignDescriptor);
            }
        }
    }
}
