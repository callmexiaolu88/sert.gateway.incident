using System;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.WorkflowDesign
{
    public class WorkflowDesignAppService : ApplicationService, IWorkflowDesignAppService
    {
        private readonly IWorkflowDesignRepository _workflowDesignRepository;

        public WorkflowDesignAppService(IWorkflowDesignRepository workflowDesignRepository)
        {
            _workflowDesignRepository = workflowDesignRepository;
        }

        public async Task<ApiResponse<GetDetailsResponseGto>> GetDetails(GetDetailsRequestGto request)
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignDetails(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetDetailsResponseGto>();
            }
        }

        public async Task<ApiResponse<GetIdsResponseGto>> GetIds()
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignIds();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetIdsResponseGto>();
            }
        }
    }
}
