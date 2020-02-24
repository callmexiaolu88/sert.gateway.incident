using System;
using System.Linq;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class GetIncidentStatusWithAlarmIdTest: BaseIncIdentControllerTest
    {
        public GetIncidentStatusWithAlarmIdTest(DIFixture dIFixture) : base(dIFixture)
        {

        }

        [Fact]
        public async void GetIncidentStatusWithAlarmId_Success()
        {
            await ImportWorkflowDesign();
            var alarmId = Guid.NewGuid().ToString();

            var result = await CreateIncidentByAlarm(alarmId);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.IncidentAlarmInfos.Count > 0);
            Assert.Equal(alarmId, result.Value.IncidentAlarmInfos.First().AlarmId);
            var incidentId = result.Value.IncidentAlarmInfos.First().IncidentId;

         
            var incidentStatusResponse = await IncidentGateWayApi.GetStatusByAlarmAsync(new[] { alarmId });

            Assert.NotNull(incidentStatusResponse);
            Assert.True(incidentStatusResponse.IsSuccess);
            Assert.NotNull(incidentStatusResponse.Value);
            Assert.NotNull(incidentStatusResponse.Value);
            Assert.True(incidentStatusResponse.Value.Any());
            Assert.Equal(incidentId,incidentStatusResponse.Value.First().IncidentId);
            Assert.Equal(alarmId, incidentStatusResponse.Value.First().AlarmId);

            await DeleteIncident(incidentId.ToString());
            await DeleteWorkflowDesign();
        }
    }
}
