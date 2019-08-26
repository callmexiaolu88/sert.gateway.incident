using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Microsoft.AspNetCore.Http;

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

        public async Task<ExecuteResult> ImportWorkflowDesigns(IFormFile file)
        {
            var workflowDesignStream = file.OpenReadStream();
            var responseDtoList = await _workflowDesignApi.Imports(workflowDesignStream);
            if (responseDtoList.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            var result = new ExecuteResult {Status = ExecuteStatus.Error};
            responseDtoList.ImportResponseList.ForEach(x => result.ErrorList.AddRange(x.Errors));
            return result;
        }

        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var guidList = workflowDesignIds.Select(Guid.Parse).ToArray();
            var result = await _workflowDesignApi.Deletes(new WorkflowDesignDeleteRequestDto {Ids = guidList });
            if (result.IsSuccess)
            {
                return ExecuteResult.Success;
            }

            return new ExecuteResult
            {
                Status = ExecuteStatus.Error,
                ErrorList = new List<string> { result.Message }
            };
        }

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesign()
        {
            var result = await _workflowDesignApi.GetSummaries();
            if (result.IsSuccess)
            {
                return HoneyMapper.Map<WorkflowDesignSummaryDto[],
                    WorkflowDesignSummaryGto[]>(result.Summaries.ToArray());
            }

            return new WorkflowDesignSummaryGto[] { };
        }

        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            var requestId = new[] { Guid.Parse(workflowDesignId) };
            var result = await _workflowDesignApi.GetDetails(new WorkflowDesignDetailsRequestDto {Ids = requestId });
            if (result.IsSuccess)
            {
                var workflowDesignList = HoneyMapper.Map<WorkflowDesignDto[], WorkflowDesignGto[]>(result.Details.ToArray());
                return workflowDesignList.FirstOrDefault();
            }
            return null;
        }
    }
}
