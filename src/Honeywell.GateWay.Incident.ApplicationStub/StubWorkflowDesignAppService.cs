using System;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubWorkflowDesignAppService : BaseIncidentStub, IWorkflowDesignAppService
    {
        public Task<ApiResponse<GetDetailsResponseGto>> GetDetails(GetDetailsRequestGto request)
        {
            try
            {
                var response = new GetDetailsResponseGto();
                foreach (var id in request.Ids)
                {
                    var workflowDesigns = StubData<WorkflowDesignGto[]>().FirstOrDefault(m => m.Id == id);
                    if (workflowDesigns != null) { response.WorkflowDesigns.Add(workflowDesigns); }

                    throw new Exception($"cannot found the workflow design by id:{id}");
                }

                return Task.FromResult(ApiResponse.CreateSuccess().To(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ApiResponse.CreateFailed(ex).To<GetDetailsResponseGto>());
            }
        }

        public Task<ApiResponse<GetIdsResponseGto>> GetIds()
        {
            try
            {
                var response = new GetIdsResponseGto();
                var workflowDesigns = StubData<WorkflowDesignGto[]>().Select(w => new WorkflowDesignIdGto
                {
                    WorkflowDesignReferenceId = w.Id,
                    Name = w.Name
                }).ToList();

                if (workflowDesigns.Any())
                {
                    response.WorkflowDesignIds = workflowDesigns;
                    return Task.FromResult(ApiResponse.CreateSuccess().To(response));
                }

                throw new Exception("cannot found any workflow design");
            }
            catch (Exception ex)
            {
                return Task.FromResult(ApiResponse.CreateFailed(ex).To<GetIdsResponseGto>());
            }
        }
    }
}
