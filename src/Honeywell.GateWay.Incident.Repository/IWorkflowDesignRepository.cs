using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IWorkflowDesignRepository
    {
        Task<ExecuteResult> ImportWorkflowDesigns(Stream workflowDesignStream);

        Task<ExecuteResult> ValidatorWorkflowDesigns(Stream workflowDesignStream);

        Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors();

        Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadWorkflowTemplate();

        Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds);

        Task<GetIdsResponseGto> GetWorkflowDesignIds();

        Task<GetDetailsResponseGto> GetWorkflowDesignDetails(GetDetailsRequestGto request);
    }
}
