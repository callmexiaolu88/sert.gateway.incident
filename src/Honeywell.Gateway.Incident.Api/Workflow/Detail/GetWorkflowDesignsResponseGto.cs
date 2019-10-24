using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api.Workflow.Detail
{
    public class GetWorkflowDesignsResponseGto
    {
        public IList<WorkflowDesignGto> WorkflowDesigns { get; set; }

        public GetWorkflowDesignsResponseGto()
        {
            WorkflowDesigns = new List<WorkflowDesignGto>();
        }
    }
}
