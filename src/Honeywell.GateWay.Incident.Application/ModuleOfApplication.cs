using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Application
{
    [DependsOn(typeof(ModuleOfRepository))]
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
            IocContainer.AddHttpContextAccessor();
        }
    }
}
