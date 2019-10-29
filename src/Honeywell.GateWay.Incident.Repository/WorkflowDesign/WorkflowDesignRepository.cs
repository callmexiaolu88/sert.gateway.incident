using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
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
