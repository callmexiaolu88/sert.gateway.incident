using System;
using System.ComponentModel.DataAnnotations;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Update
{
    public class UpdateWorkflowDesignResponseGto
    {
        public Guid Id { get; set; }

        public string WorkflowDesignName { get; set; }
    }
}
