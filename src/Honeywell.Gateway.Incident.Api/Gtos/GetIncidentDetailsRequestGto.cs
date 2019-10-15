namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class GetIncidentDetailsRequestGto
    {
        public string IncidentId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
    }
}
