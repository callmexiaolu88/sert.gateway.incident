using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowDesignSelectorListGto : ExecuteResult
    {
        public IList<WorkflowDesignSelectorGto> List { get; set; }

        public WorkflowDesignSelectorListGto()
        {
            List = new List<WorkflowDesignSelectorGto>();
        }
    }
}
