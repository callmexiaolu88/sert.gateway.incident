using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetSelectorsAsyncTest : BaseIncIdentControllerTest
    {
        public GetSelectorsAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetWorkflowDesignSelectors_GetData_Success()
        {
            //assign
            await ImportWorkflowDesign();
            var workflowDesigns = GetAllWorkflowDesigns();

            //action
            var workflowDesignSelectors = await WorkflowDesignGateWayApi.GetSelectorsAsync();

            //assert
            Assert.True(workflowDesignSelectors.IsSuccess);
            Assert.True(workflowDesignSelectors.Value.Length == workflowDesigns.Result.Length);
            
            //clear
            await DeleteWorkflowDesign();
        }
    }
}