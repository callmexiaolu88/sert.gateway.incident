﻿using Honeywell.GateWay.Incident.Repository.Data;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Infra.Client.WebApi.Config;
using Honeywell.Infra.Core.Modular;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Repository
{
    public class ModuleOfRepository : Module<IServiceCollection>
    {
        public ModuleOfRepository(IServiceCollection icoContainer) : base(icoContainer) { }


        public override void InitializeDependencyInject()
        {
            IocContainer.AddScoped<IProwatchRepository, ProwatchRepository>();

            var config = IocContainer.BuildServiceProvider().GetService<IConfiguration>();
            var assembly = typeof(IProwatchApi).Assembly;
            IocContainer.AddRemoteService(assembly,
                o =>
                {
                    var configuration = new RemoteServiceConfiguration(config["ProWatchAddress"]);
                    o.ConfigRemoteService(assembly, configuration);
                });
        }
    }
}
