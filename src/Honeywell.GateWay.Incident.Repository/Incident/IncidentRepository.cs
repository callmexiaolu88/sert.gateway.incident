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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident;

namespace Honeywell.GateWay.Incident.Repository.Incident
{

    public class IncidentRepository : ApplicationService, IIncidentRepository
    {
        private readonly IWorkflowDesignApi _workflowDesignApi;
        private readonly IIncidentMicroApi _incidentMicroApi;
        private readonly IWorkflowInstanceApi _workflowInstanceApi;
        private readonly IIncidentFacadeApi _incidentFacadeApi;

        public IncidentRepository(IWorkflowDesignApi workflowDesignApi,
            IIncidentMicroApi incidentMicroApi,
            IWorkflowInstanceApi workflowInstanceApi,
            IIncidentFacadeApi incidentFacadeApi)
        {
            _workflowDesignApi = workflowDesignApi;
            _incidentMicroApi = incidentMicroApi;
            _workflowInstanceApi = workflowInstanceApi;
            _incidentFacadeApi = incidentFacadeApi;
        }
        public async Task<ExecuteResult> ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            var result = new ExecuteResult();
            var responseDtoList = await _workflowDesignApi.Imports(workflowDesignStream);
            responseDtoList.ImportResponseList.ForEach(x => result.ErrorList.AddRange(x.Errors));
            result.Status = responseDtoList.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
            Logger.LogError($"call workflow design api Imports error:{string.Join(",", result.ErrorList)}");
            return result;
        }

        public async Task<ExecuteResult> ValidatorWorkflowDesigns(Stream workflowDesignStream)
        {
            var result = new ExecuteResult();
            var responseDtoList = await _workflowDesignApi.Validate(workflowDesignStream);
            responseDtoList.ImportResponseList.ForEach(x => result.ErrorList.AddRange(x.Errors));
            result.Status = responseDtoList.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
            Logger.LogError($"call workflow design api Imports error:{string.Join(",", result.ErrorList)}");
            return result;
        }

        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var guidList = workflowDesignIds.Select(Guid.Parse).ToArray();
            var result = await _workflowDesignApi.Deletes(new WorkflowDesignDeleteRequestDto { Ids = guidList });
            if (result.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError($"call workflow design api Deletes error:{result.Message}");
            return new ExecuteResult
            {
                Status = ExecuteStatus.Error,
                ErrorList = new List<string> { result.Message }
            };
        }

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            var result = await _workflowDesignApi.GetSummaries();
            if (result.IsSuccess)
            {
                return HoneyMapper.Map<WorkflowDesignSummaryDto[],
                    WorkflowDesignSummaryGto[]>(result.Summaries.ToArray());
            }

            Logger.LogError($"call workflow design api GetSummaries error:{result.Message}");
            return new WorkflowDesignSummaryGto[] { };
        }

        public async Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectorsByName(string workflowName)
        {
            var workflowDesignSelectorRequestDto = new WorkflowDesignSelectorRequestDto();

            if (string.IsNullOrEmpty(workflowName))
            {
                workflowName = "";
            }
            workflowDesignSelectorRequestDto.WorkflowName = workflowName;
            var result = await _workflowDesignApi.GetSelector(workflowDesignSelectorRequestDto);
            if (result.IsSuccess)
            {
                return HoneyMapper.Map<WorkflowDesignSelectorDto[],
                    WorkflowDesignSelectorGto[]>(result.Selectors.ToArray());
            }

            Logger.LogError($"call workflow design api GetSelectors error:{result.Message}");
            return new WorkflowDesignSelectorGto[] { };
        }

        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            var requestId = new[] { Guid.Parse(workflowDesignId) };
            var result = await _workflowDesignApi.GetDetails(new WorkflowDesignDetailsRequestDto { Ids = requestId });
            if (result.IsSuccess)
            {
                var workflowDesignList = HoneyMapper.Map<WorkflowDesignDto[], WorkflowDesignGto[]>(result.Details.ToArray());
                return workflowDesignList.FirstOrDefault();
            }
            Logger.LogError($"call workflow design api GetDetails error:{result.Message}");
            return null;
        }

        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            Logger.LogInformation("call workflow design api DownloadWorkflowTemplate Start");
            var result = await _workflowDesignApi.DownloadTemplate();

            if (result.IsSuccess)
            {
                var status = result.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
                WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(status, result.FileName, result.FileBytes);
                return workflowDownloadTemplateGto;
            }

            Logger.LogError($"call workflow design api DownloadWorkflowTemplate error:|{result.Message}");
            return new WorkflowTemplateGto() { Status = ExecuteStatus.Error, FileBytes = new byte[0] };
        }

        public async Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            Logger.LogInformation(string.Format("call workflow design api ExportWorkflows Start|workflowId.Length:{0},guids:{1}", workflowIds.Length, string.Join(",", workflowIds.ToArray())));
            Guid[] guidWorkflowIds = workflowIds.Select(o => Guid.Parse(o)).ToArray();
            var exportWorkflowRequestDto = new ExportWorkflowRequestDto() { WorkflowIds = guidWorkflowIds };
            var result = await _workflowDesignApi.ExportWorkflows(exportWorkflowRequestDto);

            if (result.IsSuccess)
            {
                var status = result.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
                WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(status, result.WorkflowsBytes);
                return workflowDownloadTemplateGto;
            }

            Logger.LogError($"call workflow design api ExportWorkflowDesigns error:{result.Message}");
            return new WorkflowTemplateGto() { Status = ExecuteStatus.Error, FileBytes = new byte[0] };
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
            var response = await _incidentFacadeApi.GetDetails(new GetDetailRequestDto { IncidentIds = requestId });
            if (!response.IsSuccess)
            {
                result.ErrorList.Add(response.Message);
                return await Task.FromResult(result);
            }

            HoneyMapper.Map(response.Details[0], result);
            result.Status = ExecuteStatus.Successful;
            return await Task.FromResult(result);

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

            var response = await _incidentFacadeApi.CreateIncident(facadeRequest);

            if (response.IsSuccess)
            {
                return response.IncidentIds.First().ToString();
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
            
            var request = new FacadeApi.Respond.RespondIncidentRequestDto() { IncidentId = incidentGuid };

            var response = await _incidentFacadeApi.RespondIncident(request);
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

            var request = new FacadeApi.Takeover.TakeoverIncidentRequestDto() { IncidentId = incidentGuid };
            var response = await _incidentFacadeApi.TakeoverIncident(request);
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
            var response = await _incidentFacadeApi.CloseIncident(request);
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

            var request = new FacadeApi.Complete.CompleteIncidentRequestDto() { IncidentId = incidentGuid };
            var response = await _incidentFacadeApi.CompleteIncident(request);
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
            var result = await _incidentMicroApi.GetActiveList();
            if (!result.IsSuccess)
            {
                Logger.LogError($"call workflow design api GetActiveList error:{result.Message}");
                return new ActiveIncidentListGto() { Status = ExecuteStatus.Error };
            }

            var workflowIds = result.List.Select(x => x.WorkflowId).ToArray();
            var request = new WorkflowSummaryRequestDto
            {
                WorkflowIds = workflowIds
            };
            var workflowSummaries = await _workflowInstanceApi.GetWorkflowSummaries(request);
            if (!workflowSummaries.IsSuccess)
            {
                Logger.LogError($"call workflow design api GetWorkflowSummaries error:{result.Message}");
                return new ActiveIncidentListGto() { Status = ExecuteStatus.Error };
            }

            var activeIncidentsGto = HoneyMapper.Map<IncidentListItemDto[], ActiveIncidentGto[]>(result.List.ToArray());
            foreach (var activeIncident in activeIncidentsGto)
            {
                foreach (var workflowSummary in workflowSummaries.Summaries)
                {
                    if (activeIncident.WorkflowId == workflowSummary.WorkflowId)
                    {
                        activeIncident.WorkflowDesignName = workflowSummary.WorkflowDesignName;
                        activeIncident.CompletedSteps = workflowSummary.CompletedSteps;
                        activeIncident.TotalSteps = workflowSummary.TotalSteps;
                    }
                }
            }

            return new ActiveIncidentListGto()
            {
                Status = ExecuteStatus.Successful,
                List = activeIncidentsGto.ToList()
            };
        }
    }
}
