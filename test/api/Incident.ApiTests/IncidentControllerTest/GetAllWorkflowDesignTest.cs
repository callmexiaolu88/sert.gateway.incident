using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetAllWorkflowDesignTest: BaseIncIdentControllerTest
    {
        public GetAllWorkflowDesignTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetAllWorkflowDesign()
        {
            await ImportWorkflowDesign();
            var workflowDesigns = GetAllWorkflowDesigns();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            Assert.True(workflowDesigns.Result.Length > 0);
            await DeleteWorkflowDesign();
        }
    }
}
