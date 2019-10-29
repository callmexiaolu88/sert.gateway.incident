using System;
 using System.IO;
 using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
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

        public async Task<ExecuteResult> UpdateWorkflowStepStatusAsync(string workflowStepId, bool isHandled)
        {
            return await _incidentRepository.UpdateWorkflowStepStatus(workflowStepId, isHandled);
        }

        public async Task<IncidentGto> GetAsync(string incidentId)
        {
            var incidentInfo = await _incidentRepository.GetIncidentById(incidentId);
            if (incidentInfo.Status != ExecuteStatus.Successful)
            {
                return incidentInfo;
            }

            if (string.IsNullOrEmpty(incidentInfo.DeviceId))
            {
                return incidentInfo;
            }

            var deviceInfo = await _deviceRepository.GetDeviceById(incidentInfo.DeviceId);
            incidentInfo.DeviceDisplayName = deviceInfo.Config[0].Identifiers.Name;
            incidentInfo.DeviceLocation = deviceInfo.Config[0].Identifiers.Tag[0];

            return incidentInfo;
        }

        public async Task<string> CreateAsync(CreateIncidentRequestGto request)
        {
            return await _incidentRepository.CreateIncident(request);
        }

        public async Task<SiteDeviceGto[]> GetSiteDevicesAsync()
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

        public async Task<ExecuteResult> RespondAsync(string incidentId)
        {
            return await _incidentRepository.RespondIncident(incidentId);
        }

        public async Task<ExecuteResult> TakeoverAsync(string incidentId)
        {
            return await _incidentRepository.TakeoverIncident(incidentId);
        }

        public async Task<ExecuteResult> CloseAsync(string incidentId, string reason)
        {
            return await _incidentRepository.CloseIncident(incidentId, reason);
        }

        public async Task<ExecuteResult> CompleteAsync(string incidentId)
        {
            return await _incidentRepository.CompleteIncident(incidentId);
        }

        public async Task<ActiveIncidentListGto> GetsAsync()
        {
            return await _incidentRepository.GetActiveIncidentList();
        }

        public async Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarmAsync(
            CreateByAlarmRequestGto request)
        {
            try
            {
                return await _incidentRepository.CreateIncidentByAlarm(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<CreateIncidentResponseGto>();
            }
        }

        public async Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarmAsync(
            GetStatusByAlarmRequestGto request)
        {
            try
            {
                return await _incidentRepository.GetIncidentStatusByAlarm(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetStatusByAlarmResponseGto>();
            }
        }

        public async Task<ExecuteResult> AddStepCommentAsync(AddStepCommentGto addStepCommentGto)
        {
            return await _incidentRepository.AddStepComment(addStepCommentGto);
        }
    }
}
