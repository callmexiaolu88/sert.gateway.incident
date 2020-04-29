namespace Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus
{
    public class UpdateStepStatusRequestGto
    {
        public string WorkflowStepId { get; set; }

        public bool IsHandled { get; set; }

        public string WorkflowId { get; set; }
    }
}
