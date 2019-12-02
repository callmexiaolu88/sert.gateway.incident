﻿using System;
using System.Collections.Generic;
using System.Text;
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

            var getListResponse = await IncidentGateWayApi.GetListByDeviceAsync(0,device.DeviceId);

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
    }
}