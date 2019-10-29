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
            
            var devices = await IncidentGateWayApi.GetSiteDevicesAsync();
            Assert.NotNull(devices);
            Assert.True(devices.Length > 0);

            var device = devices[0].Devices[0];
            Assert.NotNull(device);

            var incidentId = await CreateIncident(device.DeviceId, device.DeviceType);

            var incidentDetails = await IncidentGateWayApi.GetDetailAsync(incidentId);
            Assert.True(incidentDetails.Status == ExecuteStatus.Successful);
            Assert.Equal(incidentDetails.Id.ToString(), incidentId);
            Assert.Equal(incidentDetails.DeviceDisplayName, device.DeviceDisplayName);
            Assert.Equal(incidentDetails.DeviceLocation, device.DeviceLocation);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
