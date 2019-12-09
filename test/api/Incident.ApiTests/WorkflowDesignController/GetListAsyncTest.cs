using System.Linq;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetListAsyncTest: BaseIncIdentControllerTest
    {
        public GetListAsyncTest(DIFixture dIFixture) : base(dIFixture)
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


            int allWorkflowCount = workflowDesigns.Result.Length;
            var conditon = "Fire/Safety";
            workflowDesigns = GetAllWorkflowDesigns(conditon);
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            Assert.True(workflowDesigns.Result.Length <= allWorkflowCount);
            foreach (var workflow in workflowDesigns.Result)
            {
                Assert.True(workflow.Name.Contains(conditon) || workflow.Description.Contains(conditon));
            }

            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetAllWorkflowDesign_IsDescriptionEmptyIncluded()
        {
            await ImportWorkflowDesign();

            var conditon = "--";
            var workflowDesigns = GetAllWorkflowDesigns(conditon);

            Assert.NotNull(workflowDesigns);
            Assert.True(workflowDesigns.Result.Length > 0);
            Assert.NotNull(workflowDesigns.Result);
            foreach (var workflow in workflowDesigns.Result)
            {
                Assert.True(workflow.Name.Contains(conditon) || workflow.Description.Contains(conditon) ||
                            string.IsNullOrEmpty(workflow.Description));
            }

            await DeleteWorkflowDesign();
        }
    }
}
