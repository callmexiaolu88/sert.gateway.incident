using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Infra.Api.Abstract;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class CreateAsyncTest : BaseIncIdentControllerTest
    {
        public CreateAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void CreateWorkFlowDesign_Success()
        {
            var result = CreateWorkflowDesign();

            Assert.True(result.Result.IsSuccess);

            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void CreateWorkFlowDesign_NoSteps_Failed()
        {
            var mockCreateWorkflowDesignRequestGto = new CreateWorkflowDesignRequestGto
            {
                Description = "This procedure shall be completed 48hours before any events.",
                Name = "Event Prep Master 1",
            };

            var result = await WorkflowDesignGateWayApi.CreateAsync(mockCreateWorkflowDesignRequestGto);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async void CreateWorkFlowDesign_NameDuplicate_Failed()
        {

            await CreateWorkflowDesign();
            var result = CreateWorkflowDesign();
        
            Assert.False(result.Result.IsSuccess);

            await DeleteWorkflowDesign();
        }


        public async Task<ApiResponse> CreateWorkflowDesign()
        {
            var mockCreateWorkflowDesignRequestGto = new CreateWorkflowDesignRequestGto
            {
                Description = "This procedure shall be completed 48hours before any events.",
                Name= "Event Prep Master 1",
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
