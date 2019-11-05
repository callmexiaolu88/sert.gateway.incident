using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
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
            Assert.NotNull(result.Value);
            var incidentId = result.Value.First();

         
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
