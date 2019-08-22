using Honeywell.Gateway.Incident.Api;
using System;
using System.Linq;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Honeywell.Gateway.Incident
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IncidentGatewayController : ControllerBase, IWorkflowDesignGatewayApi
    {
        private readonly IWorkflowDesignAppService _workflowDesignAppService;
        public IncidentGatewayController(IWorkflowDesignAppService workflowDesignAppService)
        {
            _workflowDesignAppService = workflowDesignAppService;
        }

        [HttpPost]
        public JsonResult ImportWorkflowDesigns(IFormFile file)
        {
            var workflowDesignStream = file.OpenReadStream();
            var result = _workflowDesignAppService.ImportWorkflowDesigns(workflowDesignStream);
            return new JsonResult(result);
        }

        [HttpPost]
        public JsonResult DeleteWorkflowDesigns([FromBody]string[] workflowDesignIds)
        {
            var guidList = workflowDesignIds.Select(workflowDesignId => Guid.Parse(workflowDesignId)).ToArray();
            var result = _workflowDesignAppService.DeleteWorkflowDesigns(guidList);
            return new JsonResult(result);
        }

        [HttpGet]
        public JsonResult GetAllActiveWorkflowDesign()
        {
            var workflowDesignList = _workflowDesignAppService.GetAllActiveWorkflowDesign();
            return new JsonResult(workflowDesignList);
        }

        [HttpGet]
        public JsonResult GetWorkflowDesignsById(string workflowDesignId)
        {
            var requestId = new[] { Guid.Parse(workflowDesignId) };
            var workflowDetailList = _workflowDesignAppService.GetWorkflowDesignsByIds(requestId);
            return new JsonResult(workflowDetailList.FirstOrDefault());
        }
    }
}
