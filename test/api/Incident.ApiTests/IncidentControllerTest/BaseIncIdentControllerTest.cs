using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    public class BaseIncIdentControllerTest : TestOfBase
    {
        protected ServiceProvider ServiceProvider { get; }
        protected IIncidentApi IncidentGateWayApi { get; }
        public BaseIncIdentControllerTest(DIFixture dIFixture)
        {
            ServiceProvider = dIFixture.ServiceProvider;
            IncidentGateWayApi = ServiceProvider.GetService<IIncidentApi>();
        }

        protected string GetFirstWorkflowDesignId()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            var workflowDesign = workflowDesigns.Result.FirstOrDefault();
            Assert.NotNull(workflowDesign);
            return workflowDesign.Id.ToString();
        }
        protected string GetFirstWorkflowDesignName()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            var workflowDesign = workflowDesigns.Result.FirstOrDefault();
            Assert.NotNull(workflowDesign);
            return workflowDesign.Name;
        }

        protected async Task<WorkflowDesignSummaryGto[]> GetAllWorkflowDesigns()
        {
            var workflowDesigns = await IncidentGateWayApi.GetAllActiveWorkflowDesigns();
            return workflowDesigns;
        }

        protected async Task<ExecuteResult> ImportWorkflowDesign()
        {
            var resourceName = "Incident.ApiTests.Data.TestTemplate.docx";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var result = await IncidentGateWayApi.ImportWorkflowDesigns(stream);
            return result;
        }

        protected async Task DeleteWorkflowDesign()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            await IncidentGateWayApi.DeleteWorkflowDesigns(workflowDesigns.Result.Select(m => m.Id.ToString()).ToArray());
        }

        protected async Task DeleteIncident(string incidentId)
        {
            var respondResult = await IncidentGateWayApi.RespondIncident(incidentId);
            Assert.True(respondResult.Status == ExecuteStatus.Successful);

            var closeResult = await IncidentGateWayApi.CloseIncident(incidentId, "test delete");
            Assert.True(closeResult.Status == ExecuteStatus.Successful);
        }

        protected async Task<string> CreateIncident(string deviceId = null, string deviceType = null)
        {
            var workflowDesignId = GetFirstWorkflowDesignId();
            var incident = new CreateIncidentRequestGto
            {
                Description = "incident 1", Priority = "Low", WorkflowDesignReferenceId = workflowDesignId,
                DeviceId = deviceId, DeviceType = deviceType
            };
            var result = await IncidentGateWayApi.CreateIncident(incident);
            Assert.NotNull(result);
            return result;
        }
    }

}
