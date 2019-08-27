using AutoMapper;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Core;
using Honeywell.Infra.Core.HoneyMapper;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Honeywell.Infra.HoneyMapper.AutoMapper.Imp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class ApplicationServiceTestBase
    {
        public ApplicationServiceTestBase()
        {
            var loggerMock = new Mock<ILogger<WorkflowDesignAppService>>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(x => loggerMock.Object);
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
