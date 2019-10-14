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
            var result = ImportWorkflowDesign();
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetAllWorkflowDesign()
        {
            await ImportWorkflowDesign();
            var workflowDesigns = GetAllWorkflowDesigns();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            Assert.True(workflowDesigns.Result.Length > 0);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetWorkflowDetails()
        {
            await ImportWorkflowDesign();
            var workflowDesignId = GetFirstWorkflowDesignId();
            var workflowDesignGto = await _incidentGateWayApi.GetWorkflowDesignById(workflowDesignId);
            Assert.NotNull(workflowDesignGto);
            Assert.NotNull(workflowDesignGto.Name);
            Assert.NotNull(workflowDesignGto.Description);
            Assert.NotNull(workflowDesignGto.Steps);
            Assert.True(workflowDesignGto.Steps.Length > 0);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetSiteDevices_Success()
        {
            var result = await _incidentGateWayApi.GetSiteDevices();
            Assert.NotNull(result);
        }

        [Fact]
        public async void CreateIncident_Success()
        {
            await ImportWorkflowDesign();

            var incidentId = CreateIncident().Result;

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetIncidentById_WithDevice_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var devices = await _incidentGateWayApi.GetSiteDevices();
            if (devices.Length > 0)
            {
                var deviceId = devices[0].Devices[0].DeviceId;
                var deviceType = devices[0].Devices[0].DeviceType;
                var queryIncidentDetailsRequestGto = new QueryIncidentDetailsRequestGto
                    { IncidentId = incidentId, DeviceId = deviceId, DeviceType = deviceType };
                var incidentDetails = await _incidentGateWayApi.GetIncidentById(queryIncidentDetailsRequestGto);
                Assert.True(incidentDetails.Status == ExecuteStatus.Successful);
                Assert.Equal(incidentDetails.Id.ToString(), incidentId);
                Assert.Equal(incidentDetails.DeviceDisplayName, devices[0].Devices[0].DeviceDisplayName);
                Assert.Equal(incidentDetails.DeviceLocation, devices[0].Devices[0].DeviceLocation);
            }

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

        #region private methods

        private string GetFirstWorkflowDesignId()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            var workflowDesign = workflowDesigns.Result.FirstOrDefault();
            Assert.NotNull(workflowDesign);
            return workflowDesign.Id.ToString();
        }

        private async Task<WorkflowDesignSummaryGto[]> GetAllWorkflowDesigns()
        {
            var workflowDesigns = await _incidentGateWayApi.GetAllActiveWorkflowDesigns();
            return workflowDesigns;
        }

        private async Task<ExecuteResult> ImportWorkflowDesign()
        {
            var resourceName = "Incident.ApiTests.Data.TestTemplate.docx";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var result = await _incidentGateWayApi.ImportWorkflowDesigns(stream);
            return result;
        }

        private async Task DeleteWorkflowDesign()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            await _incidentGateWayApi.DeleteWorkflowDesigns(workflowDesigns.Result.Select(m => m.Id.ToString()).ToArray());
        }

        private async Task DeleteIncident(string incidentId)
        {
            var respondResult = await _incidentGateWayApi.RespondIncident(incidentId);
            Assert.True(respondResult.Status == ExecuteStatus.Successful);

            var closeResult = await _incidentGateWayApi.CloseIncident(incidentId, "test delete");
            Assert.True(closeResult.Status == ExecuteStatus.Successful);
        }

        private async Task<string> CreateIncident()
        {
            var workflowDesignId = GetFirstWorkflowDesignId();
            var incident = new CreateIncidentRequestGto
                { Description = "incident 1", Priority = "0", WorkflowDesignReferenceId = workflowDesignId };
            var result = await _incidentGateWayApi.CreateIncident(incident);
            Assert.NotNull(result);
            return result;
        }

        #endregion
    }

}
