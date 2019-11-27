using Honeywell.Infra.Core;
using Honeywell.Infra.Core.Modular;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.Gateway.Incident.Host
{
    [DependsOn(typeof(ModuleOfCore))]
    public class ModuleOfHost : Module<IServiceCollection>
    {

    }
}
