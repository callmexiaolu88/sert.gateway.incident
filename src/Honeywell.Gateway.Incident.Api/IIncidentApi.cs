using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Detail;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;
using System.Threading.Tasks;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IIncidentApi : IRemoteService
    {
        Task<ApiResponse> UpdateStepStatusAsync(string workflowStepId, bool isHandled);

        Task<ApiResponse<GetDetailResponseGto>> GetDetailAsync(string incidentId);

        Task<ApiResponse<string>> CreateAsync(CreateIncidentRequestGto request);

        Task<ApiResponse<GetListResponseGto>> GetListAsync();

        Task<ApiResponse<SiteDeviceGto[]>> GetSiteDevicesAsync();

        Task<ApiResponse> RespondAsync(string incidentId);

        Task<ApiResponse> TakeoverAsync(string incidentId);

        Task<ApiResponse> CloseAsync(string incidentId, string reason);

        Task<ApiResponse> CompleteAsync(string incidentId);

        Task<ApiResponse> AddStepCommentAsync(AddStepCommentGto addStepCommentGto);

        Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarmAsync(CreateByAlarmRequestGto request);

        Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarmAsync(GetStatusByAlarmRequestGto request);
    }
}
