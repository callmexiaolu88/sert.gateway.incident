using System;
using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class IncidentGto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public long Number { get; set; }

        public DateTime RaisedAtUtc { get; set; }

        public DateTime? ClosedAtUtc { get; set; }

        public string Owner { get; set; }

        public byte Priority { get; set; }

        public byte State { get; set; }

        public string WorkflowName { get; set; }

        public string WorkflowDescription { get; set; }

        public string WorkflowOwner { get; set; }

        public List<IncidentStepGto> IncidentSteps { get; set; }
    }
}
