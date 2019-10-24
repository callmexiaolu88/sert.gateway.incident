using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowApi : IRemoteService
    {
        Task<ApiResponse<GetWorkflowDesignIdentifiersResponseGto>> GetDesignIds();

        Task<ApiResponse<GetWorkflowDesignsResponseGto>> GetDesignDetails(GetWorkflowDesignsRequestGto request);
    }
}
