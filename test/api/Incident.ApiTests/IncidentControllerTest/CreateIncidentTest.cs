using System.Linq;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class CreateIncidentTest: BaseIncIdentControllerTest
    {
        public CreateIncidentTest(DIFixture dIFixture):base(dIFixture)
        {

        }
        [Fact]
        public async void CreateIncident_Success()
        {
            await ImportWorkflowDesign();

            var incidentId = await CreateIncident();

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void CreateIncidentByAlarm_Success()
        {
            await ImportWorkflowDesign();
            var result = await CreateIncidentByAlarm();
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value.IncidentIds);
            var incidentId = result.Value.IncidentIds.First();
            await DeleteIncident(incidentId.ToString());
            await DeleteWorkflowDesign();
        }
    }
}
