using Honeywell.Gateway.Incident.Api;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Core;
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

            //var env = IocContainer.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
            //if (env.IsDevelopment())
            //{
            //    IocContainer.AddScoped<IWorkflowDesignAppService, StubWorkflowDesignAppService>();
            //}
            //else
            //{

            var configuration = IocContainer.BuildServiceProvider().GetRequiredService<IConfiguration>();

            IocContainer.AddScoped<IWorkflowDesignAppService, WorkflowDesignAppService>();
            IocContainer.AddRemoteService(
                typeof(IWorkflowDesignApi).Assembly,
                config =>
                {
                    config.DefaultUrl = configuration["WorkflowServiceUrl"];
                }).BuildServiceProvider();
            //}

            IocContainer.AddHttpContextAccessor();
        }
    }
}
