using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
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
using Honeywell.Facade.Services.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Infra.Core.Common.Exceptions;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident;


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
    
        public async Task UpdateWorkflowStepStatus(string workflowStepId, bool isHandled)
        {
            Logger.LogInformation("call workflow design api UpdateWorkflowStepStatus Start");
            var workflowStepGuid = Guid.Parse(workflowStepId);

            var request = new UpdateWorkflowStepStatusRequestDto { WorkflowStepId = workflowStepGuid, IsHandled = isHandled };

            var response = await _workflowMicroApi.UpdateStepStatusAsync(request);
            ApiResponse.ThrowExceptionIfFailed(response);
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
            var response = await _incidentFacadeApi.GetDetailsAsync(new GetDetailRequestDto { IncidentIds = requestId });
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

            if (!Enum.TryParse<FacadeApi.Create.IncidentPriority>(request.Priority, true, out var priority))
            {
                var msg = $"wrong priority value: {request.Priority}";
                Logger.LogError(msg);
                throw new ArgumentException(msg);
            }

            var facadeRequest = new CreateIncidentRequestDto
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

            var response = await _incidentFacadeApi.CreateAsync(facadeRequest);

            ApiResponse.ThrowExceptionIfFailed(response);

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

            var request = new FacadeApi.Actions.IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentFacadeApi.RespondAsync(request);
            ApiResponse.ThrowExceptionIfFailed(response);
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

            var request = new FacadeApi.Actions.IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentFacadeApi.TakeoverAsync(request);
         
            ApiResponse.ThrowExceptionIfFailed(response);
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

            var request = new FacadeApi.Close.CloseIncidentRequestDto { IncidentId = incidentGuid, Reason = reason };

            var response = await _incidentFacadeApi.CloseAsync(request);

            ApiResponse.ThrowExceptionIfFailed(response);
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

            var request = new FacadeApi.Actions.IncidentActionRequestDto { IncidentId = incidentGuid };

            var response = await _incidentFacadeApi.CompleteAsync(request);

            ApiResponse.ThrowExceptionIfFailed(response);
        }

        public async Task<IncidentSummaryGto[]> GetActiveIncidentList()
        {
            Logger.LogInformation("call Incident api GetActiveIncidentList Start");
            var result = await _incidentMicroApi.GetListAsync();
            ApiResponse.ThrowExceptionIfFailed(result);

            var workflowIds = result.Value.List.Select(x => x.WorkflowId).ToArray();
            var request = new WorkflowSummaryRequestDto
            {
                WorkflowIds = workflowIds
            };
            var workflowSummaries = await _workflowMicroApi.GetSummariesAsync(request);
            ApiResponse.ThrowExceptionIfFailed(workflowSummaries);

            var activeIncidentsGto = HoneyMapper.Map<IncidentListItemDto[], IncidentSummaryGto[]>(result.Value.List.ToArray());
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

            return activeIncidentsGto;
        }

        public async Task<Guid[]> CreateIncidentByAlarm(CreateIncidentByAlarmRequestGto[] requests)
        {
            Logger.LogInformation($"call Incident api {nameof(CreateIncidentByAlarm)} Start");
            var facadeRequest =
                HoneyMapper.Map<CreateIncidentByAlarmRequestGto[], CreateIncidentByAlarmDto[]>(requests);

            var response = await _incidentFacadeApi.CreateByAlarmAsync(new CreateIncidentByAlarmRequestDto{CreateIncidentDatas = facadeRequest});
            ApiResponse.ThrowExceptionIfFailed(response);

            return response.Value.IncidentIds.ToArray();
        }

        public async Task<IncidentStatusInfoGto[]> GetIncidentStatusByAlarm(string[] alarmIds)
        {
            Logger.LogInformation($"call Incident api {nameof(GetIncidentStatusByAlarm)} Start");

            var response = await _incidentMicroApi.GetStatusByTriggerAsync(new GetIncidentStatusRequestDto{TriggerIds = alarmIds});
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper.Map<IncidentStatusDto[], IncidentStatusInfoGto[]>(response.Value.IncidentStatusInfos.ToArray());
            return result;
        }

        public async Task AddStepComment(AddStepCommentRequestGto addStepCommentGto)
        {
            Logger.LogInformation(
                $"call Incident api AddStepComment Start,workflowStepId:{addStepCommentGto.WorkflowStepId},comment:{addStepCommentGto.Comment}");

            AddStepCommentRequestDto requestDto = new AddStepCommentRequestDto
            {
                WorkflowStepId = Guid.Parse(addStepCommentGto.WorkflowStepId),
                Comment = addStepCommentGto.Comment
            };

            var response = await _workflowMicroApi.AddStepCommentAsync(requestDto);

            ApiResponse.ThrowExceptionIfFailed(response);
        }
    }
}
