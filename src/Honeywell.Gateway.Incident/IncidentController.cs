﻿using System.IO;
using Honeywell.Gateway.Incident.Api;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ExecuteResult> ImportWorkflowDesigns([FromBody]Stream workflowStream)
        {
            var result = await _workflowDesignAppService.ImportWorkflowDesigns(workflowStream);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var result = await _workflowDesignAppService.DeleteWorkflowDesigns(workflowDesignIds);
            return result;
        }

        [HttpPost]
        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            var workflowDesignList = await _workflowDesignAppService.GetAllActiveWorkflowDesigns();
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
