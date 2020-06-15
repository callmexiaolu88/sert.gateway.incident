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
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Infra.Services.Isom.Api;
using Honeywell.Infra.Services.Isom.Api.Custom;
using Honeywell.Infra.Services.Isom.Api.Custom.Camera.GetCamera;
using Honeywell.Infra.Services.Isom.Api.Custom.Device.GetDevice;
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
        private readonly IDeviceFacadeExtendApi _deviceFacadeExtendApi;

        public IncidentAppService(IIncidentRepository incidentRepository,
            ICameraFacadeApi cameraFacadeApi,
            IDeviceFacadeApi deviceFacadeApi, IDeviceFacadeExtendApi deviceFacadeExtendApi)
        {
            _cameraFacadeApi = cameraFacadeApi;
            _deviceFacadeApi = deviceFacadeApi;
            _incidentRepository = incidentRepository;
            _deviceFacadeExtendApi = deviceFacadeExtendApi;
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
                Logger.LogInformation("IncidentAppService.GetDetailAsync begin");
                const string emptyDeviceId= "0x000000000000000000000000000000000000";
                var incidentInfo = await _incidentRepository.GetIncidentById(incidentId);
                if (string.IsNullOrEmpty(incidentInfo.DeviceId)|| incidentInfo.DeviceId.Equals(emptyDeviceId, StringComparison.InvariantCultureIgnoreCase))
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

                Logger.LogInformation("IncidentAppService.GetDetailAsync end");

                return incidentInfo;
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetDetailAsync Failed: {ex}", ex);
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

        public async Task<ApiResponse<SiteGto[]>> GetSiteListByDeviceNameAsync(string deviceName)
        {
            try
            {
                List<SiteGto> siteGtoList=new List<SiteGto>();
                var deviceConfigs = _deviceFacadeExtendApi.GetSiteListByDeviceName(deviceName);
                foreach (var device in deviceConfigs.config)
                {
                    if (!siteGtoList.Any(t => t.SiteId.Equals(device.relation[0].id)))
                    {
                        var site = new SiteGto
                        {
                            SiteId = device.relation[0].id,
                            SiteDisplayName = device.relation[0].entityId,
                            DeviceCount = deviceConfigs.config.Count(d => d.relation[0].id.Equals(device.relation[0].id))
                        };
                        siteGtoList.Add(site);
                    }
                }

                return await Task.FromResult(siteGtoList.ToArray());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<SiteGto[]>();
            }
        }

        public async Task<ApiResponse<DeviceGto[]>> GetDeviceListAsync(GetDeviceListRequestGto request)
        {
            try
            {
                DeviceRequestDto deviceRequestDto = new DeviceRequestDto
                    {SiteId = request.SiteId, DeviceName = request.DeviceName};
                var deviceConfigs = _deviceFacadeExtendApi.GetDeviceList(deviceRequestDto);
                var deviceGtos = deviceConfigs.config.Select(
                    x=>new DeviceGto
                    {
                        DeviceDisplayName = x.identifiers.name,
                        DeviceId = x.identifiers.id,
                        DeviceType = DeviceTypeHelper.GetSystemDeviceType(x.type.ToString()),
                        DeviceLocation = x.identifiers.tag[0]
                    }).ToArray();

                return await Task.FromResult(deviceGtos.ToArray());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<DeviceGto[]>();
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
