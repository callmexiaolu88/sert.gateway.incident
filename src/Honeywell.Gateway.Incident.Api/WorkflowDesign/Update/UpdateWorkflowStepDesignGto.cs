
namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Update
{
    public class UpdateWorkflowStepDesignGto
    {
        public UpdateWorkflowStepDesignGto(bool isOptional, string instruction, string helpText)
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
