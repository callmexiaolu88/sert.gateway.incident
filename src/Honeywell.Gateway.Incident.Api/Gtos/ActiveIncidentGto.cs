using System;
using System.Collections.Generic;
using Honeywell.Micro.Services.Incident.Domain.Shared;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class ActiveIncidentGto:ExecuteResult
    {
        public ActiveIncidentGto()
        {
            Status = ExecuteStatus.Error;
        }

        public Guid WorkflowId { get; set; }

        public string WorkflowDesignName { get; set; }

        public DateTime CreateAtUtc { get; set; }

        public string Owner { get; set; }

        public int CompletedSteps { get; set; }

        public int TotalSteps { get; set; }

        public IncidentPriority Priority { get; set; }

        public long Number { get; set; }

    }
}
