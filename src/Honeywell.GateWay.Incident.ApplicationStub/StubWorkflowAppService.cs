﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.Gateway.Incident.Api.Workflow.List;
using Honeywell.GateWay.Incident.Application.Workflow;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubWorkflowAppService : BaseIncidentStub, IWorkflowAppService
    {
        public Task<ApiResponse<GetWorkflowDesignDetailsResponseGto>> GetDesignDetails(GetWorkflowDesignDetailsRequestGto request)
        {
            try
            {
                var response = new GetWorkflowDesignDetailsResponseGto();
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
                return Task.FromResult(ApiResponse.CreateFailed(ex).To<GetWorkflowDesignDetailsResponseGto>());
            }
        }

        public Task<ApiResponse<GetWorkflowDesignIdsResponseGto>> GetDesignIds()
        {
            try
            {
                var response = new GetWorkflowDesignIdsResponseGto();
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
                return Task.FromResult(ApiResponse.CreateFailed(ex).To<GetWorkflowDesignIdsResponseGto>());
            }
        }
    }
}
