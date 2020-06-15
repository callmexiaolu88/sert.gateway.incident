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
            var getSiteResponse = await IncidentGateWayApi.GetSiteListByDeviceNameAsync(string.Empty);
            Assert.True(getSiteResponse.IsSuccess);

            var sites = getSiteResponse.Value;
            Assert.NotNull(sites);
            Assert.True(sites.Length > 0);

            var site = sites[0];
            Assert.NotNull(site);

            var result = await IncidentGateWayApi.GetDeviceListAsync(new GetDeviceListRequestGto { DeviceName = string.Empty,SiteId = site.SiteId });

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Length>0);

        }
    }
}
