using System;
using System.ComponentModel.DataAnnotations;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Create
{
    public class CreateWorkflowDesignResponseGto
    {
        public Guid Id { get; set; }

        public string WorkflowDesignName { get; set; }
    }
}
