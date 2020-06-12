using System;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetDeviceListTest : BaseIncIdentControllerTest
    {
        public GetDeviceListTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void GetDeviceListTest_Success()
        {
            var result = await IncidentGateWayApi.GetDeviceListAsync(new GetDeviceListRequestGto { DeviceName = string.Empty,SiteId = string.Empty});
            Assert.NotNull(result);
        }
    }
}
