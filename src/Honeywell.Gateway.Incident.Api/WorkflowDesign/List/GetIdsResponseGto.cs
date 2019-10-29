using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.List
{
    public class GetIdsResponseGto
    {
        public IList<WorkflowDesignIdGto> WorkflowDesignIds { get; set; }
        public GetIdsResponseGto()
        {
            WorkflowDesignIds = new List<WorkflowDesignIdGto>();
        }
    }
}
