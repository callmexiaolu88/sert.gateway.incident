using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Export;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Repository.WorkflowDesign
{
    public class WorkflowDesignRepository : ApplicationService, IWorkflowDesignRepository
    {
        private readonly IWorkflowDesignMicroApi _workflowDesignApi;

        public WorkflowDesignRepository(IWorkflowDesignMicroApi workflowDesignApi)
        {
            _workflowDesignApi = workflowDesignApi;
        }
        public async Task<ExecuteResult> ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            var result = new ExecuteResult();
            var responseDtoList = await _workflowDesignApi.ImportsAsync(workflowDesignStream);
            responseDtoList.Value.ImportResponseList.ForEach(x => result.ErrorList.AddRange(x.Errors));
            result.Status = responseDtoList.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
            Logger.LogError($"call workflow design api Imports error:{string.Join(",", result.ErrorList)}");
            return result;
        }

        public async Task<ExecuteResult> ValidatorWorkflowDesigns(Stream workflowDesignStream)
        {
            var result = new ExecuteResult();
            var responseDtoList = await _workflowDesignApi.ValidateAsync(workflowDesignStream);
            responseDtoList.Value.ImportResponseList.ForEach(x => result.ErrorList.AddRange(x.Errors));
            result.Status = responseDtoList.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
            Logger.LogError($"call workflow design api Imports error:{string.Join(",", result.ErrorList)}");
            return result;
        }

        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var guidList = workflowDesignIds.Select(Guid.Parse).ToArray();
            var result = await _workflowDesignApi.DeletesAsync(new WorkflowDesignDeleteRequestDto { Ids = guidList });
            if (result.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            Logger.LogError($"call workflow design api Deletes error:{result.Messages?.FirstOrDefault()?.Message}");
            return new ExecuteResult
            {
                Status = ExecuteStatus.Error,
                ErrorList = result.Messages?.Select(item => item.Message).ToList()
            };
        }

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            var result = await _workflowDesignApi.GetSummariesAsync();
            if (result.IsSuccess)
            {
                return HoneyMapper.Map<WorkflowDesignSummaryDto[],
                    WorkflowDesignSummaryGto[]>(result.Value.Summaries.ToArray());
            }

            Logger.LogError($"call workflow design api GetSummaries error:{result.Messages?.FirstOrDefault()?.Message}");
            return new WorkflowDesignSummaryGto[] { };
        }

        public async Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors()
        {
            var result = await _workflowDesignApi.GetSelectorAsync();
            if (!result.IsSuccess)
            {
                Logger.LogError($"call workflow design api GetSelector error:{result.Messages?.FirstOrDefault()?.Message}");
                return new WorkflowDesignSelectorListGto { Status = ExecuteStatus.Error };
            }

            return new WorkflowDesignSelectorListGto
            {
                Status = ExecuteStatus.Successful,
                List = HoneyMapper.Map<WorkflowDesignSelectorDto[],
                    WorkflowDesignSelectorGto[]>(result.Value.Selectors.ToArray()).ToList()
            };
        }

        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            var requestId = new[] { Guid.Parse(workflowDesignId) };
            var result = await _workflowDesignApi.GetDetailsAsync(new WorkflowDesignDetailsRequestDto { Ids = requestId });
            if (result.IsSuccess)
            {
                var workflowDesignList = HoneyMapper.Map<WorkflowDesignDto[], WorkflowDesignGto[]>(result.Value.Details.ToArray());
                return workflowDesignList.FirstOrDefault();
            }

            Logger.LogError($"call workflow design api GetDetails error:{result.Messages?.FirstOrDefault()?.Message}");
            return null;
        }

        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            Logger.LogInformation("call workflow design api DownloadWorkflowTemplate Start");
            var result = await _workflowDesignApi.DownloadTemplateAsync();

            if (result.IsSuccess)
            {
                var status = result.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
                WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(status, result.Value.FileName, result.Value.FileBytes);
                return workflowDownloadTemplateGto;
            }

            Logger.LogError($"call workflow design api DownloadWorkflowTemplate error:|{result.Messages?.FirstOrDefault()?.Message}");
            return new WorkflowTemplateGto { Status = ExecuteStatus.Error, FileBytes = new byte[0] };
        }

        public async Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            Logger.LogInformation(
                $"call workflow design api ExportWorkflows Start|workflowId.Length:{workflowIds.Length},guids:{string.Join(",", workflowIds.ToArray())}");
            Guid[] guidWorkflowIds = workflowIds.Select(o => Guid.Parse(o)).ToArray();
            var exportWorkflowRequestDto = new ExportWorkflowRequestDto() { WorkflowIds = guidWorkflowIds };
            var result = await _workflowDesignApi.ExportAsync(exportWorkflowRequestDto);

            if (result.IsSuccess)
            {
                var status = result.IsSuccess ? ExecuteStatus.Successful : ExecuteStatus.Error;
                WorkflowTemplateGto workflowDownloadTemplateGto = new WorkflowTemplateGto(status, result.Value.WorkflowsBytes);
                return workflowDownloadTemplateGto;
            }

            Logger.LogError($"call workflow design api ExportWorkflowDesigns error:{result.Messages?.FirstOrDefault()?.Message}");
            return new WorkflowTemplateGto { Status = ExecuteStatus.Error, FileBytes = new byte[0] };
        }

        public async Task<GetIdsResponseGto> GetWorkflowDesignIds()
        {
            Logger.LogInformation($"call Incident api {nameof(GetWorkflowDesignIds)} Start");

            var response = await _workflowDesignApi.GetSummariesAsync();
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper
                .Map<WorkflowDesignSummaryResponseDto, GetIdsResponseGto>(response.Value);
            return result;
        }

        public async Task<GetDetailsResponseGto> GetWorkflowDesignDetails(GetDetailsRequestGto request)
        {
            Logger.LogInformation($"call Incident api {nameof(GetWorkflowDesignDetails)} Start");

            var workflowDesignRequest =
                HoneyMapper.Map<GetDetailsRequestGto, WorkflowDesignDetailsRequestDto>(request);
            var response = await _workflowDesignApi.GetDetailsAsync(workflowDesignRequest);
            ApiResponse.ThrowExceptionIfFailed(response);

            var result = HoneyMapper.Map<WorkflowDesignResponseDto, GetDetailsResponseGto>(response.Value);
            return result;
        }
    }
}
