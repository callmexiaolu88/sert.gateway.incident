using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api.Workflow.Detail
{
    public class GetWorkflowDesignDetailsResponseGto
    {
        public IList<WorkflowDesignGto> WorkflowDesigns { get; set; }

        public GetWorkflowDesignDetailsResponseGto()
        {
            WorkflowDesigns = new List<WorkflowDesignGto>();
        }
    }
}
