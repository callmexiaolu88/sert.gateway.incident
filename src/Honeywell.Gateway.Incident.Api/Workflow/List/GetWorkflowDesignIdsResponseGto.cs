using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Workflow.List;

namespace Honeywell.Gateway.Incident.Api.Workflow.List
{
    public class GetWorkflowDesignIdsResponseGto
    {
        public IList<WorkflowDesignIdGto> WorkflowDesignIds { get; set; }
        public GetWorkflowDesignIdsResponseGto()
        {
            WorkflowDesignIds = new List<WorkflowDesignIdGto>();
        }
    }
}
