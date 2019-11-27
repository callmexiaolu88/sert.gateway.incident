using Honeywell.GateWay.Incident.Application;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.AspNetCore;
using Honeywell.Infra.Core;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.Gateway.Incident
{
    [DependsOn(typeof(ModuleOfCore),
        typeof(ModuleOfAutomapper),
        typeof(ModuleOfApplication),
        typeof(ModuleOfAspNetCore)
    )]
    public class ModuleOfIncident : Module<IServiceCollection>
    {

    }
}
