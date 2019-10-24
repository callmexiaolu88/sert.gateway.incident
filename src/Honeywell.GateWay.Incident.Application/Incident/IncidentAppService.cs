﻿using System;
 using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Gtos.Create;
using Honeywell.Gateway.Incident.Api.Gtos.Detail;
using Honeywell.Gateway.Incident.Api.Gtos.Status;
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

        public async Task<ExecuteResult> ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            return await _incidentRepository.ImportWorkflowDesigns(workflowDesignStream);
        }

        public async Task<ExecuteResult> ValidatorWorkflowDesigns(Stream workflowDesignStream)
        {
            return await _incidentRepository.ValidatorWorkflowDesigns(workflowDesignStream);
        }

        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            return await _incidentRepository.DeleteWorkflowDesigns(workflowDesignIds);
        }

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            return await _incidentRepository.GetAllActiveWorkflowDesigns();
        }

        public async Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors()
        {
            return await _incidentRepository.GetWorkflowDesignSelectors();
        }

        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            return await _incidentRepository.GetWorkflowDesignById(workflowDesignId);
        }

        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            return await _incidentRepository.DownloadWorkflowTemplate();
        }

        public async Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            return await _incidentRepository.ExportWorkflowDesigns(workflowIds);
        }

        public async Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled)
        {
            return await _incidentRepository.UpdateWorkflowStepStatus(workflowStepId, isHandled);
        }

        public async Task<IncidentGto> GetIncidentById(string incidentId)
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

        public async Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            return await _incidentRepository.CreateIncident(request);
        }

        public async Task<SiteDeviceGto[]> GetSiteDevices()
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

        public async Task<ExecuteResult> RespondIncident(string incidentId)
        {
            return await _incidentRepository.RespondIncident(incidentId);
        }

        public async Task<ExecuteResult> TakeoverIncident(string incidentId)
        {
            return await _incidentRepository.TakeoverIncident(incidentId);
        }

        public async Task<ExecuteResult> CloseIncident(string incidentId, string reason)
        {
            return await _incidentRepository.CloseIncident(incidentId, reason);
        }

        public async Task<ExecuteResult> CompleteIncident(string incidentId)
        {
            return await _incidentRepository.CompleteIncident(incidentId);
        }

        public async Task<ActiveIncidentListGto> GetActiveIncidentList()
        {
            return await _incidentRepository.GetActiveIncidentList();
        }

        public async Task<ApiResponse<CreateIncidentResponseGto>> CreateIncidentByAlarm(
            CreateIncidentByAlarmRequestGto request)
        {
            try
            {
                return await _incidentRepository.CreateIncidentByAlarm(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To(new CreateIncidentResponseGto());
            }
        }

        public async Task<ApiResponse<GetWorkflowDesignIdentifiersResponseGto>> GetWorkflowDesignIds()
        {
            try
            {
                return await _incidentRepository.GetWorkflowDesignIds();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To(new GetWorkflowDesignIdentifiersResponseGto());
            }
        }

        public async Task<ApiResponse<GetWorkflowDesignsResponseGto>> GetWorkflowDesigns(
            GetWorkflowDesignsRequestGto request)
        {
            try
            {
                return await _incidentRepository.GetWorkflowDesigns(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To(new GetWorkflowDesignsResponseGto());
            }
        }

        public async Task<ApiResponse<GetIncidentStatusResponseGto>> GetIncidentStatusWithAlarmId(
            GetIncidentStatusRequestGto request)
        {
            try
            {
                return await _incidentRepository.GetIncidentStatusWithAlarmId(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To(new GetIncidentStatusResponseGto());
            }
        }

        public async Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto)
        {
            return await _incidentRepository.AddStepComment(addStepCommentGto);
        }
    }
}
