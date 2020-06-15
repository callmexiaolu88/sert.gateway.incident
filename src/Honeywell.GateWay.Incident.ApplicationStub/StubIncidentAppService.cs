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
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubIncidentAppService : BaseIncidentStub, IIncidentAppService
    {
  
        public Task<ApiResponse> UpdateStepStatusAsync(UpdateStepStatusRequestGto updateWorkflowStepStatusGto)
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
            if (incident == null)
            {
                throw new HoneywellException("cannot found the incident");
            }
            return incident.Id.ToString();
        }

        public async Task<ApiResponse<SiteDeviceGto[]>> GetSiteDevicesAsync()
        {
            return await StubDataAsync<SiteDeviceGto[]>();
        }

        public async Task<ApiResponse<SiteGto[]>> GetSiteListByDeviceNameAsync(string deviceName)
        {
            return await StubDataAsync<SiteGto[]>();
        }

        public async Task<ApiResponse<DeviceGto[]>> GetDeviceListAsync(GetDeviceListRequestGto request)
        {
            return await StubDataAsync<DeviceGto[]>();
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

        public async Task<ApiResponse<IncidentSummaryGto[]>> GetListAsync(PageRequest<GetListRequestGto> request)
        {
            var result = await StubDataAsync<List<IncidentSummaryGto>>();
            if (string.IsNullOrEmpty(request.Value.DeviceId))
            {
               return result.ToArray();
            }
            return result.Where(x => x.DeviceId == request.Value.DeviceId).ToArray();
        }

        public async Task<ApiResponse<CreateIncidentByAlarmResponseGto>> CreateByAlarmAsync(
            CreateIncidentByAlarmRequestGto[] requests)
        {
            try
            {
                if (requests == null)
                {
                    throw new ArgumentNullException(nameof(requests));
                }
                var result = new List<IncidentAlarmGto>();
                foreach (var request in requests)
                {
                    var item = new IncidentAlarmGto {AlarmId = request.AlarmId};
                    var workflowName = (await StubDataAsync<WorkflowDesignDetailGto[]>())
                        .FirstOrDefault(m => m.Id == request.WorkflowDesignReferenceId)?.Name;

                    var incident = (await StubDataAsync<IncidentDetailGto[]>()).FirstOrDefault(m => m.WorkflowName == workflowName);

                    if (incident != null)
                    {
                        item.IncidentId = incident.Id;
                        item.IsCreatedAtThisRequest = true;
                        result.Add(item);
                    }
                    else
                    {
                        throw new HoneywellException("cannot found the incident");
                    }

                }

                return new CreateIncidentByAlarmResponseGto {IncidentAlarmInfos = result};
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<CreateIncidentByAlarmResponseGto>();
            }
        }

        public async Task<ApiResponse<IncidentStatusInfoGto[]>> GetStatusByAlarmAsync(string[] alarmIds)
        {
            try
            {
                var result = new List<IncidentStatusInfoGto>();
                if (alarmIds == null)
                {
                    throw new ArgumentNullException(nameof(alarmIds));
                }
                foreach (var id in alarmIds)
                {
                    var incident = (await StubDataAsync<IncidentDetailGto[]>()).FirstOrDefault((m => m.DeviceId == id));
                    if (incident == null)
                    {
                        throw new HoneywellException($"cannot found the incident associates with alarm id {id}");
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

        public async Task<ApiResponse<IncidentStatisticsGto>> GetStatisticsAsync(string deviceId)
        {
            return await StubDataAsync<IncidentStatisticsGto>();
        }

        public Task<ApiResponse> AddStepCommentAsync(AddStepCommentRequestGto addStepCommentGto)
        {
            return ResponseRequest();
        }

    }
}
