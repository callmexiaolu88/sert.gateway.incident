using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector
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
