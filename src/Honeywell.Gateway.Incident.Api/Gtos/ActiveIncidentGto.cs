using System;
using Honeywell.Micro.Services.Incident.Domain.Shared;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class ActiveIncidentGto
    {
        public int SequenceId { get; set; }

        public Guid WorkflowId { get; set; }

        public string WorkflowDesignName { get; set; }

        public string Owner { get; set; }

        public DateTime CreateAtUtc { get; set; }

        public int TotalSteps { get; set; }

        public int CompletedSteps { get; set; }
    }
}
