using System;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    [Collection(Colections.DICollection)]
    public class CreateIncidentTest: BaseIncIdentControllerTest
    {
        public CreateIncidentTest(DIFixture dIFixture):base(dIFixture)
        {

        }
        [Fact]
        public async void CreateIncident_Success()
        {
            await ImportWorkflowDesign();

            var incidentId = await CreateIncident();

            await DeleteIncident(incidentId);
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void CreateIncidentByAlarm_Success()
        {
            await ImportWorkflowDesign();
            var result = await CreateIncidentByAlarm();
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.IncidentAlarmInfos.Count > 0);
            var incidentId = result.Value.IncidentAlarmInfos.First().IncidentId;
            await DeleteIncident(incidentId.ToString());
            await DeleteWorkflowDesign();
        }

        [Fact]
        public async void CreateIncidentByAlarm_AlarmRepeat()
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

            var secondResult = await CreateIncidentByAlarm(alarmId);
            Assert.NotNull(secondResult);
            Assert.False(secondResult.IsSuccess);
            Assert.NotNull(secondResult.Value);
            Assert.True(secondResult.Value.IncidentAlarmInfos.Count > 0);
            Assert.Equal(alarmId, secondResult.Value.IncidentAlarmInfos.First().AlarmId);
            Assert.True(secondResult.Messages.Count > 0);
            Assert.Equal(CreateIncidentByAlarmResponseGto.MessageCodeAlarmDuplication,
                secondResult.Messages.First().MessageCode);

            await DeleteIncident(incidentId.ToString());
            await DeleteWorkflowDesign();
        }
    }
}
