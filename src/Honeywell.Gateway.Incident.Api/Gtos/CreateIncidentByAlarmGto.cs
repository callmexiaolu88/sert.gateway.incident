using System;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class CreateIncidentByAlarmGto
    {
        public Guid WorkflowDesignReferenceId { get; set; }

        public IncidentPriority Priority { get; set; }

        public string Description { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }

        public string AlarmId { get; set; }

        public AlarmData AlarmData { get; set; }
    }

    public class AlarmData
    {
        public string AlarmType { get; set; }
        public string Description { get; set; }
    }
}
