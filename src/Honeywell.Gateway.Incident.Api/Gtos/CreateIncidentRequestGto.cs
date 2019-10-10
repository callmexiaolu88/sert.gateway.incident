namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class CreateIncidentRequestGto
    {
        public string WorkflowDesignReferenceId { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }
    }
}
