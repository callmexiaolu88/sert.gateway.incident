using System;
using System.Linq;
using System.Threading;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
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

        [Fact]
        public async void GetIncidentStatusWithAlarmId_2of5NotFound_Failed()
        {
            //await DeleteWorkflowDesign();
            await ImportWorkflowDesign();
            var alarmIds = new[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
            };

            var result = await CreateIncidentsByAlarms(alarmIds.Take(3).ToArray());

            var incidentStatusResponse = await IncidentGateWayApi.GetStatusByAlarmAsync(alarmIds);

            Assert.NotNull(incidentStatusResponse);
            Assert.False(incidentStatusResponse.IsSuccess);
            Assert.NotNull(incidentStatusResponse.Value);
            Assert.True(incidentStatusResponse.Value.Length == 3);
            for (int i = 0; i < 3; i++)
            {
                Assert.Contains(incidentStatusResponse.Value, p => p.AlarmId == alarmIds[i]);
            }

            for (int i = 3; i < 5; i++)
            {
                Assert.Contains(
                    incidentStatusResponse.Messages,
                    msg =>
                        msg.MessageCode == GetIncidentStatusResponseDto.MessageCodeIncidentNotFound
                        && msg.Message.Contains(alarmIds[i]));
            }

            foreach (var incident in result.Value.IncidentAlarmInfos)
            {
                await DeleteIncident(incident.IncidentId.ToString());
            }

            await DeleteWorkflowDesign();
        }
    }
}
