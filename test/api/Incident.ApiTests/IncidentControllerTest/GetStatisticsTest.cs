using System;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetStatisticsTest : BaseIncIdentControllerTest
    {
        public GetStatisticsTest(DIFixture dIFixture) : base(dIFixture)
        {
        }

        [Fact]
        public async void GetStatistics_Success()
        {
            var deviceId = Guid.NewGuid().ToString();
            var deviceType = "Door";

            //create 1 completeIncident 
            await ImportWorkflowDesign();
            var completeIncidentId = CreateIncident(deviceId, deviceType).Result;

            var respondCompleteResult = await IncidentGateWayApi.RespondAsync(completeIncidentId);
            Assert.True(respondCompleteResult.IsSuccess);

            var completeResult = await IncidentGateWayApi.CompleteAsync(completeIncidentId);
            Assert.True(completeResult.IsSuccess);


            //create 1 closeIncident 
            var closeIncidentId = CreateIncident(deviceId, deviceType).Result;

            var respondCloseResult = await IncidentGateWayApi.RespondAsync(closeIncidentId);
            Assert.True(respondCloseResult.IsSuccess);

            var closeResult = await IncidentGateWayApi.CloseAsync(closeIncidentId,"No reason");
            Assert.True(closeResult.IsSuccess);


            // create 2 active incident
            var activitIncidentId1 = CreateIncident(deviceId, deviceType).Result;
            var activitIncidentId2 = CreateIncident(deviceId, deviceType).Result;

            var result = await IncidentGateWayApi.GetStatisticsAsync(deviceId);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(2,result.Value.ActiveCount);
            Assert.Equal(1, result.Value.CloseCount);
            Assert.Equal(1, result.Value.CompletedCount);

            await DeleteWorkflowDesign();
        }
    }
}
