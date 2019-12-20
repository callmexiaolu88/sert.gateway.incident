using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Export;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetIds;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Update;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Create;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.List;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Update;

namespace Honeywell.GateWay.Incident.Repository.WorkflowDesign
{
    public class WorkflowDesignRepository : ApplicationService, IWorkflowDesignRepository
    {
        private readonly IWorkflowDesignMicroApi _workflowDesignApi;
        private readonly string DefaultDescriptionEmptyShowText = "--";

        public WorkflowDesignRepository(IWorkflowDesignMicroApi workflowDesignApi)
        {
            _workflowDesignApi = workflowDesignApi;
        }
        public async Task<CreateWorkflowDesignResponseGto> CreateWorkflowDesign(CreateWorkflowDesignRequestGto createWorkflowDesignRequestGto)
        {
            if (createWorkflowDesignRequestGto == null)
            {
                throw new ArgumentNullException(nameof(createWorkflowDesignRequestGto));
            }

            var createWorkflowDesignRequestDto = new CreateWorkflowDesignRequestDto
            {
                Description = createWorkflowDesignRequestGto.Description,
                Name = createWorkflowDesignRequestGto.Name
            };
            foreach (var step in createWorkflowDesignRequestGto.Steps)
            {
                var createWorkflowStepDesignDto = new CreateWorkflowStepDesignDto
                {
                    IsOptional = step.IsOptional,
                    Instruction = step.Instruction,
                    HelpText = step.HelpText
                };
                createWorkflowDesignRequestDto.Steps.Add(createWorkflowStepDesignDto);
            }

            var responseDtoList = await _workflowDesignApi.CreateAsync(createWorkflowDesignRequestDto);
            ApiResponse.ThrowExceptionIfFailed(responseDtoList);

            return new CreateWorkflowDesignResponseGto { Id = responseDtoList.Value.Id, WorkflowDesignName = responseDtoList.Value.WorkflowDesignName };
        }

        public async Task<UpdateWorkflowDesignResponseGto> UpdateWorkflowDesign(UpdateWorkflowDesignRequestGto updateWorkflowDesignRequestGto)
        {
            if (updateWorkflowDesignRequestGto == null)
            {
                throw new ArgumentNullException(nameof(updateWorkflowDesignRequestGto));
            }

            var updateWorkflowDesignRequestDto = new UpdateWorkflowDesignRequestDto
            {
                Description = updateWorkflowDesignRequestGto.Description,
                Name = updateWorkflowDesignRequestGto.Name,
                Id = updateWorkflowDesignRequestGto.Id
            };
            foreach (var step in updateWorkflowDesignRequestGto.Steps)
            {
                var createWorkflowStepDesignDto = new UpdateWorkflowStepDesignDto
                {
                    IsOptional = step.IsOptional,
                    Instruction = step.Instruction,
                    HelpText = step.HelpText
                };
                updateWorkflowDesignRequestDto.Steps.Add(createWorkflowStepDesignDto);
            }

            var responseDtoList = await _workflowDesignApi.UpdateAsync(updateWorkflowDesignRequestDto);
            ApiResponse.ThrowExceptionIfFailed(responseDtoList);
            return new UpdateWorkflowDesignResponseGto { Id = responseDtoList.Value.Id, WorkflowDesignName = responseDtoList.Value.WorkflowDesignName };
        }

        public async Task ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            var responseDtoList = await _workflowDesignApi.ImportsAsync(workflowDesignStream);
            ApiResponse.ThrowExceptionIfFailed(responseDtoList);
        }

        public async Task<ApiResponse> ValidatorWorkflowDesigns(Stream workflowDesignStream)
        {
            var responseDtoList = await _workflowDesignApi.ValidateAsync(workflowDesignStream);
            if (responseDtoList.IsSuccess)
            {
                return ApiResponse.CreateSuccess(responseDtoList.Value.ImportResponseList[0].Errors[0]);
            }

            var result = ApiResponse.CreateFailed();
            var messages = responseDtoList.Value.ImportResponseList.SelectMany(responseDto => responseDto.Errors);
            foreach (var message in messages)
            {
                result.MakeFrom(message, true);
            }

            Logger.LogError($"call workflow design api Imports error:{string.Join(",", result.Messages.Select(x => x.Message))}");

            return result;
        }

        public async Task DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var guidList = workflowDesignIds.Select(Guid.Parse).ToArray();
            var result = await _workflowDesignApi.DeletesAsync(new WorkflowDesignDeleteRequestDto { Ids = guidList });
            ApiResponse.ThrowExceptionIfFailed(result);
        }

        public async Task<WorkflowDesignListGto[]> GetWorkflowDesignList(string condition)
        {
            var isDescriptionEmptyIncluded =
                !string.IsNullOrEmpty(condition) && DefaultDescriptionEmptyShowText.Contains(condition);
            var workflowDesignListRequestDto = new WorkflowDesignListRequestDto
            { Condition = condition, IsDescriptionEmptyIncluded = isDescriptionEmptyIncluded };

            var result = await _workflowDesignApi.GetListAsync(workflowDesignListRequestDto);

            ApiResponse.ThrowExceptionIfFailed(result);

            return HoneyMapper.Map<WorkflowDesignListDto[], WorkflowDesignListGto[]>(result.Value.Lists.ToArray());
        }

        public async Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectors()
        {
            var result = await _workflowDesignApi.GetSelectorAsync();

            ApiResponse.ThrowExceptionIfFailed(result);

            return HoneyMapper
                .Map<WorkflowDesignSelectorDto[], WorkflowDesignSelectorGto[]>(result.Value.Selectors.ToArray());
        }

        public async Task<WorkflowDesignDetailGto> GetWorkflowDesignById(string workflowDesignId)
        {
            var requestId = new[] { Guid.Parse(workflowDesignId) };

            var result = await _workflowDesignApi.GetDetailsAsync(new WorkflowDesignDetailsRequestDto { Ids = requestId });

            ApiResponse.ThrowExceptionIfFailed(result);

            var workflowDesignList = HoneyMapper.Map<WorkflowDesignDto[], WorkflowDesignDetailGto[]>(result.Value.Details.ToArray());

            return workflowDesignList.FirstOrDefault();
        }

        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            Logger.LogInformation("call workflow design api DownloadWorkflowTemplate Start");

            var result = await _workflowDesignApi.DownloadTemplateAsync();

            ApiResponse.ThrowExceptionIfFailed(result);

            WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(result.Value.FileName, result.Value.FileBytes);

            return workflowDownloadTemplateGto;
        }

        public async Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            if (workflowIds == null)
            {
                throw new ArgumentNullException(nameof(workflowIds));
            }
            Logger.LogInformation(
                $"call workflow design api ExportWorkflows Start|workflowId.Length:{workflowIds.Length},guids:{string.Join(",", workflowIds.ToArray())}");
            Guid[] guidWorkflowIds = workflowIds.Select(o => Guid.Parse(o)).ToArray();
            var exportWorkflowRequestDto = new ExportWorkflowRequestDto { WorkflowIds = guidWorkflowIds };
            var result = await _workflowDesignApi.ExportAsync(exportWorkflowRequestDto);

            ApiResponse.ThrowExceptionIfFailed(result);
            WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(result.Value.WorkflowsBytes);
            return workflowDownloadTemplateGto;
        }

        public async Task<WorkflowDesignIdGto[]> GetWorkflowDesignIds()
        {
            Logger.LogInformation($"call Incident api {nameof(GetWorkflowDesignIds)} Start");

            var workflowDesignListRequestDto = new WorkflowDesignListRequestDto { Condition = string.Empty };
            var response = await _workflowDesignApi.GetListAsync(workflowDesignListRequestDto);
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper
                .Map<WorkflowDesignListDto[], WorkflowDesignIdGto[]>(response.Value.Lists.ToArray());
            return result;
        }

        public async Task<WorkflowDesignDetailGto[]> GetWorkflowDesignDetails(Guid[] workflowDesignIds)
        {
            Logger.LogInformation($"call Incident api {nameof(GetWorkflowDesignDetails)} Start");

            var workflowDesignRequest = new WorkflowDesignDetailsRequestDto { Ids = workflowDesignIds };
            var response = await _workflowDesignApi.GetDetailsAsync(workflowDesignRequest);
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper.Map<WorkflowDesignDto[], WorkflowDesignDetailGto[]>(response.Value.Details.ToArray());
            return result;
        }
    }
}
