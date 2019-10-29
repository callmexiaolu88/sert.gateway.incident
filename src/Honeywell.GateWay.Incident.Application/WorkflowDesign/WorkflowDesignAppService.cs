using System;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.WorkflowDesign
{
    public class WorkflowDesignAppService : ApplicationService, IWorkflowDesignAppService
    {
        private readonly IWorkflowDesignRepository _workflowDesignRepository;

        public WorkflowDesignAppService(IWorkflowDesignRepository workflowDesignRepository)
        {
            _workflowDesignRepository = workflowDesignRepository;
        }
        public async Task<ExecuteResult> ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            return await _workflowDesignRepository.ImportWorkflowDesigns(workflowDesignStream);
        }

        public async Task<ExecuteResult> ValidatorWorkflowDesigns(Stream workflowDesignStream)
        {
            return await _workflowDesignRepository.ValidatorWorkflowDesigns(workflowDesignStream);
        }

        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            return await _workflowDesignRepository.DeleteWorkflowDesigns(workflowDesignIds);
        }

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            return await _workflowDesignRepository.GetAllActiveWorkflowDesigns();
        }

        public async Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors()
        {
            return await _workflowDesignRepository.GetWorkflowDesignSelectors();
        }

        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            return await _workflowDesignRepository.GetWorkflowDesignById(workflowDesignId);
        }

        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            return await _workflowDesignRepository.DownloadWorkflowTemplate();
        }

        public async Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowDesignIds)
        {
            return await _workflowDesignRepository.ExportWorkflowDesigns(workflowDesignIds);
        }

        public async Task<ApiResponse<GetDetailsResponseGto>> GetDetails(GetDetailsRequestGto request)
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignDetails(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetDetailsResponseGto>();
            }
        }

        public async Task<ApiResponse<GetIdsResponseGto>> GetIds()
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignIds();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<GetIdsResponseGto>();
            }
        }
    }
}
