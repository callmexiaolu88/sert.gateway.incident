using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Incident.Status
{
    public class GetStatusByAlarmResponseGto
    {
        public IList<IncidentStatusInfoGto> IncidentStatusInfos { get; set; }
        public GetStatusByAlarmResponseGto()
        {
            IncidentStatusInfos = new List<IncidentStatusInfoGto>();
        }
    }
}
