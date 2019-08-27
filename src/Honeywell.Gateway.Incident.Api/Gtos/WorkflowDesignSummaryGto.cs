using System;
using System.ComponentModel.DataAnnotations;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowDesignSummaryGto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
