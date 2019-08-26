using Honeywell.Infra.Core;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Microsoft.AspNetCore.Http;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowDesignGatewayApi : IRemoteService
    {
        Task<ExecuteResult> ImportWorkflowDesigns(IFormFile file);

        Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId);
    }
}
