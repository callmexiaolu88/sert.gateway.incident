using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Infra.Api.Abstract;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetIncidentListTest : BaseIncIdentControllerTest
    {
        public GetIncidentListTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void GetIncidentList_WithDevice_Success()
        {
            await ImportWorkflowDesign();

            var getDeviceResponse = await IncidentGateWayApi.GetSiteDevicesAsync();
            Assert.True(getDeviceResponse.IsSuccess);

            var devices = getDeviceResponse.Value;
            Assert.NotNull(devices);
            Assert.True(devices.Length > 0);

            var device = devices[0].Devices[0];
            Assert.NotNull(device);

            var incidentId1 = await CreateIncident(device.DeviceId, device.DeviceType);
            var incidentId2 =  await CreateIncident(device.DeviceId, device.DeviceType);
            var mockRequest = MockGetListRequestGto(0,device.DeviceId);

            var getListResponse = await IncidentGateWayApi.GetListAsync(mockRequest);

            Assert.True(getListResponse.IsSuccess);
            var list = getListResponse.Value;
            foreach (var incident in list)
            {
                Assert.Equal(device.DeviceId,incident.DeviceId);
            }
            await DeleteIncident(incidentId1);
            await DeleteIncident(incidentId2);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetActiveIncidentList_GetData_Success()
        {
            //assign
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;
            var request = new GetListRequestGto { State = 0 };
            //action
            var activeIncidentList = await IncidentGateWayApi.GetListAsync(new PageRequest().To(request));

            //assert
            Assert.True(activeIncidentList.IsSuccess);
            Assert.NotNull(activeIncidentList.Value.FirstOrDefault(x => x.Id == Guid.Parse(incidentId)));

            //clear
            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

        private PageRequest<GetListRequestGto> MockGetListRequestGto(int state,string deviceId)
        {
            var request = new GetListRequestGto
            {
                State = state,
                DeviceId = deviceId,
                HasOwner = false
            };
            var pageRequest = new PageRequest().To(request);
            return pageRequest;
        }

    }
}
