using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class ImportAsyncTest: BaseIncIdentControllerTest
    {
        public ImportAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void ImportWorkFlowDesign_Success()
        {
            var result = ImportWorkflowDesign();
            Assert.True(result.Result.IsSuccess);
            await DeleteWorkflowDesign();
        }
    }
}
