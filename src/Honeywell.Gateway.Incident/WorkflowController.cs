using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.GateWay.Incident.Application.Workflow;
using Honeywell.Infra.Api.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Honeywell.Gateway.Incident
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkflowController : ControllerBase, IWorkflowApi
    {
        private readonly IWorkflowAppService _workflowAppService;

        public WorkflowController(IWorkflowAppService workflowAppService)
        {
            _workflowAppService = workflowAppService;
        }

        [HttpPost]
        public async Task<ApiResponse<GetWorkflowDesignIdentifiersResponseGto>> GetDesignIds()
        {
            var result = await _workflowAppService.GetDesignIds();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetWorkflowDesignsResponseGto>> GetDesignDetails(GetWorkflowDesignsRequestGto request)
        {
            var result = await _workflowAppService.GetDesignDetails(request);
            return result;
        }
    }
}
