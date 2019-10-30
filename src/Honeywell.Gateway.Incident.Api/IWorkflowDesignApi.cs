using Honeywell.Gateway.Incident.Api.WorkflowDesign;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;
using System.IO;
using System.Threading.Tasks;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowDesignApi : IRemoteService
    {
        Task<ApiResponse> ImportAsync(Stream workflowDesignStream);

        Task<ApiResponse> ValidateAsync(Stream workflowDesignStream);

        Task<ApiResponse> DeletesAsync(string[] workflowDesignIds);

        Task<ApiResponse<WorkflowDesignSummaryGto[]>> GetSummariesAsync();

        Task<ApiResponse<WorkflowDesignSelectorListGto>> GetSelectorsAsync();

        Task<ApiResponse<WorkflowDesignGto>> GetByIdAsync(string workflowDesignId);

        Task<ApiResponse<WorkflowTemplateGto>> DownloadTemplateAsync();

        Task<ApiResponse<WorkflowTemplateGto>> ExportsAsync(string[] workflowDesignIds);

        Task<ApiResponse<GetIdsResponseGto>> GetIdsAsync();

        Task<ApiResponse<GetDetailsResponseGto>> GetDetailsAsync(GetDetailsRequestGto request);
    }
}
