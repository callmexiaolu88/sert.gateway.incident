using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowDesignApi : IRemoteService
    {
        Task<ExecuteResult> ImportAsync(Stream workflowDesignStream);

        Task<ExecuteResult> ValidatorAsync(Stream workflowDesignStream);

        Task<ExecuteResult> DeletesAsync(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetSummariesAsync();

        Task<WorkflowDesignSelectorListGto> GetSelectorsAsync();

        Task<WorkflowDesignGto> GetByIdAsync(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadTemplateAsync();

        Task<WorkflowTemplateGto> ExportsAsync(string[] workflowDesignIds);

        Task<ApiResponse<GetIdsResponseGto>> GetIdsAsync();

        Task<ApiResponse<GetDetailsResponseGto>> GetDetailsAsync(GetDetailsRequestGto request);
    }
}
