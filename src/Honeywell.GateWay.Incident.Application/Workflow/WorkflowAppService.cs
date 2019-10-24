using System;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.Workflow
{
    public class WorkflowAppService : ApplicationService, IWorkflowAppService
    {
        private readonly IIncidentRepository _incidentRepository;

        public WorkflowAppService(IIncidentRepository incidentRepository)
        {
            _incidentRepository = incidentRepository;
        }

        public async Task<ApiResponse<GetWorkflowDesignsResponseGto>> GetDesignDetails(GetWorkflowDesignsRequestGto request)
        {
            try
            {
                return await _incidentRepository.GetWorkflowDesigns(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To(new GetWorkflowDesignsResponseGto());
            }
        }

        public async Task<ApiResponse<GetWorkflowDesignIdentifiersResponseGto>> GetDesignIds()
        {
            try
            {
                return await _incidentRepository.GetWorkflowDesignIds();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To(new GetWorkflowDesignIdentifiersResponseGto());
            }
        }
    }
}
