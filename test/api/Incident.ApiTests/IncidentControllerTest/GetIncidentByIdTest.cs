using System;
using System.Linq;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetIncidentByIdTest : BaseIncIdentControllerTest
    {
        public GetIncidentByIdTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetIncidentById_WithDevice_Success()
        {
            await ImportWorkflowDesign();

            var getDeviceResponse = await IncidentGateWayApi.GetSiteDevicesAsync();
            Assert.True(getDeviceResponse.IsSuccess);

            var devices = getDeviceResponse.Value;
            Assert.NotNull(devices);
            Assert.True(devices.Length > 0);

            var device = devices[0].Devices[0];
            Assert.NotNull(device);

            var incidentId = await CreateIncident(device.DeviceId, device.DeviceType);

            var getDetailResponse = await IncidentGateWayApi.GetDetailAsync(incidentId);

            Assert.True(getDetailResponse.IsSuccess);
            var detail = getDetailResponse.Value;
            Assert.Equal(detail.Id.ToString(), incidentId);
            Assert.Equal(detail.DeviceDisplayName, device.DeviceDisplayName);
            Assert.Equal(detail.DeviceLocation, device.DeviceLocation);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetIncidentById_WithCamera_Success()
        {
            await ImportWorkflowDesign();

            var getDeviceResponse = await IncidentGateWayApi.GetSiteDevicesAsync();
            Assert.True(getDeviceResponse.IsSuccess);

            var devices = getDeviceResponse.Value;
            Assert.NotNull(devices);
            Assert.True(devices.Length > 0);

            var device = devices[0].Devices.FirstOrDefault(m => m.DeviceDisplayName.EndsWith("_camera"));
            Assert.NotNull(device);

            var incidentId = await CreateIncident(device.DeviceId, device.DeviceType);

            var getDetailResponse = await IncidentGateWayApi.GetDetailAsync(incidentId);



            Assert.True(getDetailResponse.IsSuccess);
            var detail = getDetailResponse.Value;
            Assert.Equal(detail.Id.ToString(), incidentId);
            Assert.Equal(detail.TriggerId, device.DeviceId);
            Assert.Equal(detail.DeviceDisplayName, device.DeviceDisplayName);
            Assert.Equal(detail.DeviceLocation, device.DeviceLocation);
            Assert.False(string.IsNullOrEmpty(detail.CameraNumber));
            Assert.NotNull(detail.CreateAtUtc);
            Assert.False(detail.CreateAtUtc.Value.Kind == DateTimeKind.Utc);
            var utcCreateDateTime = DateTime.SpecifyKind(detail.CreateAtUtc.Value, DateTimeKind.Utc);
            Assert.Equal(detail.EventTimeStamp, new DateTimeOffset(utcCreateDateTime).ToUnixTimeMilliseconds());

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
