using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class CreateIncidentRequestGto
    {
        public string WorkflowDesignReferenceId { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
    }
}
