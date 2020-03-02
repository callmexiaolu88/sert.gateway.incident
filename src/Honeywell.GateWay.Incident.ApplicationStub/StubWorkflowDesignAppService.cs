using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetIds;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Update;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubWorkflowDesignAppService : BaseIncidentStub, IWorkflowDesignAppService
    {
        public async Task<ApiResponse<CreateWorkflowDesignResponseGto>> CreateAsync(CreateWorkflowDesignRequestGto createWorkflowDesignRequestGto)
        {
            return await StubDataAsync<CreateWorkflowDesignResponseGto>();
        }

        public async Task<ApiResponse<UpdateWorkflowDesignResponseGto>> UpdateAsync(UpdateWorkflowDesignRequestGto updateWorkflowDesignRequestGto)
        {
            return await StubDataAsync<UpdateWorkflowDesignResponseGto>();
        }

        public Task<ApiResponse> ImportAsync(Stream workflowDesignStream)
        {
            return ResponseRequest();
        }

        public Task<ApiResponse> ValidateAsync(Stream workflowDesignStream)
        {
            return ResponseRequest();
        }

        public Task<ApiResponse> DeletesAsync(string[] workflowDesignIds)
        {
            return ResponseRequest();
        }

        public async Task<ApiResponse<WorkflowDesignListGto[]>> GetListAsync(string condition)
        {
            return await StubDataAsync<WorkflowDesignListGto[]>();
        }

        public async Task<ApiResponse<WorkflowDesignSelectorGto[]>> GetSelectorsAsync()
        {
            var result = await StubDataAsync<List<WorkflowDesignSelectorGto>>();
            return result.ToArray();
        }

        public async Task<ApiResponse<WorkflowDesignDetailGto>> GetDetailByIdAsync(string workflowDesignId)
        {
            return (await StubDataAsync<WorkflowDesignDetailGto[]>()).FirstOrDefault(
                m => m.Id == Guid.Parse(workflowDesignId));
        }

        public Task<ApiResponse<WorkflowTemplateGto>> DownloadTemplateAsync()
        {
            const string resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.WorkflowTemplate.en-us.dotx";
            const string fileName = "WorkflowTemplate.dotx";
            return ExportTemplate(resourceName, fileName);
        }

        public Task<ApiResponse<WorkflowTemplateGto>> ExportsAsync(string[] workflowDesignIds)
        {
            const string resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.Workflows.docx";
            const string fileName = "Workflows.docx";
            return ExportTemplate(resourceName, fileName);
        }

        public async Task<ApiResponse<WorkflowDesignDetailGto[]>> GetDetailsAsync(Guid[] workflowDesignIds)
        {
            try
            {
                if (workflowDesignIds == null)
                {
                    throw new ArgumentNullException(nameof(workflowDesignIds));
                }
                var result = new List<WorkflowDesignDetailGto>();
                foreach (var id in workflowDesignIds)
                {
                    var workflowDesigns = (await StubDataAsync<WorkflowDesignDetailGto[]>()).FirstOrDefault(m => m.Id == id);
                    if (workflowDesigns != null)
                    {
                        result.Add(workflowDesigns);
                    }
                    else
                    {
                        throw new NullReferenceException($"cannot found the workflow design by id:{id}");
                    }

                }
                return result.ToArray();
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignDetailGto[]>();
            }
        }

        public async Task<ApiResponse<WorkflowDesignIdGto[]>> GetIdsAsync()
        {
            try
            {
                var result = new List<WorkflowDesignIdGto>();
                var workflowDesigns = (await StubDataAsync<WorkflowDesignDetailGto[]>()).Select(w => new WorkflowDesignIdGto
                {
                    WorkflowDesignReferenceId = w.Id,
                    Name = w.Name
                }).ToList();

                if (workflowDesigns.Any())
                {
                    result.AddRange(workflowDesigns);
                    return result.ToArray();
                }

                throw new NullReferenceException("cannot found any workflow design");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<WorkflowDesignIdGto[]>();
            }
        }

        private async Task<ApiResponse<WorkflowTemplateGto>> ExportTemplate(string resourceName, string fileName)
        {
            var template = new WorkflowTemplateGto();
            var fs = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (fs == null)
            {
                throw new HoneywellException($"{resourceName} is not found");
            }

            var buffer = new byte[1024];
            using var ms = new MemoryStream();
            int read;
            while ((read = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            template.FileBytes = ms.ToArray();
            template.FileName = fileName;
            return template;
        }

        private static Task<ApiResponse> ResponseRequest()
        {
            var result = ApiResponse.CreateSuccess();
            return Task.FromResult(result);
        }
    }
}
