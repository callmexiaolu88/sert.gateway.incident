using System;
using System.Collections.Generic;
using Honeywell.Micro.Services.Incident.Domain.Shared;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class IncidentGto : ExecuteResult
    {
        public IncidentGto()
        {
            Status = ExecuteStatus.Error;
            IncidentSteps = new List<IncidentStepGto>();
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public long Number { get; set; }

        public DateTime? CreateAtUtc { get; set; }

        public DateTime? LastUpdateAtUtc { get; set; }

        public string Owner { get; set; }

        public IncidentPriority Priority { get; set; }

        public IncidentState State { get; set; }

        public string WorkflowId { get; set; }

        public string WorkflowName { get; set; }

        public string WorkflowDescription { get; set; }

        public string WorkflowOwner { get; set; }

        public List<IncidentStepGto> IncidentSteps { get; set; }

        public List<ActivityGto> IncidentActivities { get; set; }

        public List<WorkflowActivitiesGto> WorkflowActivities { get; set; }

        public string DeviceDisplayName { get; set; }

        public string DeviceLocation { get; set; }

    }
}
