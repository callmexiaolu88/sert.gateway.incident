using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class UpdateWorkflowStepStatusTest: BaseIncIdentControllerTest
    {
        public UpdateWorkflowStepStatusTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void UpdateWorkflowStepStatus_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var incidentDetails = await IncidentGateWayApi.GetIncidentById(incidentId);
            var workflowStepId = incidentDetails.IncidentSteps[0].Id;
            var result = await IncidentGateWayApi.UpdateWorkflowStepStatus(workflowStepId.ToString(), true);
            Assert.True(result.Status == ExecuteStatus.Successful);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
