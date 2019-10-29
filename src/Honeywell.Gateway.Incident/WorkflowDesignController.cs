﻿using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Honeywell.Gateway.Incident
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WorkflowDesignController : ControllerBase, IWorkflowDesignApi
    {
        private readonly IWorkflowDesignAppService _workflowDesignAppService;

        public WorkflowDesignController(IWorkflowDesignAppService workflowDesignAppService)
        {
            _workflowDesignAppService = workflowDesignAppService;
        }

        [HttpPost]
        public async Task<ApiResponse<GetIdsResponseGto>> GetIds()
        {
            var result = await _workflowDesignAppService.GetIds();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetDetailsResponseGto>> GetDetails(GetDetailsRequestGto request)
        {
            var result = await _workflowDesignAppService.GetDetails(request);
            return result;
        }
    }
}