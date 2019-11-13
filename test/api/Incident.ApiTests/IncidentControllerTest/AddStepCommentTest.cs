using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class AddStepCommentTest : BaseIncIdentControllerTest
    {
        public AddStepCommentTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void AddStepComment_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var incidentDetails = await IncidentGateWayApi.GetDetailAsync(incidentId);
            var workflowStepId = incidentDetails.Value.IncidentSteps[0].Id;
            string commentRemark = Guid.NewGuid().ToString();
            var addStepCommentGto = new AddStepCommentRequestGto()
            {
                WorkflowStepId = workflowStepId.ToString(),
                Comment = $"this is comment.|{commentRemark}",
                IncidentId = incidentId
            };
            var result = await IncidentGateWayApi.AddStepCommentAsync(addStepCommentGto);
            Assert.True(result.IsSuccess);

            var incidentDetailResponse = await IncidentGateWayApi.GetDetailAsync(incidentId);
            bool isAddCommentSucess = incidentDetailResponse.Value.IncidentSteps.First(o => o.Id == workflowStepId)
                .StepComments.Any(x => x.Description.Contains(commentRemark));
            Assert.True(isAddCommentSucess);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
