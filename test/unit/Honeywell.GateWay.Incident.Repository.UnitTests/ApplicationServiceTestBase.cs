using AutoMapper;
using Honeywell.GateWay.Incident.Application;
using Honeywell.Infra.Core;
using Honeywell.Infra.Core.HoneyMapper;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Honeywell.Infra.HoneyMapper.AutoMapper.Imp;
using Microsoft.Extensions.DependencyInjection;

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class ApplicationServiceTestBase
    {
        protected ApplicationServiceTestBase()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(typeof(AutomapperAccessor));
            serviceCollection.AddSingleton<IHoneyMapper, AutoMapperHoneyMapper>();
            var mapperConfig = new MapperConfiguration(
                cfg => { cfg.AddProfile(typeof(AutoMapperProfile)); });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<AutomapperAccessor>().Mapper = mapperConfig.CreateMapper();

            HoneyEngine.ServiceProvider = serviceProvider;
        }
    }
}
