using System;
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
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Infra.Services.Isom.Api;
using Honeywell.Infra.Services.Isom.Api.Custom;
using Honeywell.Infra.Services.Isom.Api.Custom.Camera.GetCamera;
using Microsoft.Extensions.Logging;
using Proxy.Honeywell.Security.ISOM.Devices;

namespace Honeywell.GateWay.Incident.Application.Incident
{
    public class IncidentAppService :
        ApplicationService,
        IIncidentAppService
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ICameraFacadeApi _cameraFacadeApi;
        private readonly IDeviceFacadeApi _deviceFacadeApi;

        public IncidentAppService(IIncidentRepository incidentRepository,
            ICameraFacadeApi cameraFacadeApi,
            IDeviceFacadeApi deviceFacadeApi)
        {
            _cameraFacadeApi = cameraFacadeApi;
            _deviceFacadeApi = deviceFacadeApi;
            _incidentRepository = incidentRepository;
        }

        public async Task<ApiResponse> UpdateStepStatusAsync(UpdateStepStatusRequestGto updateWorkflowStepStatusGto)
        {
            try
            {
                await _incidentRepository.UpdateWorkflowStepStatus(updateWorkflowStepStatusGto);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse<IncidentDetailGto>> GetDetailAsync(string incidentId)
        {
            try
            {
                const string emptyDeviceId= "0x000000000000000000000000000000000000";
                var incidentInfo = await _incidentRepository.GetIncidentById(incidentId);
                if (string.IsNullOrEmpty(incidentInfo.DeviceId)||incidentId.Equals(emptyDeviceId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return incidentInfo;
                }

                var deviceInfo = _deviceFacadeApi.GetDeviceDetails(incidentInfo.DeviceId, null);
                incidentInfo.DeviceDisplayName = deviceInfo.identifiers.name;
                incidentInfo.DeviceLocation = deviceInfo.identifiers.tag[0];

                if (incidentInfo.TriggerType == Gateway.Incident.Api.IncidentTriggerType.Alarm)
                {
                    incidentInfo.EventTimeStamp = MappingEventTimeStamp(incidentInfo.AlarmData.AlarmUtcDateTime);
                    var getCameraInfo = _cameraFacadeApi.GetCameraByAlarmId(incidentInfo.TriggerId);
                    MappingCameraId(incidentInfo, getCameraInfo);
                }

                if (incidentInfo.TriggerType == Gateway.Incident.Api.IncidentTriggerType.Manual)
                {
                    var request = new GetCameraRequestDto { DeviceId = incidentInfo.TriggerId , DeviceType = deviceInfo.type};
                    var getCameraInfo = _cameraFacadeApi.GetCamera(request);
                    MappingCameraId(incidentInfo, getCameraInfo);
                    if (incidentInfo.CreateAtUtc != null)
                    {
                        incidentInfo.EventTimeStamp = MappingEventTimeStamp(incidentInfo.CreateAtUtc.Value);
                    }
                }

                return incidentInfo;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<IncidentDetailGto>();
            }
        }

        private long MappingEventTimeStamp(DateTime evenDateTime)
        {
            var utcDateTime = DateTime.SpecifyKind(evenDateTime, DateTimeKind.Utc);
            return new DateTimeOffset(utcDateTime).ToUnixTimeMilliseconds();
        }

        private void MappingCameraId(IncidentDetailGto gto, ApiResponse<GetCameraInfo> getCameraInfo)
        {
            if (getCameraInfo.IsSuccess)
            {
                gto.CameraNumber = getCameraInfo.Value.CameraNum;
            }
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateIncidentRequestGto request)
        {
            try
            {
                return await _incidentRepository.CreateIncident(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<string>();
            }
        }

        public async Task<ApiResponse<IncidentSummaryGto[]>> GetListAsync(PageRequest<GetListRequestGto> request)
        {
            try
            {
                return await _incidentRepository.GetList(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<IncidentSummaryGto[]>();
            }
        }

        public async Task<ApiResponse<SiteDeviceGto[]>> GetSiteDevicesAsync()
        {
            try
            {
                Logger.LogInformation("call Incident api GetDevices Start");
                var deviceConfigs = _deviceFacadeApi.GetDeviceList(null);
                var devices = deviceConfigs.config.GroupBy(item => new { item.relation[0].id, item.relation[0].entityId })
                    .Select(group => new SiteDeviceGto
                    {
                        SiteId = group.Key.id,
                        SiteDisplayName = group.Key.entityId,
                        Devices = group.Select(x => new DeviceGto
                        {
                            DeviceDisplayName = x.identifiers.name,
                            DeviceId = x.identifiers.id,
                            DeviceType = DeviceTypeHelper.GetSystemDeviceType(x.type.ToString()),
                            DeviceLocation = x.identifiers.tag[0]
                        }).ToArray()
                    });
                Logger.LogInformation("call Incident api GetDevices end");
                return await Task.FromResult(devices.ToArray());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<SiteDeviceGto[]>();
            }
        }

        public async Task<ApiResponse> RespondAsync(string incidentId)
        {
            try
            {
                await _incidentRepository.RespondIncident(incidentId);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse> TakeoverAsync(string incidentId)
        {
            try
            {
                await _incidentRepository.TakeoverIncident(incidentId);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse> CloseAsync(string incidentId, string reason)
        {
            try
            {
                await _incidentRepository.CloseIncident(incidentId, reason);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse> CompleteAsync(string incidentId)
        {
            try
            {
                await _incidentRepository.CompleteIncident(incidentId);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse<CreateIncidentByAlarmResponseGto>> CreateByAlarmAsync(CreateIncidentByAlarmRequestGto[] requests)
        {
            try
            {
                return await _incidentRepository.CreateIncidentByAlarm(requests);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<CreateIncidentByAlarmResponseGto>();
            }
        }

        public async Task<ApiResponse<IncidentStatusInfoGto[]>> GetStatusByAlarmAsync(string[] alarmIds)
        {
            try
            {
                return await _incidentRepository.GetIncidentStatusByAlarm(alarmIds);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<IncidentStatusInfoGto[]>();
            }
        }

        public async Task<ApiResponse<ActivityGto[]>> GetActivitysAsync(string incidentId)
        {
            try
            {
                return await _incidentRepository.GetActivitys(incidentId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<ActivityGto[]>();
            }
        }

        public async Task<ApiResponse<IncidentStatisticsGto>> GetStatisticsAsync(string deviceId)
        {
            try
            {
                return await _incidentRepository.GetStatistics(deviceId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<IncidentStatisticsGto>();
            }
        }

        public async Task<ApiResponse> AddStepCommentAsync(AddStepCommentRequestGto addStepCommentGto)
        {
            try
            {
                await _incidentRepository.AddStepComment(addStepCommentGto);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }
    }
}
