using System;
using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IIncidentApi : IRemoteService
    {
        Task<ApiResponse> UpdateStepStatusAsync(string workflowStepId, bool isHandled);

        Task<ApiResponse<IncidentDetailGto>> GetDetailAsync(string incidentId);

        Task<ApiResponse<string>> CreateAsync(CreateIncidentRequestGto request);

        Task<ApiResponse<IncidentSummaryGto[]>> GetListAsync();

        Task<ApiResponse<SiteDeviceGto[]>> GetSiteDevicesAsync();

        Task<ApiResponse> RespondAsync(string incidentId);

        Task<ApiResponse> TakeoverAsync(string incidentId);

        Task<ApiResponse> CloseAsync(string incidentId, string reason);

        Task<ApiResponse> CompleteAsync(string incidentId);

        Task<ApiResponse> AddStepCommentAsync(AddStepCommentRequestGto addStepCommentGto);

        Task<ApiResponse<Guid[]>> CreateByAlarmAsync(CreateIncidentByAlarmRequestGto[] requests);

        Task<ApiResponse<IncidentStatusInfoGto[]>> GetStatusByAlarmAsync(string[] alarmIds);

        Task<ApiResponse<ActivityGto[]>> GetActivitysAsync(string incidentId);
    }
}
