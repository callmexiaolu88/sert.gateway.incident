namespace Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus
{
    public class UpdateWorkflowStepStatusGto
    {
        public string WorkflowStepId { get; set; }

        public bool IsHandled { get; set; }

        public string IncidentId { get; set; }
    }
}
