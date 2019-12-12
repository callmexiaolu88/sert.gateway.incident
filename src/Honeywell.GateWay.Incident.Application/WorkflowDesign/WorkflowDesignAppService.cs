using System;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetIds;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Update;
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

        public async Task<ApiResponse> CreateAsync(CreateWorkflowDesignRequestGto createWorkflowDesignRequestGto)
        {
            try
            {
                if (createWorkflowDesignRequestGto == null)
                {
                    throw new ArgumentNullException(nameof(createWorkflowDesignRequestGto));
                }
                if (createWorkflowDesignRequestGto.Steps == null)
                {
                    throw new ArgumentNullException(nameof(createWorkflowDesignRequestGto.Steps));
                }

                await _workflowDesignRepository.CreateWorkflowDesign(createWorkflowDesignRequestGto);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex);
            }
        }

        public async Task<ApiResponse> UpdateAsync(UpdateWorkflowDesignRequestGto updateWorkflowDesignRequestGto)
        {
            try
            {
                await _workflowDesignRepository.UpdateWorkflowDesign(updateWorkflowDesignRequestGto);
                return ApiResponse.CreateSuccess();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex);
            }
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
            Logger.LogInformation("WorkflowDesignAppService.ValidateAsync entry.");
            try
            {
                return await _workflowDesignRepository.ValidatorWorkflowDesigns(workflowDesignStream);
            }
            catch(Exception ex)
            {
                Logger.LogError($"WorkflowDesignAppService.ValidateAsync exception: {ex}", ex);
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

        public async Task<ApiResponse<WorkflowDesignListGto[]>> GetListAsync(string condition)
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignList(condition);
            }
            catch(Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignListGto[]>();
            }
        }

        public async Task<ApiResponse<WorkflowDesignSelectorGto[]>> GetSelectorsAsync()
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignSelectors();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignSelectorGto[]>();
            }
        }

        public async Task<ApiResponse<WorkflowDesignDetailGto>> GetDetailByIdAsync(string workflowDesignId)
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignById(workflowDesignId);
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignDetailGto>();
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

        public async Task<ApiResponse<WorkflowDesignDetailGto[]>> GetDetailsAsync(Guid[] workflowDesignIds)
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignDetails(workflowDesignIds);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignDetailGto[]>();
            }
        }

        public async Task<ApiResponse<WorkflowDesignIdGto[]>> GetIdsAsync()
        {
            try
            {
                return await _workflowDesignRepository.GetWorkflowDesignIds();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignIdGto[]>();
            }
        }
    }
}
