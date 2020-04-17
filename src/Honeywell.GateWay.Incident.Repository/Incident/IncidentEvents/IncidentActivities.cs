using System;
using Honeywell.Infra.Services.LiveData.Api;

namespace Honeywell.GateWay.Incident.Repository.Incident.IncidentEvents
{
    public sealed class IncidentActivities : EventData
    {
        public IncidentActivities(Guid incidentId)
        {
            EventGroup = $"{nameof(IncidentActivities)}|{incidentId}";
        }
        public  override string EventGroup { get; set; }
    }
}
