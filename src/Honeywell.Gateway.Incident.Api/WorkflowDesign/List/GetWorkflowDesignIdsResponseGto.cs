using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.List
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
