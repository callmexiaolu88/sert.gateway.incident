using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetWorkflowDesignSelectorsByNameTest : BaseIncIdentControllerTest
    {
        public GetWorkflowDesignSelectorsByNameTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetWorkflowDesignSelectorsByName()
        {
            //assign
            await ImportWorkflowDesign();

            //action
            var workflowDesigns = await IncidentGateWayApi.GetWorkflowDesignSelectorsByName(string.Empty);

            //assert
            Assert.NotNull(workflowDesigns[0].Name);
            Assert.NotNull(workflowDesigns[0].Id.ToString());
            Assert.NotNull(workflowDesigns[0].ReferenceId.ToString());
            Assert.True(workflowDesigns.Length > 0);

            //clear
            await DeleteWorkflowDesign();
        }
    }
}