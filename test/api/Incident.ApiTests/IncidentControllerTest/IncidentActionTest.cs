using System;
using System.Collections.Generic;
using System.Text;
using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class IncidentActionTest : BaseIncIdentControllerTest
    {
        public IncidentActionTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void IncidentAction_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var takeoverResult = await IncidentGateWayApi.TakeoverIncident(incidentId);
            Assert.True(takeoverResult.Status == ExecuteStatus.Successful);

            var completeResult = await IncidentGateWayApi.CompleteIncident(incidentId);
            Assert.True(completeResult.Status == ExecuteStatus.Successful);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
