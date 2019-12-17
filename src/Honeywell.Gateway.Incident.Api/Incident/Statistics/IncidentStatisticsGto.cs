
namespace Honeywell.Gateway.Incident.Api.Incident.Statistics
{
    public class IncidentStatisticsGto
    {
        public string DeviceId { get; set; }

        public int UnAssignedCount { get; set; }

        public int ActiveCount { get; set; }

        public int CompletedCount { get; set; }

        public int CloseCount { get; set; }
    }
}
