﻿using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{

    public class DownloadTemplateAsyncTest
    {
        [Fact]
        public async void DownloadWorkflowTemplate_Success()
        {
            var httpClient = new HttpClientHelper();
            var response = await httpClient.PostAsync($"/api/WorkflowDesign/DownloadTemplate", null, httpClient.InitHeader());
            var resultData = await response.Content.ReadAsStreamAsync();
            
            Assert.True(resultData.Length > 100);
        }
    }
}