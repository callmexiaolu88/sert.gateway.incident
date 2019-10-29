using System.IO;
using Honeywell.Gateway.Incident.Api;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.Infra.Api.Abstract;
using Microsoft.Extensions.Logging;

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
        public async Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled)
        {
            var result = await _incidentAppService.UpdateWorkflowStepStatus(workflowStepId, isHandled);
            return result;
        }

        [HttpPost]
        public async Task<IncidentGto> GetIncidentById(string incidentId)
        {
            var incident = await _incidentAppService.GetIncidentById(incidentId);
            return incident;
        }

        [HttpPost]
        public async Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            var incidentId = await _incidentAppService.CreateIncident(request);
            return incidentId;
        }

        [HttpPost]
        public async Task<ActiveIncidentListGto> GetActiveIncidentList()
        {
            var activeIncidents = await _incidentAppService.GetActiveIncidentList();
            return activeIncidents;
        }

        [HttpPost]
        public async Task<SiteDeviceGto[]> GetSiteDevices()
        {
            var devices = await _incidentAppService.GetSiteDevices();
            return devices;
        }

        [HttpPost]
        public async Task<ExecuteResult> RespondIncident(string incidentId)
        {
            var result = await _incidentAppService.RespondIncident(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> TakeoverIncident(string incidentId)
        {
            var result = await _incidentAppService.TakeoverIncident(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> CloseIncident(string incidentId, string reason)
        {
            var result = await _incidentAppService.CloseIncident(incidentId, reason);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> CompleteIncident(string incidentId)
        {
            var result = await _incidentAppService.CompleteIncident(incidentId);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarm(CreateByAlarmRequestGto request)
        {
            var result = await _incidentAppService.CreateByAlarm(request);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarm(GetStatusByAlarmRequestGto request)
        {
            var result = await _incidentAppService.GetStatusByAlarm(request);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto)
        {
            var result = await _incidentAppService.AddStepComment(addStepCommentGto);
            return result;
        }
    }
}
