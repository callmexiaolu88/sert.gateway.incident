using Honeywell.Infra.Core;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowDesignGatewayApi : IRemoteService
    {
        JsonResult ImportWorkflowDesigns(IFormFile file);

        JsonResult DeleteWorkflowDesigns(string[] workflowDesignIds);

        JsonResult GetAllActiveWorkflowDesign();

        JsonResult GetWorkflowDesignsById(string workflowDesignId);
    }
}
