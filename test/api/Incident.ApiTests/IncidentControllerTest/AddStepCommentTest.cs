using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Gtos;
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

            var incidentDetails = await IncidentGateWayApi.GetIncidentById(incidentId);
            var workflowStepId = incidentDetails.IncidentSteps[0].Id;
            string commentRemark = Guid.NewGuid().ToString();
            var addStepCommentGto = new AddStepCommentGto()
            {
                WorkflowStepId = workflowStepId.ToString(),
                Comment = $"this is comment.|{commentRemark}"
            };
            var result = await IncidentGateWayApi.AddStepComment(addStepCommentGto);
            Assert.True(result.Status == ExecuteStatus.Successful);

            var incidentDetailReponse = await IncidentGateWayApi.GetIncidentById(incidentId);
            bool isAddCommentSucess = incidentDetailReponse.IncidentSteps.First(o => o.Id == workflowStepId)
                .StepComments.Any(x => x.Description.Contains(commentRemark));
            Assert.True(isAddCommentSucess);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
