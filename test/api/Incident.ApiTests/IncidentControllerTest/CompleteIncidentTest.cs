﻿using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class CompleteIncidentTest : BaseIncIdentControllerTest
    {
        public CompleteIncidentTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void CompleteIncident_Success()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var respondResult = await IncidentGateWayApi.RespondAsync(incidentId);
            Assert.True(respondResult.IsSuccess);

            var completeResult = await IncidentGateWayApi.CompleteAsync(incidentId);
            Assert.True(completeResult.IsSuccess);

            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void CompleteIncident_Fail()
        {
            await ImportWorkflowDesign();
            var incidentId = CreateIncident().Result;

            var completeResult = await IncidentGateWayApi.CompleteAsync(incidentId);
            Assert.False(completeResult.IsSuccess);

            await DeleteWorkflowDesign();
        }
    }
}
