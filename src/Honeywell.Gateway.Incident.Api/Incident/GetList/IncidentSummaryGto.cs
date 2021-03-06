﻿using System;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;

namespace Honeywell.Gateway.Incident.Api.Incident.GetList
{
    public class IncidentSummaryGto
    {
        public Guid Id { get; set; }

        public Guid WorkflowId { get; set; }

        public string WorkflowDesignName { get; set; }

        public DateTime CreateAtUtc { get; set; }

        public string Owner { get; set; }

        public int CompletedSteps { get; set; }

        public int TotalSteps { get; set; }

        public IncidentPriority Priority { get; set; }

        public long Number { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }
    }
}
