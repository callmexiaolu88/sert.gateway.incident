using System.IO;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<ExecuteResult> ImportWorkflowDesigns([FromBody] Stream workflowDesignStream)
        {
            var result = await _workflowDesignAppService.ImportWorkflowDesigns(workflowDesignStream);
            return result;
        }

        [HttpPost]
        public async Task<ExecuteResult> ValidatorWorkflowDesigns([FromBody] Stream workflowDesignStream)
        {
            var result = await _workflowDesignAppService.ValidatorWorkflowDesigns(workflowDesignStream);
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
        public async Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors()
        {
            var workflowDesignSelectorList = await _workflowDesignAppService.GetWorkflowDesignSelectors();
            return workflowDesignSelectorList;
        }

        [HttpPost]
        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            var workflowDetail = await _workflowDesignAppService.GetWorkflowDesignById(workflowDesignId);
            return workflowDetail;
        }

        [HttpPost]
        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            var result = await _workflowDesignAppService.DownloadWorkflowTemplate();
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition",
                "attachment; filename=" + HttpUtility.UrlEncode(result.FileName, System.Text.Encoding.UTF8));
            await Response.Body.WriteAsync(result.FileBytes);
            Response.Body.Flush();
            Response.Body.Close();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetIdsResponseGto>> GetIds()
        {

            var result = await _workflowDesignAppService.ExportWorkflowDesigns(workflowDesignIds);
            Response.ContentType = "application/octet-stream";
            await Response.Body.WriteAsync(result.FileBytes);
            Response.Body.Flush();
            Response.Body.Close();
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<GetWorkflowDesignIdsResponseGto>> GetIds()
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
