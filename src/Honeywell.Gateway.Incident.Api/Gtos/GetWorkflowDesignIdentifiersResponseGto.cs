using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class GetWorkflowDesignIdentifiersResponseGto
    {
        public IList<WorkflowDesignIdentifierGto> Identifiers { get; set; }
        public GetWorkflowDesignIdentifiersResponseGto()
        {
            Identifiers = new List<WorkflowDesignIdentifierGto>();
        }
    }
}
