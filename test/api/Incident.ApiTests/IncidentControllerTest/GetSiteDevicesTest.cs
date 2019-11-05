using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetSiteDevicesTest: BaseIncIdentControllerTest
    {
        public GetSiteDevicesTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetSiteDevices_Success()
        {
            var result = await IncidentGateWayApi.GetSiteDevicesAsync();
            Assert.NotNull(result);
        }
    }
}
