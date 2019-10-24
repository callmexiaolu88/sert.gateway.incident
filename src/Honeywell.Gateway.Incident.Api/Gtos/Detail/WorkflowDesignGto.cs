using System;
using System.ComponentModel.DataAnnotations;

namespace Honeywell.Gateway.Incident.Api.Gtos.Detail
{
    public class WorkflowDesignGto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public WorkflowStepDesignGto[] Steps { get; set; }
    }
}
