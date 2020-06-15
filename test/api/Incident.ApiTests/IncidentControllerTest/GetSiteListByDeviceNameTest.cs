using System;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetSiteListByDeviceNameTest : BaseIncIdentControllerTest
    {
        public GetSiteListByDeviceNameTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void GetSiteListByDeviceName_Success()
        {
            var result = await IncidentGateWayApi.GetSiteListByDeviceNameAsync(string.Empty);
        
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Length > 0);
        }
    }
}
