using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.CreateIncident;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.Incident
{
    public class IncidentAppService :
        ApplicationService,
        IIncidentAppService
    {
        private readonly IWorkflowDesignApi _workflowDesignApi;
        private readonly IIncidentMicroApi _incidentMicroApi;
        private readonly IWorkflowInstanceApi _workflowInstanceApi;
        private readonly IIncidentFacadeApi _incidentFacadeApi;
        private readonly IProwatchRepository _prowatchRepository;

        public IncidentAppService(IWorkflowDesignApi workflowDesignApi,
            IIncidentMicroApi incidentMicroApi,
            IWorkflowInstanceApi workflowInstanceApi, IIncidentFacadeApi incidentFacadeApi,
            IProwatchRepository prowatchRepository)
        {
            _workflowDesignApi = workflowDesignApi;
            _incidentMicroApi = incidentMicroApi;
            _workflowInstanceApi = workflowInstanceApi;
            _incidentFacadeApi = incidentFacadeApi;
            _prowatchRepository = prowatchRepository;
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
            WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(
                result.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error, result.FileName, result.FileBytes);
            return workflowDownloadTemplateGto;
        }

        public async Task<IncidentGto> GetIncidentById(string incidentId)
        {

            Logger.LogInformation("call Incident api GetIncidentById Start");
            var result = new IncidentGto();
            var requestId = new[] { Guid.Parse(incidentId) };
            var incidentResponse = await _incidentMicroApi.GetDetails(new GetIncidentDetailsRequestDto { Ids = requestId });
            if (!incidentResponse.IsSuccess)
            {
                result.ErrorList.Add(incidentResponse.Message);
                return await Task.FromResult(result);
            }
            var incidentGto = HoneyMapper.Map<IncidentDto, IncidentGto>(incidentResponse.Details[0]);

            var workflowResponse = await _workflowInstanceApi.GetWorkflowDetails(new WorkflowDetailsRequestDto { Ids = incidentResponse.Details.Select(m => m.WorkflowId).ToArray() });
            if (!workflowResponse.IsSuccess)
            {
                result.ErrorList.Add(workflowResponse.Message);
                return await Task.FromResult(result);
            }
            HoneyMapper.Map(workflowResponse.Details[0], incidentGto);
            HoneyMapper.Map(workflowResponse.Details[0].WorkflowSteps, incidentGto.IncidentSteps);
            result.Status = ExecuteStatus.Successful;
            return await Task.FromResult(incidentGto);
        }

        public async Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            Logger.LogInformation("call Incident api CreateIncident Start");
            if (!Guid.TryParse(request.WorkflowDesignReferenceId, out var workflowDesignReferenceId))
            {
                Logger.LogError($"wrong WorkflowDesignReferenceId value: {request.WorkflowDesignReferenceId}");
                return string.Empty;
            }

            if (!Enum.TryParse<IncidentPriority>(request.Priority, true, out var priority))
            {
                Logger.LogError($"wrong priority value: {request.Priority}");
                return string.Empty;
            }

            var facadeRequest = new CreateIncidentRequestDto
            {
                WorkflowDesignReferenceId = workflowDesignReferenceId,
                Priority = priority,
                Description = request.Description
            };

            var response = await _incidentFacadeApi.CreateIncident(facadeRequest);

            if (response.IsSuccess)
            {
                return response.IncidentId.ToString();
            }

            Logger.LogError("Failed to create incident!");
            return string.Empty;
        }

        public async Task<ProwatchDeviceGto[]> GetProwatchDevices()
        {
            Logger.LogInformation("call Incident api GetProwatchDeviceList Start");
            var result = await _prowatchRepository.GetDevices();
            var devices = result.Config.Select(x => new ProwatchDeviceGto
            {
                SiteId = x.Relation[0].Id,
                SiteName = x.Relation[0].EntityId,
                DeviceId = x.Identifiers.Id,
                DeviceDisplayName =  x.Identifiers.Name,
                DeviceType = x.Type
            });

            Logger.LogInformation("call Incident api GetProwatchDeviceList end");
            return devices.ToArray();
        }

        private IncidentPriority ConvertPriority(string priority)
        {
            return (IncidentPriority) Enum.Parse(typeof(IncidentPriority), priority);
        }
    }
}
