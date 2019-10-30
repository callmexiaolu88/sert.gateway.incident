using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Detail;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.List;
using System.Threading.Tasks;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IIncidentRepository
    {
        Task UpdateWorkflowStepStatus(string workflowStepId, bool isHandled);

        Task<GetDetailResponseGto> GetIncidentById(string incidentId);

        Task<string> CreateIncident(CreateIncidentRequestGto request);

        Task RespondIncident(string incidentId);

        Task TakeoverIncident(string incidentId);

        Task CloseIncident(string incidentId, string reason);

        Task CompleteIncident(string incidentId);

        Task AddStepComment(AddStepCommentGto addStepCommentGto);

        Task<GetListResponseGto> GetActiveIncidentList();

        Task<CreateIncidentResponseGto> CreateIncidentByAlarm(CreateByAlarmRequestGto request);

        Task<GetStatusByAlarmResponseGto> GetIncidentStatusByAlarm(GetStatusByAlarmRequestGto request);
    }
}
