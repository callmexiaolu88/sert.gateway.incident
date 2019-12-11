using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
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
            var request = new GetListRequestGto {CurrentPage = 1, PageSize = 500};
            //action
            var activeIncidentList = await IncidentGateWayApi.GetListAsync(request);

            //assert
            Assert.True(activeIncidentList.IsSuccess);
            Assert.NotNull(activeIncidentList.Value.FirstOrDefault(x => x.Id == Guid.Parse(incidentId)));

            //clear
            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

    }
}
