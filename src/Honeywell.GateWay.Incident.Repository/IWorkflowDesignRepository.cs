using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IWorkflowDesignRepository
    {
        Task<GetIdsResponseGto> GetWorkflowDesignIds();

        Task<GetDetailsResponseGto> GetWorkflowDesignDetails(GetDetailsRequestGto request);
    }
}
