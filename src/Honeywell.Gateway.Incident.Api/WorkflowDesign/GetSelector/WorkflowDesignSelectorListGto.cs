using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector
{
    public class WorkflowDesignSelectorListGto
    {
        public IList<WorkflowDesignSelectorGto> List { get; set; }

        public WorkflowDesignSelectorListGto()
        {
            List = new List<WorkflowDesignSelectorGto>();
        }
    }
}
