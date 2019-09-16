using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
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
