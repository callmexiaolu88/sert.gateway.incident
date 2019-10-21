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
    }
}
