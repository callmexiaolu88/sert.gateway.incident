using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Workflow.List;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class GetWorkflowDesignIdentifiersResponseGto
    {
        public IList<WorkflowDesignIdGto> Identifiers { get; set; }
        public GetWorkflowDesignIdentifiersResponseGto()
        {
            Identifiers = new List<WorkflowDesignIdGto>();
        }
    }
}
