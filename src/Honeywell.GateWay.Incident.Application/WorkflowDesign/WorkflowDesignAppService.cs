using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Honeywell.Micro.Services.Workflow.Domain.Shared;

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

        public ExecuteResult ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            var responseDtoList = _workflowDesignApi.Imports(workflowDesignStream);
            if (responseDtoList.IsSuccess)
            {
                return new ExecuteResult
                {
                    Status = ExecuteStatus.Successful
                };
            }
            else
            {
                var result = new ExecuteResult
                {
                    Status = ExecuteStatus.Error
                };
                foreach (var item in responseDtoList.ImportResponseList)
                {
                    foreach (var importResult in item.Results)
                    {
                        result.ErrorList.Add(importResult.Message);
                    }
                }
                return result;
            }
        }

        public ExecuteResult DeleteWorkflowDesigns(Guid[] workflowDesignIds)
        {
            var result = _workflowDesignApi.Deletes(new WorkflowDesignDeleteRequestDto(){Ids = workflowDesignIds });
            if (result.IsSuccess)
            {
                return new ExecuteResult
                {
                    Status = ExecuteStatus.Successful
                };
            }
            else
            {
                return new ExecuteResult
                {
                    Status = ExecuteStatus.Error,
                    ErrorList = new List<string> { result.ToString() }
                };
            }
        }

        public WorkflowDesignSummaryGto[] GetAllActiveWorkflowDesign()
        {
            var result = _workflowDesignApi.GetSummaries();

            return HoneyMapper.Map<WorkflowDesignSummaryDto[],
                WorkflowDesignSummaryGto[]>(result.Summaries.ToArray());
        }

        public WorkflowDesignGto[] GetWorkflowDesignsByIds(Guid[] workflowDesignIds)
        {
            var request = new WorkflowDesignDetailsRequestDto { Ids = workflowDesignIds };
            var result = _workflowDesignApi.GetDetails(request);
            return HoneyMapper.Map<WorkflowDesignDto[],
                WorkflowDesignGto[]>(result.Details.ToArray());
        }
    }
}
