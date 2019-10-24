using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos.Status
{
    public class GetIncidentStatusResponseGto
    {
        public IList<IncidentStatusInfoGto> IncidentStatusInfos { get; set; }
        public GetIncidentStatusResponseGto()
        {
            IncidentStatusInfos = new List<IncidentStatusInfoGto>();
        }
    }
}
