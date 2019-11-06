using System;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IWorkflowDesignRepository
    {
        Task ImportWorkflowDesigns(Stream workflowDesignStream);

        Task<ApiResponse> ValidatorWorkflowDesigns(Stream workflowDesignStream);

        Task DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectors();

        Task<WorkflowDesignDetailGto> GetWorkflowDesignById(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadWorkflowTemplate();

        Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds);

        Task<WorkflowDesignIdGto[]> GetWorkflowDesignIds();

        Task<WorkflowDesignDetailGto[]> GetWorkflowDesignDetails(Guid[] workflowDesignIds);
    }
}
