using System;
using System.Runtime.Serialization;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowStepDesignGto
    {
        [DataMember(IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsOptional { get; set; }

        [DataMember(IsRequired = true)]
        public string Instruction { get; set; }

        [DataMember(IsRequired = true)]
        public string HelpText { get; set; }
    }
}
