using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetWorkflowDetailsTest: BaseIncIdentControllerTest
    {
        public GetWorkflowDetailsTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void GetWorkflowDesignById()
        {
            await ImportWorkflowDesign();
            var workflowDesignId = GetFirstWorkflowDesignId();
            var workflowDesignGto = await IncidentGateWayApi.GetWorkflowDesignById(workflowDesignId);
            Assert.NotNull(workflowDesignGto);
            Assert.NotNull(workflowDesignGto.Name);
            Assert.NotNull(workflowDesignGto.Description);
            Assert.NotNull(workflowDesignGto.Steps);
            Assert.True(workflowDesignGto.Steps.Length > 0);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetWorkflowDetails()
        {
            await ImportWorkflowDesign();
            var workflowDesignId = GetFirstWorkflowDesignId();
            var request = new GetWorkflowDesignsRequestGto
            {
                Ids = new[] {new Guid(workflowDesignId)}
            };
            var workflowDesigns = await WorkflowGateWayApi.GetDesignDetails(request);

            Assert.NotNull(workflowDesigns);
            Assert.True(workflowDesigns.IsSuccess);
            Assert.NotNull(workflowDesigns.Value);
            Assert.NotNull(workflowDesigns.Value.WorkflowDesigns);
            Assert.True(workflowDesigns.Value.WorkflowDesigns.Any());
            await DeleteWorkflowDesign();
        }
    }
}
