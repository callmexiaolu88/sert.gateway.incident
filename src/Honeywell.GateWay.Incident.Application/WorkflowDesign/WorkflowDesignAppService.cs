using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.WorkflowDesign
{
    public class WorkflowDesignAppService :
        ApplicationService,
        IWorkflowDesignAppService
    {
        private readonly IWorkflowDesignApi _workflowDesignApi;

        public WorkflowDesignAppService(IWorkflowDesignApi workflowDesignApi)
        {
            _workflowDesignApi = workflowDesignApi;
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
    }
}
