using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
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
            var workflowDesignGto = await WorkflowDesignGateWayApi.GetByIdAsync(workflowDesignId);
            Assert.True(workflowDesignGto.IsSuccess);
            Assert.NotNull(workflowDesignGto.Value);
            Assert.NotNull(workflowDesignGto.Value.Name);
            Assert.NotNull(workflowDesignGto.Value.Description);
            Assert.NotNull(workflowDesignGto.Value.Steps);
            Assert.True(workflowDesignGto.Value.Steps.Length > 0);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void GetWorkflowDetails()
        {
            await ImportWorkflowDesign();
            var workflowDesignId = GetFirstWorkflowDesignId();
            var request = new GetDetailsRequestGto
            {
                Ids = new[] {new Guid(workflowDesignId)}
            };
            var workflowDesigns = await WorkflowDesignGateWayApi.GetDetailsAsync(request);

            Assert.NotNull(workflowDesigns);
            Assert.True(workflowDesigns.IsSuccess);
            Assert.NotNull(workflowDesigns.Value);
            Assert.NotNull(workflowDesigns.Value.WorkflowDesigns);
            Assert.True(workflowDesigns.Value.WorkflowDesigns.Any());
            await DeleteWorkflowDesign();
        }
    }
}
