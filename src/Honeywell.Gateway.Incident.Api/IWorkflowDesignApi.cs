using System;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.getIds;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowDesignApi : IRemoteService
    {
        Task<ApiResponse> ImportAsync(Stream workflowDesignStream);

        Task<ApiResponse> ValidateAsync(Stream workflowDesignStream);

        Task<ApiResponse> DeletesAsync(string[] workflowDesignIds);

        Task<ApiResponse<WorkflowDesignListGto[]>> GetListAsync(string condition);

        Task<ApiResponse<WorkflowDesignSelectorGto[]>> GetSelectorsAsync();

        Task<ApiResponse<WorkflowDesignDetailGto>> GetDetailByIdAsync(string workflowDesignId);

        Task<ApiResponse<WorkflowTemplateGto>> DownloadTemplateAsync();

        Task<ApiResponse<WorkflowTemplateGto>> ExportsAsync(string[] workflowDesignIds);

        Task<ApiResponse<WorkflowDesignIdGto[]>> GetIdsAsync();

        Task<ApiResponse<WorkflowDesignDetailGto[]>> GetDetailsAsync(Guid[] workflowDesignIds);
    }
}
