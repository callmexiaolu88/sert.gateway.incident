using System.Runtime.Serialization;

namespace Honeywell.Gateway.Incident.Api
{
    public enum IncidentTriggerType
    {
        [EnumMember] Manual = 0,

        [EnumMember] Alarm = 1

    }
}
