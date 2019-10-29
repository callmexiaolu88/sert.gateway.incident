using System.IO;
using Honeywell.Infra.Core;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IIncidentApi : IRemoteService
    {
        Task<ExecuteResult> UpdateWorkflowStepStatusAsync(string workflowStepId, bool isHandled);

        Task<IncidentGto> GetAsync(string incidentId);

        Task<string> CreateAsync(CreateIncidentRequestGto request);

        Task<ActiveIncidentListGto> GetsAsync();

        Task<SiteDeviceGto[]> GetSiteDevicesAsync();

        Task<ExecuteResult> RespondAsync(string incidentId);

        Task<ExecuteResult> TakeoverAsync(string incidentId);

        Task<ExecuteResult> CloseAsync(string incidentId, string reason);

        Task<ExecuteResult> CompleteAsync(string incidentId);

        Task<ExecuteResult> AddStepCommentAsync(AddStepCommentGto addStepCommentGto);

        Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarm(CreateByAlarmRequestGto request);

        Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarm(GetStatusByAlarmRequestGto request);
    }
}
