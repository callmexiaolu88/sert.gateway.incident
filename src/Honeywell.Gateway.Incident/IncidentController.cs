using Honeywell.Gateway.Incident.Api;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Honeywell.Gateway.Incident
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IncidentController : ControllerBase, IWorkflowDesignGatewayApi
    {
        private readonly IWorkflowDesignAppService _workflowDesignAppService;

        public IncidentController(IWorkflowDesignAppService workflowDesignAppService)
        {
            _workflowDesignAppService = workflowDesignAppService;
        }

        [HttpPost]
        public async Task<ExecuteResult> ImportWorkflowDesigns(IFormFile file)
        {
            var result = await _workflowDesignAppService.ImportWorkflowDesigns(file);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var result = await _workflowDesignAppService.DeleteWorkflowDesigns(workflowDesignIds);
            return result;
        }

        [HttpPost]
        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesign()
        {
            var workflowDesignList = await _workflowDesignAppService.GetAllActiveWorkflowDesign();
            return workflowDesignList;
        }

        [HttpPost]
        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            var workflowDetail = await _workflowDesignAppService.GetWorkflowDesignById(workflowDesignId);
            return workflowDetail;
        }
    }
}
