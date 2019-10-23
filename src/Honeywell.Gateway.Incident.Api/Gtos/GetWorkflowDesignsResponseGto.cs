﻿using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos
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
