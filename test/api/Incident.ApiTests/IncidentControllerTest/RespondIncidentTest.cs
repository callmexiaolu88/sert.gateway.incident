using System;
using System.Collections.Generic;
using System.Text;
using Honeywell.Gateway.Incident.Api.Gtos;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class RespondIncidentTest : BaseIncIdentControllerTest
    {
        public RespondIncidentTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void RespondIncident_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var respondResult = await IncidentGateWayApi.RespondIncident(incidentId);
            Assert.True(respondResult.Status == ExecuteStatus.Successful);

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }
    }
}
