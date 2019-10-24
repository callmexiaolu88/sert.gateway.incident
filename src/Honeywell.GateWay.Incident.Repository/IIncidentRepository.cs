using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IIncidentRepository
    {
        Task<ExecuteResult> ImportWorkflowDesigns(Stream stream);

        Task<ExecuteResult> ValidatorWorkflowDesigns(Stream stream);

        Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors();

        Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadWorkflowTemplate();

        Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds);

        Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled);

        Task<IncidentGto> GetIncidentById(string incidentId);

        Task<string> CreateIncident(CreateIncidentRequestGto request);

        Task<ExecuteResult> RespondIncident(string incidentId);

        Task<ExecuteResult> TakeoverIncident(string incidentId);

        Task<ExecuteResult> CloseIncident(string incidentId, string reason);

        Task<ExecuteResult> CompleteIncident(string incidentId);

        Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto);

        Task<ActiveIncidentListGto> GetActiveIncidentList();

        Task<ApiResponse<CreateIncidentResponseGto>> CreateIncidentByAlarm(CreateIncidentByAlarmRequestGto request);

        Task<ApiResponse<GetWorkflowDesignIdentifiersResponseGto>> GetWorkflowDesignIds();

        Task<ApiResponse<GetWorkflowDesignsResponseGto>> GetWorkflowDesigns(GetWorkflowDesignsRequestGto request);

        Task<ApiResponse<GetIncidentStatusResponseGto>> GetIncidentStatusWithAlarmId(GetIncidentStatusRequestGto request);
    }
}
