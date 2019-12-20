using System;
using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Update
{
    public class UpdateWorkflowDesignRequestGto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UpdateWorkflowStepDesignGto> Steps { get; set; }
        public UpdateWorkflowDesignRequestGto()
        {
            Steps = new List<UpdateWorkflowStepDesignGto>();
        }
    }
}
