using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Gtos;

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
