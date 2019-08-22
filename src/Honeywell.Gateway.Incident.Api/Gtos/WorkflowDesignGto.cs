﻿using System;
using System.Runtime.Serialization;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowDesignGto
    {
        [DataMember(IsRequired = true)]
        public Guid Id { get; set; }

        [DataMember(IsRequired = true)]
        public string Name { get; set; }

        [DataMember(IsRequired = true)]
        public string Description { get; set; }

        [DataMember(IsRequired = true)]
        public WorkflowStepDesignGto[] Steps { get; set; }
    }
}
