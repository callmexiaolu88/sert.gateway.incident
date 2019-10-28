using System;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.Workflow
{
    public class WorkflowDesignAppService : ApplicationService, IWorkflowDesignAppService
    {
        private readonly IIncidentRepository _incidentRepository;

        public WorkflowDesignAppService(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        public async Task<ApiResponse<GetWorkflowDesignDetailsResponseGto>> GetDetails(GetWorkflowDesignDetailsRequestGto request)
        {
            try
            {
                return await _incidentRepository.GetWorkflowDesignDetails(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetWorkflowDesignDetailsResponseGto>();
            }
        }

        public async Task<ApiResponse<GetWorkflowDesignIdsResponseGto>> GetIds()
        {
            try
            {
                return await _incidentRepository.GetWorkflowDesignIds();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetWorkflowDesignIdsResponseGto>();
            }
        }
    }
}
