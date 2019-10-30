using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubWorkflowDesignAppService : BaseIncidentStub, IWorkflowDesignAppService
    {
        public Task<ApiResponse> ImportAsync(Stream stream)
        {
            return ResponseRequest();
        }

        public Task<ApiResponse> ValidateAsync(Stream stream)
        {
            return ResponseRequest();
        }

        public Task<ApiResponse> DeletesAsync(string[] workflowDesignIds)
        {
            return ResponseRequest();
        }

        public async Task<ApiResponse<WorkflowDesignSummaryGto[]>> GetSummariesAsync()
        {
            return await StubDataAsync<WorkflowDesignSummaryGto[]>();
        }

        public async Task<ApiResponse<WorkflowDesignSelectorListGto>> GetSelectorsAsync()
        {
            var result = await StubDataAsync<List<WorkflowDesignSelectorGto>>();
            return new WorkflowDesignSelectorListGto { List = result};
        }

        public async Task<ApiResponse<WorkflowDesignGto>> GetByIdAsync(string workflowDesignId)
        {
            return (await StubDataAsync<WorkflowDesignGto[]>()).FirstOrDefault(
                m => m.Id == Guid.Parse(workflowDesignId));
        }

        public Task<ApiResponse<WorkflowTemplateGto>> DownloadTemplateAsync()
        {
            var resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.WorkflowTemplate.en-us.dotx";
            var fileName = "WorkflowTemplate.dotx";
            return ExportTemplate(resourceName, fileName);
        }

        public Task<ApiResponse<WorkflowTemplateGto>> ExportsAsync(string[] workflowIds)
        {
            var resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.Workflows.docx";
            var fileName = "Workflows.docx";
            return ExportTemplate(resourceName, fileName);
        }

        public async Task<ApiResponse<GetDetailsResponseGto>> GetDetailsAsync(GetDetailsRequestGto request)
        {
            try
            {
                var response = new GetDetailsResponseGto();
                foreach (var id in request.Ids)
                {
                    var workflowDesigns = (await StubDataAsync<WorkflowDesignGto[]>()).FirstOrDefault(m => m.Id == id);
                    if (workflowDesigns != null) { response.WorkflowDesigns.Add(workflowDesigns); }

                    throw new Exception($"cannot found the workflow design by id:{id}");
                }

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<GetDetailsResponseGto>();
            }
        }

        public async Task<ApiResponse<GetIdsResponseGto>> GetIdsAsync()
        {
            try
            {
                var response = new GetIdsResponseGto();
                var workflowDesigns = (await StubDataAsync<WorkflowDesignGto[]>()).Select(w => new WorkflowDesignIdGto
                {
                    WorkflowDesignReferenceId = w.Id,
                    Name = w.Name
                }).ToList();

                if (workflowDesigns.Any())
                {
                    response.WorkflowDesignIds = workflowDesigns;
                    return response;
                }

                throw new Exception("cannot found any workflow design");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateFailed(ex).To<GetIdsResponseGto>();
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
