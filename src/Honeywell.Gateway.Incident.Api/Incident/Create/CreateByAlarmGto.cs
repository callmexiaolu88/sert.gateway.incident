﻿using Honeywell.Gateway.Incident.Api.Incident.Detail;
using System;

namespace Honeywell.Gateway.Incident.Api.Incident.Create
{
    public class CreateByAlarmGto
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
