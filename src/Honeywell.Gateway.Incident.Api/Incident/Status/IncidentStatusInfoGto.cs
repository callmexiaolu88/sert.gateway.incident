using System;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api.Incident.Status
{
    public class IncidentStatusInfoGto
    {
        public string AlarmId { get; set; }

        public Guid IncidentId { get; set; }

        public IncidentStatus Status { get; set; }
    }
}
