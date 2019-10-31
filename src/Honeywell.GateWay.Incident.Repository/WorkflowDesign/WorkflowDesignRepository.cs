using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
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
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;

namespace Honeywell.GateWay.Incident.Repository.WorkflowDesign
{
    public class WorkflowDesignRepository : ApplicationService, IWorkflowDesignRepository
    {
        private readonly IWorkflowDesignMicroApi _workflowDesignApi;

        public WorkflowDesignRepository(IWorkflowDesignMicroApi workflowDesignApi)
        {
            _workflowDesignApi = workflowDesignApi;
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

            var errors = responseDtoList.Value.ImportResponseList.SelectMany(x => x.Errors).Select(x => new MessageInfo(x, true));
            var result = ApiResponse.CreateFailed();
            result.Messages.Clear();
            foreach(var importResponse in responseDtoList.Value.ImportResponseList)
            {
                foreach(var error in importResponse.Errors)
                {
                    var message = new MessageInfo(error, true);
                    result.Messages.Add(message);
                    result.MakeFrom();
                }
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

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            var result = await _workflowDesignApi.GetSummariesAsync();

            ApiResponse.ThrowExceptionIfFailed(result);

            return HoneyMapper.Map<WorkflowDesignSummaryDto[], WorkflowDesignSummaryGto[]>(result.Value.Summaries.ToArray());
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
            Logger.LogInformation(
                $"call workflow design api ExportWorkflows Start|workflowId.Length:{workflowIds.Length},guids:{string.Join(",", workflowIds.ToArray())}");
            Guid[] guidWorkflowIds = workflowIds.Select(o => Guid.Parse(o)).ToArray();
            var exportWorkflowRequestDto = new ExportWorkflowRequestDto() { WorkflowIds = guidWorkflowIds };
            var result = await _workflowDesignApi.ExportAsync(exportWorkflowRequestDto);

            ApiResponse.ThrowExceptionIfFailed(result);
            WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(result.Value.WorkflowsBytes);
            return workflowDownloadTemplateGto;
        }

        public async Task<WorkflowDesignIdGto[]> GetWorkflowDesignIds()
        {
            Logger.LogInformation($"call Incident api {nameof(GetWorkflowDesignIds)} Start");

            var response = await _workflowDesignApi.GetSummariesAsync();
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper
                .Map<WorkflowDesignSummaryDto[], WorkflowDesignIdGto[]>(response.Value.Summaries.ToArray());
            return result;
        }

        public async Task<WorkflowDesignDetailGto[]> GetWorkflowDesignDetails(Guid[] workflowDesignIds)
        {
            Logger.LogInformation($"call Incident api {nameof(GetWorkflowDesignDetails)} Start");

            var workflowDesignRequest = new WorkflowDesignDetailsRequestDto {Ids = workflowDesignIds};
            var response = await _workflowDesignApi.GetDetailsAsync(workflowDesignRequest);
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper.Map<WorkflowDesignDto[], WorkflowDesignDetailGto[]>(response.Value.Details.ToArray());
            return result;
        }
    }
}
