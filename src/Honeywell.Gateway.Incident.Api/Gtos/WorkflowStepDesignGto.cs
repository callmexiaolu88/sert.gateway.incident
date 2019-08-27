using System;
using System.ComponentModel.DataAnnotations;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowStepDesignGto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool IsOptional { get; set; }

        [Required]
        public string Instruction { get; set; }

        [Required]
        public string HelpText { get; set; }
    }
}
