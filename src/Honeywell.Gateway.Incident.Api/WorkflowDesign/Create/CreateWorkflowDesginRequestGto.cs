using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Create
{
    public class CreateWorkflowDesginRequestGto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CreateWorkflowStepDesignGto> Steps { get; set; }
        public CreateWorkflowDesginRequestGto()
        {
            Steps = new List<CreateWorkflowStepDesignGto>();
        }
    }
}
