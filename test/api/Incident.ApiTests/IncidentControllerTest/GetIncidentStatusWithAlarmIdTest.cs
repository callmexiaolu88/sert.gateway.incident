using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Incident.Status;
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
            Assert.NotNull(result.Value.IncidentIds);
            var incidentId = result.Value.IncidentIds.First();

            var request = new GetStatusByAlarmRequestGto
            {
                AlarmIds = new[] {alarmId}
            };

            var incidentStatusResponse = await IncidentGateWayApi.GetStatusByAlarm(request);

            Assert.NotNull(incidentStatusResponse);
            Assert.True(incidentStatusResponse.IsSuccess);
            Assert.NotNull(incidentStatusResponse.Value);
            Assert.NotNull(incidentStatusResponse.Value.IncidentStatusInfos);
            Assert.True(incidentStatusResponse.Value.IncidentStatusInfos.Any());
            Assert.Equal(incidentId,incidentStatusResponse.Value.IncidentStatusInfos.First().IncidentId);
            Assert.Equal(alarmId, incidentStatusResponse.Value.IncidentStatusInfos.First().AlarmId);

            await DeleteIncident(incidentId.ToString());
            await DeleteWorkflowDesign();
        }
    }
}
