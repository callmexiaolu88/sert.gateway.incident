using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class UpdateWorkflowStepStatusTest: BaseIncIdentControllerTest
    {
        public UpdateWorkflowStepStatusTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void UpdateWorkflowStepStatus_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var devices = await IncidentGateWayApi.GetSiteDevices();
            Assert.True(devices.Length > 0);
            var deviceId = devices[0].Devices[0].DeviceId;
            var deviceType = devices[0].Devices[0].DeviceType;

            var queryIncidentDetailsRequestGto = new GetIncidentDetailsRequestGto
            {
                IncidentId = incidentId,
                DeviceId = deviceId,
                DeviceType = deviceType
            };

            var incidentDetails = await IncidentGateWayApi.GetIncidentById(queryIncidentDetailsRequestGto);
            var workflowStepId = incidentDetails.IncidentSteps[0].Id;
            var result = await IncidentGateWayApi.UpdateWorkflowStepStatus(workflowStepId.ToString(), true);
            Assert.True(result.Status == ExecuteStatus.Successful);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
