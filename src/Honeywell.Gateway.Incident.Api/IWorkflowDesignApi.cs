using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IWorkflowDesignApi : IRemoteService
    {
        Task<ApiResponse<GetIdsResponseGto>> GetIds();

        Task<ApiResponse<GetDetailsResponseGto>> GetDetails(GetDetailsRequestGto request);
    }
}
