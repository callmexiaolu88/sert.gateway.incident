using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class CompleteIncidentTest : BaseIncIdentControllerTest
    {
        public CompleteIncidentTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void CompleteIncident_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var respondResult = await IncidentGateWayApi.RespondAsync(incidentId);
            Assert.True(respondResult.Status == ExecuteStatus.Successful);

            var completeResult = await IncidentGateWayApi.CompleteAsync(incidentId);
            Assert.True(completeResult.Status == ExecuteStatus.Successful);

            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void CompleteIncident_Fail()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var completeResult = await IncidentGateWayApi.CompleteAsync(incidentId);
            Assert.False(completeResult.Status == ExecuteStatus.Successful);

            await DeleteWorkflowDesign();
        }
    }
}
