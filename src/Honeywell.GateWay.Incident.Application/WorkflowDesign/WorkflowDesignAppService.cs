using System;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
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

        public async Task<ApiResponse> ImportAsync(Stream workflowDesignStream)
        {
            try
            {
                await _workflowDesignRepository.ImportWorkflowDesigns(workflowDesignStream);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse> ValidateAsync(Stream workflowDesignStream)
        {
            try
            {
                return await _workflowDesignRepository.ValidatorWorkflowDesigns(workflowDesignStream);
            }
            catch(Exception ex)
            {
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse> DeletesAsync(string[] workflowDesignIds)
        {
            try
            {
                await _workflowDesignRepository.DeleteWorkflowDesigns(workflowDesignIds);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse<WorkflowDesignSummaryGto[]>> GetSummariesAsync()
        {
            try
            {
                return await _workflowDesignRepository.GetAllActiveWorkflowDesigns();
            }
            catch(Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignSummaryGto[]>();
            }
        }

        public async Task<ApiResponse<WorkflowDesignSelectorListGto>> GetSelectorsAsync()
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignSelectors();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignSelectorListGto>();
            }
        }

        public async Task<ApiResponse<WorkflowDesignGto>> GetByIdAsync(string workflowDesignId)
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignById(workflowDesignId);
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignGto>();
            }
        }

        public async Task<ApiResponse<WorkflowTemplateGto>> DownloadTemplateAsync()
        {
            try
            {
                return await _workflowDesignRepository.DownloadWorkflowTemplate();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowTemplateGto>();
            }
        }

        public async Task<ApiResponse<WorkflowTemplateGto>> ExportsAsync(string[] workflowDesignIds)
        {
            try
            {
                return await _workflowDesignRepository.ExportWorkflowDesigns(workflowDesignIds);
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowTemplateGto>();
            }
        }

        public async Task<ApiResponse<GetDetailsResponseGto>> GetDetailsAsync(GetDetailsRequestGto request)
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

        public async Task<ApiResponse<GetIdsResponseGto>> GetIdsAsync()
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
