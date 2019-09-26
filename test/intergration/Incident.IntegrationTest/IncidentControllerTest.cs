using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Incident.ApiTests
{
    public class IncidentControllerTest : TestOfBase
    {
        private readonly IIncidentApi _incidentGateWayApi;
        public IncidentControllerTest()
        {
            _incidentGateWayApi = ServiceProvider.GetService<IIncidentApi>();
        }

        [Fact]
        public async void ImportWorkFlowDesign()
        {
            var result = Import();
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
            await Delete();
        }


        [Fact]
        public async void GetAllWorkflowDesign()
        {
            await Import();
            var workflowDesigns = GetAll();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            Assert.True(workflowDesigns.Result.Length > 0);
            await Delete();
        }

        [Fact]
        public async void GetWorkflowDetial()
        {
            await Import();
            var workflowDesigns = GetAll();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            var workflowDesign = workflowDesigns.Result.FirstOrDefault();
            Assert.NotNull(workflowDesign);
            var wrokflowDesignGto = await _incidentGateWayApi.GetWorkflowDesignById(workflowDesign.Id.ToString());
            Assert.NotNull(wrokflowDesignGto);
            Assert.NotNull(wrokflowDesignGto.Name);
            Assert.NotNull(wrokflowDesignGto.Description);
            Assert.NotNull(wrokflowDesignGto.Steps);
            Assert.True(wrokflowDesignGto.Steps.Length > 0);
            await Delete();
        }

        private async Task<WorkflowDesignSummaryGto[]> GetAll()
        {
            var workflowDesigns = await _incidentGateWayApi.GetAllActiveWorkflowDesigns();
            return workflowDesigns;
        }

        private async Task<ExecuteResult> Import()
        {
            var resourceName = "Incident.ApiTests.Data.TestTemplate.docx";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var result = await _incidentGateWayApi.ImportWorkflowDesigns(stream);
            return result;
        }

        private async Task Delete()
        {
            var workflowDesigns = GetAll();
            await _incidentGateWayApi.DeleteWorkflowDesigns(workflowDesigns.Result.Select(m => m.Id.ToString()).ToArray());
        }
    }

}
