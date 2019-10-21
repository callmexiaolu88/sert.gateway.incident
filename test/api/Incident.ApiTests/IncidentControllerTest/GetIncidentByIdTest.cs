using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetIncidentByIdTest: BaseIncIdentControllerTest
    {
        public GetIncidentByIdTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetIncidentById_WithDevice_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var devices = await IncidentGateWayApi.GetSiteDevices();
            Assert.True(devices.Length > 0);

            var deviceId = devices[0].Devices[0].DeviceId;
            var deviceType = devices[0].Devices[0].DeviceType;
            var queryIncidentDetailsRequestGto = new GetIncidentDetailsRequestGto
                { IncidentId = incidentId, DeviceId = deviceId, DeviceType = deviceType };
            var incidentDetails = await IncidentGateWayApi.GetIncidentById(queryIncidentDetailsRequestGto);
            Assert.True(incidentDetails.Status == ExecuteStatus.Successful);
            Assert.Equal(incidentDetails.Id.ToString(), incidentId);
            Assert.Equal(incidentDetails.DeviceDisplayName, devices[0].Devices[0].DeviceDisplayName);
            Assert.Equal(incidentDetails.DeviceLocation, devices[0].Devices[0].DeviceLocation);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
