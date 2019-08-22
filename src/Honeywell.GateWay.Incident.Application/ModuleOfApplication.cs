using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Core;
using Honeywell.Infra.Core.Modular;
using Honeywell.Infra.HoneyMapper.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using Honeywell.Infra.Client.WebApi;
using Honeywell.Micro.Services.Workflow.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Honeywell.GateWay.Incident.Application
{
    public class ModuleOfApplication : Module<IServiceCollection>
    {
        private IUserProvider _userProvider;

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
                IocContainer.AddScoped<IWorkflowDesignAppService, WorkflowDesignAppService>();
                IocContainer.AddRemoteService(
                    typeof(IWorkflowDesignApi).Assembly,
                    config =>
                    {
                        config.DefaultUrl = "http://localhost/webapi";
                        config.CreateHttpClient = CreateHttpClient;
                    }).BuildServiceProvider();
            //}

            IocContainer.AddHttpContextAccessor();
        }

        public override void Start(AppInitContext appInitContext)
        {
            base.Start(appInitContext);

            _userProvider = appInitContext.ServiceProvider.GetService<IUserProvider>();
        }
        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _userProvider.GetToken());
            return client;
        }
    }
}
