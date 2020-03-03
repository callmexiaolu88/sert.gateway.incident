using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IIncidentRepository
    {
        Task UpdateWorkflowStepStatus(UpdateStepStatusRequestGto updateWorkflowStepStatusGto);

        Task<IncidentDetailGto> GetIncidentById(string incidentId);

        Task<string> CreateIncident(CreateIncidentRequestGto request);

        Task RespondIncident(string incidentId);

        Task TakeoverIncident(string incidentId);

        Task CloseIncident(string incidentId, string reason);

        Task CompleteIncident(string incidentId);

        Task AddStepComment(AddStepCommentRequestGto addStepCommentGto);
        
        Task<ApiResponse<CreateIncidentByAlarmResponseGto>> CreateIncidentByAlarm(CreateIncidentByAlarmRequestGto[] requests);

        Task<ApiResponse<IncidentStatusInfoGto[]>> GetIncidentStatusByAlarm(string[] alarmIds);

        Task<ActivityGto[]> GetActivitys(string incidentId);

        Task<IncidentStatisticsGto> GetStatistics(string deviceId);

        Task<IncidentSummaryGto[]> GetList(PageRequest<GetListRequestGto> request);
    }
}
