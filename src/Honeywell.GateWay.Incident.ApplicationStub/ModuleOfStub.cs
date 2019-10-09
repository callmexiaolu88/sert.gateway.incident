using Honeywell.GateWay.Incident.Application;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Core.Modular;
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
                        typeof(IIncidentAppService),
                        typeof(StubIncidentAppService),
                        ServiceLifetime.Transient);
                IocContainer.Replace(incidentDescriptor);
            }
        }
    }
}
