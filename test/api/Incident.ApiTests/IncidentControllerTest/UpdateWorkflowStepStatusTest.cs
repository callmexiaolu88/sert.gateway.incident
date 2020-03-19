using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
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

            var incidentDetails = await IncidentGateWayApi.GetDetailAsync(incidentId);
            var workflowStepId = incidentDetails.Value.IncidentSteps[0].Id;
            var updateWorkflowStepStatusGto = new UpdateStepStatusRequestGto
            {
                WorkflowId = incidentDetails.Value.WorkflowId.ToString(),
                IsHandled = true,
                WorkflowStepId = workflowStepId.ToString()
            };
            var result =await IncidentGateWayApi.UpdateStepStatusAsync(updateWorkflowStepStatusGto);
            Assert.True(result.IsSuccess);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
