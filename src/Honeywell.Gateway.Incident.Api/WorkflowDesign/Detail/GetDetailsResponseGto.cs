using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail
{
    public class GetDetailsResponseGto
    {
        public IList<WorkflowDesignGto> WorkflowDesigns { get; set; }

        public GetDetailsResponseGto()
        {
            WorkflowDesigns = new List<WorkflowDesignGto>();
        }
    }
}
