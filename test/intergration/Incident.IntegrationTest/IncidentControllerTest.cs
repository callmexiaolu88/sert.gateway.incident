using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Incident.SystemTests
{
    public class IncidentControllerTest : TestOfBase
    {
        private readonly IIncidentApi _incidentGateWayApi;
        public IncidentControllerTest()
        {
            _incidentGateWayApi = ServiceProvider.GetService<IIncidentApi>();
        }

        [Fact]
        public void ImportWorkFlowDesign_Successful()
        {
            var result = ImportWorkFlowDesign();
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public void ServiceObjNotNull()
        {
            Assert.NotNull(_incidentGateWayApi);
        }

        [Fact]
        public async void GetAllWorkflowDesign_Successful()
        {
            var workflowDesigns = GetAllWorkflowDesign();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            Assert.True(workflowDesigns.Result.Length > 0);
        }

        private async Task<WorkflowDesignSummaryGto[]> GetAllWorkflowDesign()
        {
            var workflowDesigns = await _incidentGateWayApi.GetAllActiveWorkflowDesigns();
            return workflowDesigns;
        }

        private async Task<ExecuteResult> ImportWorkFlowDesign()
        {
            var resourceName = "Incident.SystemTests.Data.TestTemplate.docx";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var result = await _incidentGateWayApi.ImportWorkflowDesigns(stream);
            return result;
        }

        private async Task<ExecuteResult> ClearWorklfowDesign()
        {
            var workflowDesigns = GetAllWorkflowDesign();
            var result = await _incidentGateWayApi.DeleteWorkflowDesigns(workflowDesigns.Result.Select(m => m.Id.ToString()).ToArray());
            return result;
        }
    }

}
