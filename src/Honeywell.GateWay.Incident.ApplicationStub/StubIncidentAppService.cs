using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubIncidentAppService : BaseIncidentStub, IIncidentAppService
    {
  
        public Task<ApiResponse> UpdateStepStatusAsync(string workflowStepId, bool isHandled)
        {
            return ResponseRequest();
        }


        public async Task<ApiResponse<IncidentDetailGto>> GetDetailAsync(string incidentId)
        {
            var incidentInfo = (await StubDataAsync<IncidentDetailGto[]>()).First(m => m.Id == Guid.Parse(incidentId));
            if (string.IsNullOrEmpty(incidentInfo.DeviceId))
            {
                return incidentInfo;
            }

            var devices = StubData<SiteDeviceGto[]>();
            var deviceList = (from site in devices
                              select site.Devices.FirstOrDefault(x => x.DeviceId == incidentInfo.DeviceId)
                into item
                              where item != null
                              select item).ToList();
            var device = deviceList.First();
            incidentInfo.DeviceDisplayName = device.DeviceDisplayName;
            incidentInfo.DeviceLocation = device.DeviceLocation;
            return incidentInfo;
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateIncidentRequestGto request)
        {
            var workflowName = (await StubDataAsync<WorkflowDesignDetailGto[]>())
                .FirstOrDefault(m => m.Id == Guid.Parse(request.WorkflowDesignReferenceId))
                ?.Name;
            var incident = StubData<IncidentDetailGto[]>().FirstOrDefault(m => m.WorkflowName == workflowName);
            if (incident != null) return incident.Id.ToString();
            throw new Exception("cannot found the incident");
        }

        public async Task<ApiResponse<SiteDeviceGto[]>> GetSiteDevicesAsync()
        {
            return await StubDataAsync<SiteDeviceGto[]>();
        }

        public Task<ApiResponse> RespondAsync(string incidentId)
        {
            return ResponseRequest();
        }

        private static Task<ApiResponse> ResponseRequest()
        {
            var response = ApiResponse.CreateSuccess();
            return Task.FromResult(response);
        }

        public Task<ApiResponse> TakeoverAsync(string incidentId)
        {
            return ResponseRequest();
        }

        public Task<ApiResponse> CloseAsync(string incidentId, string reason)
        {
            return ResponseRequest();
        }

        public Task<ApiResponse> CompleteAsync(string incidentId)
        {
            return ResponseRequest();
        }

        public async Task<ApiResponse<IncidentSummaryGto[]>> GetListAsync()
        {
            var result = await StubDataAsync<List<IncidentSummaryGto>>();
            return result.ToArray();
        }

        public async Task<ApiResponse<Guid[]>> CreateByAlarmAsync(
            CreateIncidentByAlarmRequestGto[] requests)
        {
            try
            {
                var result = new List<Guid>();
                foreach (var request in requests)
                {
                    var workflowName = (await StubDataAsync<WorkflowDesignDetailGto[]>())
                        .FirstOrDefault(m => m.Id == request.WorkflowDesignReferenceId)?.Name;

                    var incident = (await StubDataAsync<IncidentDetailGto[]>()).FirstOrDefault(m => m.WorkflowName == workflowName);

                    if (incident != null)
                    {
                        result.Add(incident.Id);
                    }
                    else
                    {
                        throw new Exception("cannot found the incident");
                    }
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<Guid[]>();
            }
        }

        public async Task<ApiResponse<IncidentStatusInfoGto[]>> GetStatusByAlarmAsync(string[] alarmIds)
        {
            try
            {
                var result = new List<IncidentStatusInfoGto>();
                foreach (var id in alarmIds)
                {
                    var incident = (await StubDataAsync<IncidentDetailGto[]>()).FirstOrDefault((m => m.DeviceId == id));
                    if (incident == null)
                    {
                        throw new Exception($"cannot found the incident associates with alarm id {id}");
                    }

                    var statusInfoGto = new IncidentStatusInfoGto
                    {
                        AlarmId = id,
                        IncidentId = incident.Id,
                        Status = incident.State
                    };

                    result.Add(statusInfoGto);
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<IncidentStatusInfoGto[]>();
            }
        }

        public async Task<ApiResponse<ActivityGto[]>> GetActivitysAsync(string incidentId)
        {
            return await StubDataAsync<ActivityGto[]>();
        }

        public Task<ApiResponse> AddStepCommentAsync(AddStepCommentRequestGto addStepCommentGto)
        {
            return ResponseRequest();
        }
    }
}
