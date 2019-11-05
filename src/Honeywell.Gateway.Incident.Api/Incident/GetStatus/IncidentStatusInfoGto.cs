using System;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;

namespace Honeywell.Gateway.Incident.Api.Incident.GetStatus
{
    public class IncidentStatusInfoGto
    {
        public string AlarmId { get; set; }

        public Guid IncidentId { get; set; }

        public IncidentStatus Status { get; set; }
    }
}
