﻿using System;
using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Incident.Create;

namespace Honeywell.Gateway.Incident.Api.Incident.GetDetail
{
    public class IncidentDetailGto 
    {
        public IncidentDetailGto()
        {
            IncidentSteps = new List<IncidentStepGto>();
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public long Number { get; set; }

        public DateTime? CreateAtUtc { get; set; }

        public DateTime? LastUpdateAtUtc { get; set; }

        public string Owner { get; set; }

        public IncidentPriority Priority { get; set; }

        public IncidentStatus State { get; set; }

        public Guid WorkflowId { get; set; }

        public string WorkflowName { get; set; }

        public string WorkflowDescription { get; set; }

        public string WorkflowOwner { get; set; }

        public List<IncidentStepGto> IncidentSteps { get; set; }

        public List<ActivityGto> IncidentActivities { get; set; }

        public string DeviceDisplayName { get; set; }

        public string DeviceLocation { get; set; }

        public string DeviceId { get; set; }

        public IncidentTriggerType TriggerType { get; set; }

        public AlarmData AlarmData { get; set; }

        public string TriggerId { get; set; }

        public string CameraNumber { get; set; }

        public long EventTimeStamp { get; set; }

    }
}
