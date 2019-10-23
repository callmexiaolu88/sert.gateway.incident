using System;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class IncidentStatusInfoGto
    {
        public string AlarmId { get; set; }

        public Guid IncidentId { get; set; }

        public IncidentStatus Status { get; set; }
    }
}
