using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Incident.GetStatus
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
