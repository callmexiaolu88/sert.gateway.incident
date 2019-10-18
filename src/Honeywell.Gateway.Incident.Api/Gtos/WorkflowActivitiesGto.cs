using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowActivitiesGto
    {
        public Guid? WorkflowStepId { get; set; }
        public int Type { get; set; }
        public DateTime AtUtc { get; set; }
        public string Operator { get; set; }
        public string Description { get; set; }
    }
}
