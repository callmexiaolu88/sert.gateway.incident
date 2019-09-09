using System.Net.Http;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Honeywell.Infra.Core;
using Honeywell.Infra.Core.Common.Helper;
using Honeywell.Infra.Core.Modular;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Honeywell.Gateway.Incident.Host
{
    public class Startup
    {
        private ModuleManager<IServiceCollection> _moduleManager;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var path = PathHelper.GetOutPutPath();
            var moduleManager = ModuleManager<IServiceCollection>.Create(services, path);
            moduleManager.InitializeModule();
            _moduleManager = moduleManager;

            var mvcBuilder = services.AddControllers();
            foreach (var assembly in moduleManager.LoadedAssemblies)
            {
                mvcBuilder.AddApplicationPart(assembly);
            }

            mvcBuilder.AddNewtonsoftJson();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HoneyEngine.ServiceProvider = app.ApplicationServices;
            _moduleManager.StartModule(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/IncidentGateway/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
