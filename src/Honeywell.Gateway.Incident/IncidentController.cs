using System.IO;
using Honeywell.Gateway.Incident.Api;
using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Application.Incident;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Infra.Api.Abstract;
using Microsoft.Extensions.Logging;
using Honeywell.Gateway.Incident.Api.Incident;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Detail;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.List;

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
        public async Task<ApiResponse<GetDetailResponseGto>> GetDetailAsync(string incidentId)
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
        public async Task<ApiResponse<GetListResponseGto>> GetListAsync()
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
        public async Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarmAsync(CreateByAlarmRequestGto request)
        {
            var result = await _incidentAppService.CreateByAlarmAsync(request);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarmAsync(GetStatusByAlarmRequestGto request)
        {
            var result = await _incidentAppService.GetStatusByAlarmAsync(request);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> AddStepCommentAsync(AddStepCommentGto addStepCommentGto)
        {
            var result = await _incidentAppService.AddStepCommentAsync(addStepCommentGto);
            return result;
        }
    }
}
