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
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.Incident
{
    public class IncidentAppService :
        ApplicationService,
        IIncidentAppService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IIncidentRepository _incidentRepository;

        public IncidentAppService(IIncidentRepository incidentRepository,
            IDeviceRepository deviceRepository)
        {
            _incidentRepository = incidentRepository;
            _deviceRepository = deviceRepository;
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
                var incidentInfo = await _incidentRepository.GetIncidentById(incidentId);
              
                if (string.IsNullOrEmpty(incidentInfo.DeviceId))
                {
                    return incidentInfo;
                }

                var deviceInfo = await _deviceRepository.GetDeviceById(incidentInfo.DeviceId);
                incidentInfo.DeviceDisplayName = deviceInfo.Config[0].Identifiers.Name;
                incidentInfo.DeviceLocation = deviceInfo.Config[0].Identifiers.Tag[0];

                return incidentInfo;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<IncidentDetailGto>();
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
                Logger.LogInformation("call Incident api GetDeviceList Start");
                var result = await _deviceRepository.GetDevices();

                var devices = result.Config.GroupBy(item => new {item.Relation[0].Id, item.Relation[0].EntityId})
                    .Select(group => new SiteDeviceGto
                    {
                        SiteId = group.Key.Id,
                        SiteDisplayName = group.Key.EntityId,
                        Devices = group.Select(x => new DeviceGto
                            {
                                DeviceDisplayName = x.Identifiers.Name,
                                DeviceId = x.Identifiers.Id,
                                DeviceType = x.Type,
                                DeviceLocation = x.Identifiers.Tag[0]
                            })
                            .ToArray()
                    });

                Logger.LogInformation("call Incident api GetDeviceList end");
                return devices.ToArray();
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
        
        public async Task<ApiResponse<Guid[]>> CreateByAlarmAsync(CreateIncidentByAlarmRequestGto[] requests)
        {
            try
            {
                return await _incidentRepository.CreateIncidentByAlarm(requests);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<Guid[]>();
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
