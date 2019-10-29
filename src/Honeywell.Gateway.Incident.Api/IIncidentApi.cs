using System.IO;
using Honeywell.Infra.Core;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Gateway.Incident.Api.Incident;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Detail;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.List;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IIncidentApi : IRemoteService
    {
        Task<ExecuteResult> UpdateStepStatusAsync(string workflowStepId, bool isHandled);

        Task<GetDetailResponseGto> GetDetailAsync(string incidentId);

        Task<string> CreateAsync(CreateIncidentRequestGto request);

        Task<GetListResponseGto> GetListAsync();

        Task<SiteDeviceGto[]> GetSiteDevicesAsync();

        Task<ExecuteResult> RespondAsync(string incidentId);

        Task<ExecuteResult> TakeoverAsync(string incidentId);

        Task<ExecuteResult> CloseAsync(string incidentId, string reason);

        Task<ExecuteResult> CompleteAsync(string incidentId);

        Task<ExecuteResult> AddStepCommentAsync(AddStepCommentGto addStepCommentGto);

        Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarmAsync(CreateByAlarmRequestGto request);

        Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarmAsync(GetStatusByAlarmRequestGto request);
    }
}
