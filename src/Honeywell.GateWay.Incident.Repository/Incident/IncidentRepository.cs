using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Export;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Actions;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident;
using Honeywell.Micro.Services.Workflow.Api.Workflow.AddComment;

#pragma warning disable CS0612 // Type or member is obsolete

namespace Honeywell.GateWay.Incident.Repository.Incident
{

    public class IncidentRepository : ApplicationService, IIncidentRepository
    {
        private readonly IWorkflowDesignMicroApi _workflowDesignApi;
        private readonly IIncidentMicroApi _incidentMicroApi;
        private readonly IWorkflowMicroApi _workflowMicroApi;
        private readonly IIncidentFacadeApi _incidentFacadeApi;

        public IncidentRepository(IWorkflowDesignMicroApi workflowDesignApi,
            IIncidentMicroApi incidentMicroApi,
            IWorkflowMicroApi workflowMicroApi,
            IIncidentFacadeApi incidentFacadeApi)
        {
            _workflowDesignApi = workflowDesignApi;
            _incidentMicroApi = incidentMicroApi;
            _workflowMicroApi = workflowMicroApi;
            _incidentFacadeApi = incidentFacadeApi;
        }
    
        public async Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled)
        {
            Logger.LogInformation("call workflow design api UpdateWorkflowStepStatus Start");
            var workflowStepGuid = Guid.Parse(workflowStepId);

            var request = new UpdateWorkflowStepStatusRequestDto { WorkflowStepId = workflowStepGuid, IsHandled = isHandled };

            var response = await _workflowMicroApi.UpdateStepStatusAsync(request);
            if (response.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError("Failed to update workflow stepStatus!");
            var result = ExecuteResult.Error;
            result.ErrorList.AddRange(response.Messages.Select(item => item.Message));
            return result;
        }

        public async Task<IncidentGto> GetIncidentById(string incidentId)
        {
            Logger.LogInformation("call Incident api GetIncidentById Start");
            if (!Guid.TryParse(incidentId, out var guid))
            {
                throw new ArgumentException("incidentId is invalid", nameof(incidentId));
            }
            var result = new IncidentGto();
            var requestId = new[] { guid };
            var response = await _incidentFacadeApi.GetDetailsAsync(new GetDetailRequestDto { IncidentIds = requestId });
            if (!response.IsSuccess)
            {
                result.ErrorList.AddRange(response.Messages.Select(item=>item.Message));
                return result;
            }

            HoneyMapper.Map(response.Value.Details[0], result);
            result.Status = ExecuteStatus.Successful;
            return result;
        }

        public async Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            Logger.LogInformation("call Incident api CreateIncident Start");
            if (!Guid.TryParse(request.WorkflowDesignReferenceId, out var workflowDesignReferenceId))
            {
                Logger.LogError($"wrong WorkflowDesignReferenceId value: {request.WorkflowDesignReferenceId}");
                return string.Empty;
            }

            if (!Enum.TryParse<FacadeApi.Create.IncidentPriority>(request.Priority, true, out var priority))
            {
                Logger.LogError($"wrong priority value: {request.Priority}");
                return string.Empty;
            }

            var facadeRequest = new FacadeApi.Create.CreateIncidentRequestDto
            {
                CreateIncidentDatas = new[]
                {
                    new FacadeApi.Create.CreateIncidentDataDto
                    {
                        WorkflowDesignReferenceId = workflowDesignReferenceId,
                        Priority = priority,
                        Description = request.Description,
                        DeviceId = request.DeviceId,
                        DeviceType = request.DeviceType
                    }
                }
            };

            var response = await _incidentFacadeApi.CreateAsync(facadeRequest);

            if (response.IsSuccess)
            {
                return response.Value.IncidentIds.First().ToString();
            }

            Logger.LogError("Failed to create incident!");
            return string.Empty;
        }

        public async Task<ExecuteResult> RespondIncident(string incidentId)
        {
            Logger.LogInformation("call Incident api RespondIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                Logger.LogError($"wrong incident id: {incidentId}");
                return ExecuteResult.Error;
            }

            var request = new FacadeApi.Actions.IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentFacadeApi.RespondAsync(request);
            if (response.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError("Failed to respond incident!");
            return ExecuteResult.Error;
        }

        public async Task<ExecuteResult> TakeoverIncident(string incidentId)
        {
            Logger.LogInformation("call Incident api TakeoverIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                Logger.LogError($"wrong incident id: {incidentId}");
                return ExecuteResult.Error;
            }

            var request = new FacadeApi.Actions.IncidentActionRequestDto { IncidentId = incidentGuid };
            var response = await _incidentFacadeApi.TakeoverAsync(request);
            if (response.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError("Failed to takeover incident!");
            return ExecuteResult.Error;
        }

        public async Task<ExecuteResult> CloseIncident(string incidentId, string reason)
        {
            Logger.LogInformation("call Incident api CloseIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                Logger.LogError($"wrong incident id: {incidentId}");
                return ExecuteResult.Error;
            }

            var request = new FacadeApi.Close.CloseIncidentRequestDto { IncidentId = incidentGuid, Reason = reason };
            var response = await _incidentFacadeApi.CloseAsync(request);
            if (response.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError("Failed to close incident!");
            return ExecuteResult.Error;
        }

        public async Task<ExecuteResult> CompleteIncident(string incidentId)
        {
            Logger.LogInformation("call Incident api CompleteIncident Start");
            if (!Guid.TryParse(incidentId, out var incidentGuid))
            {
                Logger.LogError($"wrong incident id: {incidentId}");
                return ExecuteResult.Error;
            }

            var request = new FacadeApi.Actions.IncidentActionRequestDto { IncidentId = incidentGuid };
            var response = await _incidentFacadeApi.CompleteAsync(request);
            if (response.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError("Failed to complete incident!");
            return ExecuteResult.Error;
        }

        public async Task<ActiveIncidentListGto> GetActiveIncidentList()
        {
            Logger.LogInformation("call Incident api GetActiveIncidentList Start");
            var result = await _incidentMicroApi.GetListAsync();
            if (!result.IsSuccess)
            {
                Logger.LogError($"call workflow design api GetActiveList error:{result.Messages?.FirstOrDefault()?.Message}");
                return new ActiveIncidentListGto { Status = ExecuteStatus.Error };
            }

            var workflowIds = result.Value.List.Select(x => x.WorkflowId).ToArray();
            var request = new WorkflowSummaryRequestDto
            {
                WorkflowIds = workflowIds
            };
            var workflowSummaries = await _workflowMicroApi.GetSummariesAsync(request);
            if (!workflowSummaries.IsSuccess)
            {
                Logger.LogError($"call workflow design api GetWorkflowSummaries error:{result.Messages?.FirstOrDefault()?.Message}");
                return new ActiveIncidentListGto { Status = ExecuteStatus.Error };
            }

            var activeIncidentsGto = HoneyMapper.Map<IncidentListItemDto[], ActiveIncidentGto[]>(result.Value.List.ToArray());
            foreach (var activeIncident in activeIncidentsGto)
            {
                foreach (var workflowSummary in workflowSummaries.Value.Summaries)
                {
                    if (activeIncident.WorkflowId == workflowSummary.WorkflowId)
                    {
                        activeIncident.WorkflowDesignName = workflowSummary.WorkflowDesignName;
                        activeIncident.CompletedSteps = workflowSummary.CompletedSteps;
                        activeIncident.TotalSteps = workflowSummary.TotalSteps;
                    }
                }
            }

            return new ActiveIncidentListGto
            {
                Status = ExecuteStatus.Successful,
                List = activeIncidentsGto.ToList()
            };
        }

        public async Task<CreateIncidentResponseGto> CreateIncidentByAlarm(CreateByAlarmRequestGto request)
        {
            Logger.LogInformation($"call Incident api {nameof(CreateIncidentByAlarm)} Start");
            var facadeRequest =
                HoneyMapper.Map<CreateByAlarmRequestGto, FacadeApi.Create.CreateIncidentByAlarmRequestDto>(
                    request);
            var response = await _incidentFacadeApi.CreateByAlarmAsync(facadeRequest);
            ApiResponse.ThrowExceptionIfFailed(response);

            var responseValue = HoneyMapper
                  .Map<FacadeApi.Create.CreateIncidentResponseDto, CreateIncidentResponseGto>(response.Value);
            return responseValue;
        }

        public async Task<GetStatusByAlarmResponseGto> GetIncidentStatusByAlarm(
            GetStatusByAlarmRequestGto request)
        {
            Logger.LogInformation($"call Incident api {nameof(GetIncidentStatusByAlarm)} Start");

            var incidentRequest =
                HoneyMapper.Map<GetStatusByAlarmRequestGto, GetIncidentStatusRequestDto>(request);
            var response = await _incidentMicroApi.GetStatusByTriggerAsync(incidentRequest);
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper.Map<GetIncidentStatusResponseDto, GetStatusByAlarmResponseGto>(response.Value);
            return result;
        }

        public async Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto)
        {
            Logger.LogInformation(
                $"call Incident api AddStepComment Start,workflowStepId:{addStepCommentGto.WorkflowStepId},comment:{addStepCommentGto.Comment}");

            AddStepCommentRequestDto requestDto = new AddStepCommentRequestDto
            {
                WorkflowStepId = Guid.Parse(addStepCommentGto.WorkflowStepId),
                Comment = addStepCommentGto.Comment
            };
            var response = await _workflowMicroApi.AddStepCommentAsync(requestDto);

            if (response.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError("Failed to AddStepComment!");
            return ExecuteResult.Error;
        }
    }
}
