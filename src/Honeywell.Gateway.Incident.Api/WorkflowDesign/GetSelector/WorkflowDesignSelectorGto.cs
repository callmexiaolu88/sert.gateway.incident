using System;
using System.ComponentModel.DataAnnotations;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector
{
    public class WorkflowDesignSelectorGto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Guid ReferenceId { get; set; }
    }
}
