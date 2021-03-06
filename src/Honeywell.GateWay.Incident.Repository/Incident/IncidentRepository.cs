﻿using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Actions;
using Honeywell.Micro.Services.Workflow.Api.Workflow.AddComment;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
using Honeywell.GateWay.Incident.Repository.Incident.IncidentEvents;
using Honeywell.Infra.Core.Common.Exceptions;
using Honeywell.Infra.Services.LiveData.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Actions;
using Honeywell.Micro.Services.Incident.Api.Incident.Create;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.Statistics;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using CreateIncidentByAlarmDto = Honeywell.Micro.Services.Incident.Api.Incident.Create.CreateIncidentByAlarmDto;
using CreateIncidentByAlarmRequestDto = Honeywell.Micro.Services.Incident.Api.Incident.Create.CreateIncidentByAlarmRequestDto;
using CreateIncidentRequestDto = Honeywell.Micro.Services.Incident.Api.Incident.Create.CreateIncidentRequestDto;
using IncidentActivities = Honeywell.GateWay.Incident.Repository.Incident.IncidentEvents.IncidentActivities;

namespace Honeywell.GateWay.Incident.Repository.Incident
{

    public class IncidentRepository : ApplicationService, IIncidentRepository
    {
        private readonly IIncidentMicroApi _incidentMicroApi;
        private readonly IWorkflowMicroApi _workflowMicroApi;
        private readonly ILiveDataApi _liveDataApi;

        public IncidentRepository(
            IIncidentMicroApi incidentMicroApi,
            IWorkflowMicroApi workflowMicroApi,
            ILiveDataApi liveDataApi)
        {
            _incidentMicroApi = incidentMicroApi;
            _workflowMicroApi = workflowMicroApi;
            _liveDataApi = liveDataApi;
        }

        public async Task UpdateWorkflowStepStatus(UpdateStepStatusRequestGto updateWorkflowStepStatusGto)
        {
            if (updateWorkflowStepStatusGto == null)
            {
                throw new ArgumentNullException(nameof(updateWorkflowStepStatusGto));
            }
            Logger.LogInformation("call workflow design api UpdateWorkflowStepStatus Start");
            var workflowStepGuid = Guid.Parse(updateWorkflowStepStatusGto.WorkflowStepId);
            var workflowId = Guid.Parse(updateWorkflowStepStatusGto.WorkflowId);
            var incident = await GetIncidentByWorkflowId(workflowId);

            var request = new UpdateWorkflowStepStatusRequestDto
            {
                WorkflowId = workflowId,
                IncidentNumber = incident.Number,
                WorkflowStepId = workflowStepGuid, 
                IsHandled = updateWorkflowStepStatusGto.IsHandled
            };

            var response = await _workflowMicroApi.UpdateStepStatusAsync(request);

            ApiResponse.ThrowExceptionIfFailed(response);

            await NotificationActivity(incident.Id);
        }

        public async Task<IncidentDetailGto> GetIncidentById(string incidentId)
        {
            Logger.LogInformation("call Incident api GetIncidentById Start");
            if (!Guid.TryParse(incidentId, out var guid))
            {
                throw new ArgumentException("incidentId is invalid", nameof(incidentId));
            }

            var result = new IncidentDetailGto();
            var requestId = new[] { guid };
            var response = await _incidentMicroApi.GetDetailsAsync(new GetIncidentDetailsRequestDto { Ids = requestId });
            ApiResponse.ThrowExceptionIfFailed(response);

            HoneyMapper.Map(response.Value.Details[0], result);
            return result;
        }

        public async Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            Logger.LogInformation("call Incident api CreateIncident Start");
            if (!Guid.TryParse(request.WorkflowDesignReferenceId, out var workflowDesignReferenceId))
            {
                var msg = $"wrong WorkflowDesignReferenceId value: {request.WorkflowDesignReferenceId}";
                Logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            if (!Enum.TryParse<Micro.Services.Incident.Domain.Shared.IncidentPriority>(request.Priority, true, out var priority))
            {
                var msg = $"wrong priority value: {request.Priority}";
                Logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            var incidentRequest = new CreateIncidentRequestDto
            {
                CreateIncidentDatas = new[]
                {
                    new CreateIncidentDataDto
                    {
                        WorkflowDesignReferenceId = workflowDesignReferenceId,
                        Priority = priority,
                        Description = request.Description,
                        DeviceId = request.DeviceId,
                        DeviceType = request.DeviceType
                    }
                }
            };

            var response = await _incidentMicroApi.CreateAsync(incidentRequest);
            ApiResponse.ThrowExceptionIfFailed(response);
            await NotificationIncidentList();
            return response.Value.IncidentIds.First().ToString();
        }

        public async Task RespondIncident(string incidentId)
        {
            Logger.LogInformation("call Incident api RespondIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                var msg = $"wrong incident id: {incidentId}";
                Logger.LogError(msg);
                throw new HoneywellException(msg);
            }

            var request = new IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentMicroApi.RespondAsync(request);
            ApiResponse.ThrowExceptionIfFailed(response);

            await NotificationActivity(incidentGuid);
        }

        public async Task TakeoverIncident(string incidentId)
        {
            Logger.LogInformation("call Incident api TakeoverIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                var msg = $"wrong incident id: {incidentId}";
                Logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            var request = new IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentMicroApi.TakeoverAsync(request);

            ApiResponse.ThrowExceptionIfFailed(response);

            await NotificationActivity(incidentGuid);
        }

        public async Task CloseIncident(string incidentId, string reason)
        {
            Logger.LogInformation("call Incident api CloseIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                var msg = $"wrong incident id: {incidentId}";
                Logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            var request = new CloseIncidentRequestDto { IncidentId = incidentGuid, Reason = reason };

            var response = await _incidentMicroApi.CloseAsync(request);

            ApiResponse.ThrowExceptionIfFailed(response);

            await NotificationActivity(incidentGuid);

            await NotificationIncidentList();
        }

        public async Task CompleteIncident(string incidentId)
        {
            Logger.LogInformation("call Incident api CompleteIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                var msg = $"wrong incident id: {incidentId}";
                Logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            var request = new IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentMicroApi.CompleteAsync(request);

            ApiResponse.ThrowExceptionIfFailed(response);

            await NotificationActivity(incidentGuid);

            await NotificationIncidentList();
        }

        public async Task<IncidentSummaryGto[]> GetList(PageRequest<GetListRequestGto> request)
        {
            Logger.LogInformation("call Incident api GetListAsync Start");
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.Value == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (!string.IsNullOrEmpty(request.Value.DeviceId) && !request.Value.HasOwner.HasValue)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var incidentListRequest = new GetIncidentListRequestDto
            { State = (IncidentState)request.Value.State, DeviceId = request.Value.DeviceId, HasOwner = request.Value.HasOwner };
            var pageRequest = request.To(incidentListRequest);
            var result = await _incidentMicroApi.GetListAsync(pageRequest);
            ApiResponse.ThrowExceptionIfFailed(result);

            var workflowIds = result.Value.List.Select(x => x.WorkflowId).ToArray();
            var workflowSummaries = await GetWorkflowSummary(workflowIds);
            ApiResponse.ThrowExceptionIfFailed(workflowSummaries);

            return MappingIncidentsGtos(result, workflowSummaries);
        }


        public async Task<ApiResponse<CreateIncidentByAlarmResponseGto>> CreateIncidentByAlarm(CreateIncidentByAlarmRequestGto[] requests)
        {
            Logger.LogInformation($"call Incident api {nameof(CreateIncidentByAlarm)} Start");
            var incidentRequest =
                HoneyMapper.Map<CreateIncidentByAlarmRequestGto[], CreateIncidentByAlarmDto[]>(requests);

            var response = await _incidentMicroApi.CreateByAlarmAsync(new CreateIncidentByAlarmRequestDto { CreateIncidentDatas = incidentRequest });

            ApiResponse.ThrowExceptionIfFailed(response,new string[] {
                CreateIncidentByAlarmResponseDto.MessageCodeAlarmDuplication
            });


            await NotificationIncidentList();

            var result=HoneyMapper.Map<CreateIncidentByAlarmResponseDto, CreateIncidentByAlarmResponseGto>(response.Value);

            return response.To(result);
        }

        public async Task<ApiResponse<IncidentStatusInfoGto[]>> GetIncidentStatusByAlarm(string[] alarmIds)
        {
            Logger.LogInformation($"call Incident api {nameof(GetIncidentStatusByAlarm)} Start");

            var response = await _incidentMicroApi.GetStatusByTriggerAsync(new GetIncidentStatusRequestDto { TriggerIds = alarmIds });

            ApiResponse.ThrowExceptionIfFailed(response,new string[]{
                 GetIncidentStatusResponseDto.MessageCodeIncidentNotFound
            });

            var result = HoneyMapper.Map<IncidentStatusDto[], IncidentStatusInfoGto[]>(response.Value.IncidentStatusInfos.ToArray());
            return response.To(result);
        }

        public async Task<ActivityGto[]> GetActivitys(string incidentId)
        {

            Logger.LogInformation($"call Incident api {nameof(GetActivitys)} Start");
            var result = await GetIncidentById(incidentId);
            return result.IncidentActivities.ToArray();
        }

        public async Task<IncidentStatisticsGto> GetStatistics(string deviceId)
        {
            Logger.LogInformation($"call GetStatisticsAsync {nameof(GetStatistics)} Start");
            var response = await _incidentMicroApi.GetStatisticsAsync(new GetIncidentStatisticsRequestDto { DeviceIds = new[] { deviceId } });
            ApiResponse.ThrowExceptionIfFailed(response);
            var result = HoneyMapper.Map<IncidentStatisticsDto, IncidentStatisticsGto>(response.Value.StatisticsIncident[0]);
            return result;
        }

        private IncidentSummaryGto[] MappingIncidentsGtos(ApiResponse<GetIncidentListResponseDto> result, ApiResponse<WorkflowSummaryResponseDto> workflowSummaries)
        {
            var incidentsGtos = HoneyMapper.Map<IncidentListItemDto[], IncidentSummaryGto[]>(result.Value.List.ToArray());
            foreach (var incident in incidentsGtos)
            {
                foreach (var workflowSummary in workflowSummaries.Value.Summaries)
                {
                    if (incident.WorkflowId == workflowSummary.WorkflowId)
                    {
                        incident.WorkflowDesignName = workflowSummary.WorkflowDesignName;
                        incident.CompletedSteps = workflowSummary.CompletedSteps;
                        incident.TotalSteps = workflowSummary.TotalSteps;
                    }
                }
            }
            return incidentsGtos;
        }

        private async Task<ApiResponse<WorkflowSummaryResponseDto>> GetWorkflowSummary(Guid[] workflowIds)
        {
            var request = new WorkflowSummaryRequestDto
            {
                WorkflowIds = workflowIds
            };
            var workflowSummaries = await _workflowMicroApi.GetSummariesAsync(request);
            return workflowSummaries;
        }

        public async Task AddStepComment(AddStepCommentRequestGto addStepCommentGto)
        {
            if (addStepCommentGto == null)
            {
                throw new ArgumentNullException(nameof(addStepCommentGto));
            }

            Logger.LogInformation(
                $"call Incident api AddStepComment Start,workflowStepId:{addStepCommentGto.WorkflowStepId},comment:{addStepCommentGto.Comment}");

            var workflowId = Guid.Parse(addStepCommentGto.WorkflowId);
            var incident = await GetIncidentByWorkflowId(workflowId);

            AddStepCommentRequestDto requestDto = new AddStepCommentRequestDto
            {
                WorkflowId = Guid.Parse(addStepCommentGto.WorkflowId),
                IncidentNumber = incident.Number,
                WorkflowStepId = Guid.Parse(addStepCommentGto.WorkflowStepId),
                Comment = addStepCommentGto.Comment
            };

            var response = await _workflowMicroApi.AddStepCommentAsync(requestDto);

            ApiResponse.ThrowExceptionIfFailed(response);
            await NotificationActivity(incident.Id);
        }

        private async Task NotificationActivity(Guid incidentId)
        {
            var activities = new IncidentActivities(incidentId);
            try
            {
                await _liveDataApi.SendEventData(activities);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"NotificationActivity incident: {incidentId}");
            }
        }

        private async Task NotificationIncidentList()
        {
            var eventData = new IncidentList();
            try
            {
                await _liveDataApi.SendEventData(eventData);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"NotificationIncidentList Update");
            }
        }

        private async Task<IncidentBriefDto> GetIncidentByWorkflowId(Guid workflowId)
        {
            var request = new GetIncidentByWorkflowIdRequestDto {WorkflowIds = new[] {workflowId}};
            var response = await _incidentMicroApi.GetIncidentByWorkflowIdAsync(request);
            ApiResponse.ThrowExceptionIfFailed(response);
            return response.Value.Incidents.First();
        }
    }
}
