﻿using System;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class IncidentStepGto
    {
        public Guid Id { get; set; }
        public bool IsComplete { get; set; }
        public bool IsOptional { get; set; }
        public string Instruction { get; set; }
        public string HelpText { get; set; }
    }
}
