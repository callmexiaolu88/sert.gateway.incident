using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{

    public class DownloadWorkflowTemplateTest
    {
        [Fact]
        public async void DownloadWorkflowTemplate_Success()
        {
            var httpClient = new HttpClientHelper();
            var response = await httpClient.PostAsync($"/api/Incident/DownloadWorkflowTemplate", null, httpClient.InitHeader());
            var resultData = await response.Content.ReadAsStreamAsync();
            
            Assert.True(resultData.Length > 100);
        }
    }
}
