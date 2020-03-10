using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class ExportsAsyncTest : BaseIncIdentControllerTest
    {
        public ExportsAsyncTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void ExportWorkflowDesigns_Success()
        {
            await ImportWorkflowDesign();
            var workflowDesigns = GetAllWorkflowDesigns();

            HttpContent content = new StringContent(JsonConvert.SerializeObject(workflowDesigns.Result.Select(m => m.Id.ToString())
                .ToArray()), Encoding.UTF8, "application/json");

            var response =
                await HttpClient.PostAsync($"/api/WorkflowDesign/Exports", content);
            var resultData = await response.Content.ReadAsStreamAsync();
            Assert.True(resultData.Length > 100);

            await DeleteWorkflowDesign();
        }
    }
}
