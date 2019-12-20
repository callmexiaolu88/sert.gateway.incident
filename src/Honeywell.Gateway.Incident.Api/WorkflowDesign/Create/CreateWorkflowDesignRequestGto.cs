using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Create
{
    public class CreateWorkflowDesignRequestGto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateWorkflowStepDesignGto> Steps { get; set; }
        public CreateWorkflowDesignRequestGto()
        {
            Steps = new List<CreateWorkflowStepDesignGto>();
        }
    }
}
