using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetIds;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Update;
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
        public async Task<ApiResponse<CreateWorkflowDesignResponseGto>> CreateAsync(CreateWorkflowDesignRequestGto createWorkflowDesignRequestGto)
        {
            var result = await _workflowDesignAppService.CreateAsync(createWorkflowDesignRequestGto);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<UpdateWorkflowDesignResponseGto>> UpdateAsync(UpdateWorkflowDesignRequestGto updateWorkflowDesignRequestGto)
        {
            var result = await _workflowDesignAppService.UpdateAsync(updateWorkflowDesignRequestGto);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> ImportAsync([FromBody] Stream workflowDesignStream)
        {
            var result = await _workflowDesignAppService.ImportAsync(workflowDesignStream);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> ValidateAsync([FromBody] Stream workflowDesignStream)
        {
            var result = await _workflowDesignAppService.ValidateAsync(workflowDesignStream);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse> DeletesAsync(string[] workflowDesignIds)
        {
            var result = await _workflowDesignAppService.DeletesAsync(workflowDesignIds);
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowDesignListGto[]>> GetListAsync(string condition)
        {
            var workflowDesignList = await _workflowDesignAppService.GetListAsync(condition);
            return workflowDesignList;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowDesignSelectorGto[]>> GetSelectorsAsync()
        {
            var workflowDesignSelectorList = await _workflowDesignAppService.GetSelectorsAsync();
            return workflowDesignSelectorList;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowDesignDetailGto>> GetDetailByIdAsync(string workflowDesignId)
        {
            var workflowDetail = await _workflowDesignAppService.GetDetailByIdAsync(workflowDesignId);
            return workflowDetail;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowTemplateGto>> DownloadTemplateAsync()
        {
            var result = await _workflowDesignAppService.DownloadTemplateAsync();
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=" + HttpUtility.UrlEncode(result.Value.FileName, System.Text.Encoding.UTF8));
            await Response.Body.WriteAsync(result.Value.FileBytes);
            Response.Body.Flush();
            Response.Body.Close();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowTemplateGto>> ExportsAsync(string[] workflowDesignIds)
        {

            var result = await _workflowDesignAppService.ExportsAsync(workflowDesignIds);
            Response.ContentType = "application/octet-stream";
            await Response.Body.WriteAsync(result.Value.FileBytes);
            Response.Body.Flush();
            Response.Body.Close();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowDesignIdGto[]>> GetIdsAsync()
        {
            var result = await _workflowDesignAppService.GetIdsAsync();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<WorkflowDesignDetailGto[]>> GetDetailsAsync(Guid[] workflowDesignIds)
        {
            var result = await _workflowDesignAppService.GetDetailsAsync(workflowDesignIds);
            return result;
        }
    }
}
