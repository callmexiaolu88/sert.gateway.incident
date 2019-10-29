using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IIncidentRepository
    {
        Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled);

        Task<IncidentGto> GetIncidentById(string incidentId);

        Task<string> CreateIncident(CreateIncidentRequestGto request);

        Task<ExecuteResult> RespondIncident(string incidentId);

        Task<ExecuteResult> TakeoverIncident(string incidentId);

        Task<ExecuteResult> CloseIncident(string incidentId, string reason);

        Task<ExecuteResult> CompleteIncident(string incidentId);

        Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto);

        Task<ActiveIncidentListGto> GetActiveIncidentList();

        Task<CreateIncidentResponseGto> CreateIncidentByAlarm(CreateByAlarmRequestGto request);

        Task<GetStatusByAlarmResponseGto> GetIncidentStatusByAlarm(GetStatusByAlarmRequestGto request);
    }
}
