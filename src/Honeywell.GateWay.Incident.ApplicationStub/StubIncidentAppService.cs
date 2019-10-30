using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Detail;
using Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.List;
using Honeywell.Gateway.Incident.Api.WorkflowDesign;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubIncidentAppService : BaseIncidentStub, IIncidentAppService
    {
  
        public Task<ApiResponse> UpdateStepStatusAsync(string workflowStepId, bool isHandled)
        {
            return ResponseRequest();
        }


        public async Task<ApiResponse<GetDetailResponseGto>> GetDetailAsync(string incidentId)
        {
            var incidentInfo = (await StubDataAsync<GetDetailResponseGto[]>()).First(m => m.Id == Guid.Parse(incidentId));
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
            var workflowName = (await StubDataAsync<WorkflowDesignGto[]>())
                .FirstOrDefault(m => m.Id == Guid.Parse(request.WorkflowDesignReferenceId))
                ?.Name;
            var incident = StubData<GetDetailResponseGto[]>().FirstOrDefault(m => m.WorkflowName == workflowName);
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

        public async Task<ApiResponse<GetListResponseGto>> GetListAsync()
        {
            var result = await StubDataAsync<List<IncidentGto>>();
            return new GetListResponseGto {List = result};
        }

        public async Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarmAsync(
            CreateByAlarmRequestGto request)
        {
            try
            {
                var response = new CreateIncidentResponseGto();
                foreach (var incidentData in request.CreateDatas)
                {
                    var workflowName = (await StubDataAsync<WorkflowDesignGto[]>())
                        .FirstOrDefault(m => m.Id == incidentData.WorkflowDesignReferenceId)?.Name;

                    var incident = (await StubDataAsync<GetDetailResponseGto[]>()).FirstOrDefault(m => m.WorkflowName == workflowName);
                    if (incident != null) response.IncidentIds.Add(incident.Id);
                    throw new Exception("cannot found the incident");
                }

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<CreateIncidentResponseGto>();
            }
        }

        public async Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarmAsync(
            GetStatusByAlarmRequestGto request)
        {
            try
            {
                var response = new GetStatusByAlarmResponseGto();
                foreach (var id in request.AlarmIds)
                {
                    var incident = (await StubDataAsync<GetDetailResponseGto[]>()).FirstOrDefault((m => m.DeviceId == id));
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

                    response.IncidentStatusInfos.Add(statusInfoGto);
                }

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<GetStatusByAlarmResponseGto>();
            }
        }

        public Task<ApiResponse> AddStepCommentAsync(AddStepCommentGto addStepCommentGto)
        {
            return ResponseRequest();
        }
    }
}
