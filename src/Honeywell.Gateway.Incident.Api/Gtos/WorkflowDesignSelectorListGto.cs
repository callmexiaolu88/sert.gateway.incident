using System;
using System.Collections.Generic;
using System.Text;

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
