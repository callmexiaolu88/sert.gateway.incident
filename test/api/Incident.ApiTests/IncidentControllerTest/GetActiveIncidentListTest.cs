using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Gtos;
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
        public async void GetActiveIncidentList_GetData_Success()
        {
            //assign
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            //action
            var activeIncidentList = await IncidentGateWayApi.GetListAsync();

            //assert
            Assert.True(activeIncidentList.Status == ExecuteStatus.Successful);
            Assert.NotNull(activeIncidentList.List.FirstOrDefault(x => x.Id == Guid.Parse(incidentId)));

            //clear
            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

    }
}
