using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetWorkflowDesignSelectorsTest : BaseIncIdentControllerTest
    {
        public GetWorkflowDesignSelectorsTest(DIFixture dIFixture) : base(dIFixture)
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
            Assert.True(workflowDesignSelectors.Status == ExecuteStatus.Successful);
            Assert.True(workflowDesignSelectors.List.Count== workflowDesigns.Result.Length);
            
            //clear
            await DeleteWorkflowDesign();
        }
    }
}