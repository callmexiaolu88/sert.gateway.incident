using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Incident.Status
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
