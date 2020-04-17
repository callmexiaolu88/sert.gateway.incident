using Honeywell.Infra.Services.LiveData.Api;

namespace Honeywell.GateWay.Incident.Repository.Incident.IncidentEvents
{
    public class IncidentList : EventData
    {
        public IncidentList()
        {
            EventGroup = $"{nameof(IncidentList)}";
        }
        public override string EventGroup { get; set; }
    }

}
