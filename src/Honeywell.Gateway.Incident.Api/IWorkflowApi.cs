using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.Gateway.Incident.Api.Workflow.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowApi : IRemoteService
    {
        Task<ApiResponse<GetWorkflowDesignIdsResponseGto>> GetDesignIds();

        Task<ApiResponse<GetWorkflowDesignDetailsResponseGto>> GetDesignDetails(GetWorkflowDesignDetailsRequestGto request);
    }
}
