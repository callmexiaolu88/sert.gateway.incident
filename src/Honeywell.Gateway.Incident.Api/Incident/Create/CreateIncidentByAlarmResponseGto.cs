using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Incident.Create
{
    public class CreateIncidentByAlarmResponseGto
    {
        public static readonly string MessageCodeAlarmDuplication = "duplicate-creation";

        public static readonly string MessageCodeWorkflowNotExist = "workflow-not-exist";

        public IList<IncidentAlarmGto> IncidentAlarmInfos { get; set; }

        public CreateIncidentByAlarmResponseGto()
        {
            IncidentAlarmInfos = new List<IncidentAlarmGto>();
        }
    }
}
