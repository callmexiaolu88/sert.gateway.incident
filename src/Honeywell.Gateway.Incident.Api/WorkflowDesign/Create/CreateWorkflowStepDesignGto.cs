using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Create
{
    public class CreateWorkflowStepDesignGto
    {
        public CreateWorkflowStepDesignGto(bool isOptional, string instruction, string helpText)
        {
            IsOptional = isOptional;
            Instruction = instruction;
            HelpText = helpText;
        }
        public bool IsOptional { get; set; }
        public string Instruction { get; set; }
        public string HelpText { get; set; }
    }
}
