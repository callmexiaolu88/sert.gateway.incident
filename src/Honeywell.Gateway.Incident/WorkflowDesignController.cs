using System.IO;
using System.Threading.Tasks;
using System.Web;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
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
        public async Task<ExecuteResult> ImportAsync([FromBody] Stream workflowDesignStream)
        {
            var result = await _workflowDesignAppService.ImportAsync(workflowDesignStream);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> ValidatorAsync([FromBody] Stream workflowDesignStream)
        {
            var result = await _workflowDesignAppService.ValidatorAsync(workflowDesignStream);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> DeletesAsync(string[] workflowDesignIds)
        {
            var result = await _workflowDesignAppService.DeletesAsync(workflowDesignIds);
            return result;
        }

        [HttpPost]
        public async Task<WorkflowDesignSummaryGto[]> GetSummariesAsync()
        {
            var workflowDesignList = await _workflowDesignAppService.GetSummariesAsync();
            return workflowDesignList;
        }

        [HttpPost]
        public async Task<WorkflowDesignSelectorListGto> GetSelectorsAsync()
        {
            var workflowDesignSelectorList = await _workflowDesignAppService.GetSelectorsAsync();
            return workflowDesignSelectorList;
        }

        [HttpPost]
        public async Task<WorkflowDesignGto> GetByIdAsync(string workflowDesignId)
        {
            var workflowDetail = await _workflowDesignAppService.GetByIdAsync(workflowDesignId);
            return workflowDetail;
        }

        [HttpPost]
        public async Task<WorkflowTemplateGto> DownloadTemplateAsync()
        {
            var result = await _workflowDesignAppService.DownloadTemplateAsync();
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=" + HttpUtility.UrlEncode(result.FileName, System.Text.Encoding.UTF8));
            await Response.Body.WriteAsync(result.FileBytes);
            Response.Body.Flush();
            Response.Body.Close();
            return result;
        }

        [HttpPost]
        public async Task<WorkflowTemplateGto> ExportsAsync(string[] workflowDesignIds)
        {

            var result = await _workflowDesignAppService.ExportsAsync(workflowDesignIds);
            Response.ContentType = "application/octet-stream";
            await Response.Body.WriteAsync(result.FileBytes);
            Response.Body.Flush();
            Response.Body.Close();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetIdsResponseGto>> GetIdsAsync()
        {
            var result = await _workflowDesignAppService.GetIdsAsync();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetDetailsResponseGto>> GetDetailsAsync(GetDetailsRequestGto request)
        {
            var result = await _workflowDesignAppService.GetDetailsAsync(request);
            return result;
        }
    }
}
