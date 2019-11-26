using System.Linq;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetSummariesAsyncTest: BaseIncIdentControllerTest
    {
        public GetSummariesAsyncTest(DIFixture dIFixture) : base(dIFixture)
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
