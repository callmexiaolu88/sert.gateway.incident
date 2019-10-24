using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class TakeoverIncidentTest : BaseIncIdentControllerTest
    {
        public TakeoverIncidentTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void TakeoverIncident_Fail()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var takeoverResult = await IncidentGateWayApi.TakeoverIncident(incidentId);
            Assert.False(takeoverResult.Status == ExecuteStatus.Successful);

            await DeleteWorkflowDesign();
        }

    }
}
