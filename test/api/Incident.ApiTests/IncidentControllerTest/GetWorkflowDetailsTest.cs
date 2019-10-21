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
        public async void GetWorkflowDetails()
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
    }
}
