using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class DownloadTemplateAsyncTest: BaseIncIdentControllerTest
    {
        public DownloadTemplateAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {
        }

        [Fact]
        public async void DownloadWorkflowTemplate_Success()
        {
            var response = await HttpClient.PostAsync($"/api/WorkflowDesign/DownloadTemplate", null);
            var resultData = await response.Content.ReadAsStreamAsync();

            Assert.True(resultData.Length > 100);
        }
    }
}
