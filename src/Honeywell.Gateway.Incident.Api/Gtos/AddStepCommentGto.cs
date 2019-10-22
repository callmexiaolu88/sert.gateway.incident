using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class AddStepCommentGto
    {
        public string WorkflowStepId { get; set; }

        public string Comment { get; set; }
    }
}
