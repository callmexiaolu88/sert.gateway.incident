using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetActiveIncidentListTest : BaseIncIdentControllerTest
    {
        public GetActiveIncidentListTest(DIFixture dIFixture) : base(dIFixture)
        {

        }
        [Fact]
        public async void GetActiveIncidentList()
        {
            //assign
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;
            var workflowDesignName = GetFirstWorkflowDesignName();

            //action
            var activeIncidentList = await IncidentGateWayApi.GetActiveIncidentList();

            //assert
            Assert.True(activeIncidentList.List[0].Id.ToString() == incidentId);
            Assert.True(activeIncidentList.List[0].WorkflowDesignName == workflowDesignName);
            Assert.True(activeIncidentList.List.Count > 0);

            //clear
            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

    }
}
