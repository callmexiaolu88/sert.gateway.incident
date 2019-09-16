using System.IO;
using Honeywell.Infra.Core;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IIncidentGatewayApi : IRemoteService
    {
        Task<ExecuteResult> ImportWorkflowDesigns(Stream stream);

        Task<ExecuteResult> ValidatorWorkflowDesigns(Stream stream);

        Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignsSelectorByName(string workflowName);

        Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadWorkflowTemplate();

        Task<IncidentGto> GetIncidentById(string incidentId);
    }
}
