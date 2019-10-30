using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IWorkflowDesignRepository
    {
        Task ImportWorkflowDesigns(Stream workflowDesignStream);

        Task<ApiResponse> ValidatorWorkflowDesigns(Stream workflowDesignStream);

        Task DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors();

        Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadWorkflowTemplate();

        Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds);

        Task<GetIdsResponseGto> GetWorkflowDesignIds();

        Task<GetDetailsResponseGto> GetWorkflowDesignDetails(GetDetailsRequestGto request);
    }
}
