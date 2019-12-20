using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Update;
using Honeywell.Infra.Api.Abstract;
using Incident.ApiTests.IncidentControllerTest;
using Xunit;

namespace Incident.ApiTests.WorkflowDesignController
{
    [Collection(Colections.DICollection)]
    public class UpdateAsyncTest : BaseIncIdentControllerTest
    {
        public UpdateAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void UpdateWorkFlowDesign_Success()
        {
            await CreateWorkflowDesign();
            var response = await WorkflowDesignGateWayApi.GetListAsync(string.Empty);

            var updateRequestGto = GetUpdateWorkflowDesign();
            updateRequestGto.Id = response.Value[0].Id;

            var result = await WorkflowDesignGateWayApi.UpdateAsync(updateRequestGto);

            Assert.True(result.IsSuccess);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void UpdateWorkFlowDesign_NoSteps_Failed()
        {
            await CreateWorkflowDesign();
            var response = await WorkflowDesignGateWayApi.GetListAsync(string.Empty);

            var updateRequestGto = GetUpdateWorkflowDesign();
            updateRequestGto.Steps.Clear();

            var result = await WorkflowDesignGateWayApi.UpdateAsync(updateRequestGto);

            Assert.False(result.IsSuccess);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void UpdateWorkFlowDesign_CannotUpdateName_Failed()
        {
            await CreateWorkflowDesign();
            var response = await WorkflowDesignGateWayApi.GetListAsync(string.Empty);

            var updateRequestGto = GetUpdateWorkflowDesign();
            updateRequestGto.Name = "update name";

            var result = await WorkflowDesignGateWayApi.UpdateAsync(updateRequestGto);

            Assert.False(result.IsSuccess);
            await DeleteWorkflowDesign();
        }


        private UpdateWorkflowDesignRequestGto GetUpdateWorkflowDesign()
        {
            return new UpdateWorkflowDesignRequestGto
            {
                Description = "This procedure shall be completed 48hours before any events.",
                Name = "Event Prep Master 1",
                Steps =
                {
                    new UpdateWorkflowStepDesignGto(false,
                        "Confirm with event manager on final event requirements and special needs",
                        "View event booking calendar.On the calendar, link to event contract"),
                    new UpdateWorkflowStepDesignGto(true,
                        "If required by the event, engage Energy team for pre-event check.",
                        "Any cabling on special electricity supply ? Backup generators ready ? ")
                }
            };
        }

        private async Task<ApiResponse> CreateWorkflowDesign()
        {
            var mockCreateWorkflowDesignRequestGto = new CreateWorkflowDesignRequestGto
            {
                Description = "This procedure shall be completed 48hours before any events.",
                Name = "Event Prep Master 1",
                Steps =
                {
                    new CreateWorkflowStepDesignGto(false,
                        "Confirm with event manager on final event requirements and special needs",
                        "View event booking calendar.On the calendar, link to event contract"),
                    new CreateWorkflowStepDesignGto(true,
                        "If required by the event, engage Energy team for pre-event check.",
                        "Any cabling on special electricity supply ? Backup generators ready ? ")
                }
            };
            return await WorkflowDesignGateWayApi.CreateAsync(mockCreateWorkflowDesignRequestGto);
        }
    }
}