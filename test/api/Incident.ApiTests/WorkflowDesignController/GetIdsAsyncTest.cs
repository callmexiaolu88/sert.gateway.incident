using System.Linq;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetIdsAsyncTest : BaseIncIdentControllerTest
    {
        public GetIdsAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {

        }


        [Fact]
        public async void GetWorkflowDesignIds_Success()
        {
            await ImportWorkflowDesign();
            var workflowDesigns = await WorkflowDesignGateWayApi.GetIdsAsync();
            Assert.NotNull(workflowDesigns);
            Assert.True(workflowDesigns.IsSuccess);
            Assert.NotNull(workflowDesigns.Value);
            Assert.NotNull(workflowDesigns.Value);
            Assert.True(workflowDesigns.Value.Any());
            await DeleteWorkflowDesign();
        }
    }
}
