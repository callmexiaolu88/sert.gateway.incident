using System;
using Honeywell.Infra.Services.LiveData.Api;

namespace Honeywell.GateWay.Incident.Repository.Incident.IncidentEvents
{
    public class IncidentActivities : EventData
    {
        public IncidentActivities(Guid incidentId)
        {
            EventGroup = $"{nameof(IncidentActivities)}|{incidentId}";
        }
    }
}
