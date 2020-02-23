using System;

namespace Honeywell.Gateway.Incident.Api.Incident.Create
{
    public class IncidentAlarmGto
    {
        public Guid IncidentId { get; set; }

        public string AlarmId { get; set; }

        public bool IsCreatedAtThisRequest { get; set; }
    }
}
