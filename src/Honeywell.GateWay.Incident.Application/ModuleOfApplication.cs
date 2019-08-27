using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.Extensions.Configuration;

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

            IocContainer.AddScoped<IWorkflowDesignAppService, WorkflowDesignAppService>();

            var configuration = IocContainer.BuildServiceProvider().GetRequiredService<IConfiguration>();
            IocContainer.AddRemoteService(
                typeof(IWorkflowDesignApi).Assembly,
                config =>
                {
                    config.DefaultUrl = configuration["WorkflowServiceUrl"];
                }).BuildServiceProvider();


            // for stub
            //IocContainer.AddScoped<IWorkflowDesignApi, StubWorkflowDesignAppService>();

            IocContainer.AddHttpContextAccessor();
        }
    }
}
