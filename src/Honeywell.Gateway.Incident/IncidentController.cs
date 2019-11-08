using System;
using Honeywell.Gateway.Incident.Api;
using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Application.Incident;
using Microsoft.AspNetCore.Mvc;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Infra.Api.Abstract;
using Microsoft.Extensions.Logging;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;

namespace Honeywell.Gateway.Incident
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IncidentController : ControllerBase, IIncidentApi
    {
        private readonly IIncidentAppService _incidentAppService;

        public IncidentController(ILogger<IncidentController> logger, IIncidentAppService incidentAppService)
        {
            _incidentAppService = incidentAppService;
        }

   
        [HttpPost]
        public async Task<ApiResponse> UpdateStepStatusAsync(string workflowStepId, bool isHandled)
        {
            var result = await _incidentAppService.UpdateStepStatusAsync(workflowStepId, isHandled);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<IncidentDetailGto>> GetDetailAsync(string incidentId)
        {
            var incident = await _incidentAppService.GetDetailAsync(incidentId);
            return incident;
        }

        [HttpPost]
        public async Task<ApiResponse<string>> CreateAsync(CreateIncidentRequestGto request)
        {
            var incidentId = await _incidentAppService.CreateAsync(request);
            return incidentId;
        }

        [HttpPost]
        public async Task<ApiResponse<IncidentSummaryGto[]>> GetListAsync()
        {
            var activeIncidents = await _incidentAppService.GetListAsync();
            return activeIncidents;
        }

        [HttpPost]
        public async Task<ApiResponse<SiteDeviceGto[]>> GetSiteDevicesAsync()
        {
            var devices = await _incidentAppService.GetSiteDevicesAsync();
            return devices;
        }

        [HttpPost]
        public async Task<ApiResponse> RespondAsync(string incidentId)
        {
            var result = await _incidentAppService.RespondAsync(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> TakeoverAsync(string incidentId)
        {
            var result = await _incidentAppService.TakeoverAsync(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> CloseAsync(string incidentId, string reason)
        {
            var result = await _incidentAppService.CloseAsync(incidentId, reason);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> CompleteAsync(string incidentId)
        {
            var result = await _incidentAppService.CompleteAsync(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<Guid[]>> CreateByAlarmAsync(CreateIncidentByAlarmRequestGto[] requests)
        {
            var result = await _incidentAppService.CreateByAlarmAsync(requests);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<IncidentStatusInfoGto[]>> GetStatusByAlarmAsync(string[] alarmIds)
        {
            var result = await _incidentAppService.GetStatusByAlarmAsync(alarmIds);
            return result;
        }

        public async Task<ApiResponse<ActivityGto[]>> GetActivitysAsync(string incidentId)
        {
            var result = await _incidentAppService.GetActivitysAsync(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> AddStepCommentAsync(AddStepCommentRequestGto addStepCommentGto)
        {
            var result = await _incidentAppService.AddStepCommentAsync(addStepCommentGto);
            return result;
        }
    }
}
