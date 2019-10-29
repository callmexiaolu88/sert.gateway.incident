using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubWorkflowDesignAppService : BaseIncidentStub, IWorkflowDesignAppService
    {
        public Task<ExecuteResult> ImportAsync(Stream stream)
        {
            return ResponseRequest();
        }

        public Task<ExecuteResult> ValidatorAsync(Stream stream)
        {
            return ResponseRequest();
        }

        public Task<ExecuteResult> DeletesAsync(string[] workflowDesignIds)
        {
            return ResponseRequest();
        }

        public Task<WorkflowDesignSummaryGto[]> GetSummariesAsync()
        {
            return StubDataTask<WorkflowDesignSummaryGto[]>();
        }

        public Task<WorkflowDesignSelectorListGto> GetSelectorsAsync()
        {
            var result = StubData<List<WorkflowDesignSelectorGto>>();
            return Task.FromResult(new WorkflowDesignSelectorListGto { List = result, Status = ExecuteStatus.Successful });
        }

        public Task<WorkflowDesignGto> GetByIdAsync(string workflowDesignId)
        {
            return Task.FromResult(StubData<WorkflowDesignGto[]>().FirstOrDefault(m => m.Id == Guid.Parse(workflowDesignId)));
        }

        public Task<WorkflowTemplateGto> DownloadTemplateAsync()
        {
            var resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.WorkflowTemplate.en-us.dotx";
            var fileName = "WorkflowTemplate.dotx";
            return ExportTemplate(resourceName, fileName);
        }

        public Task<WorkflowTemplateGto> ExportsAsync(string[] workflowIds)
        {
            var resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.Workflows.docx";
            var fileName = "Workflows.docx";
            return ExportTemplate(resourceName, fileName);
        }

        public Task<ApiResponse<GetDetailsResponseGto>> GetDetailsAsync(GetDetailsRequestGto request)
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

        public Task<ApiResponse<GetIdsResponseGto>> GetIdsAsync()
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

        private Task<WorkflowTemplateGto> ExportTemplate(string resourceName, string fileName)
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
            while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            template.FileBytes = ms.ToArray();
            template.FileName = fileName;
            template.Status = ExecuteStatus.Successful;
            return Task.FromResult(template);
        }

        private static Task<ExecuteResult> ResponseRequest()
        {
            var result = new ExecuteResult { Status = ExecuteStatus.Successful };
            return Task.FromResult(result);
        }
    }
}
