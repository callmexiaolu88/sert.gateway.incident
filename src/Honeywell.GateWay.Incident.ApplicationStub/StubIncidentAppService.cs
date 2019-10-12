using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubIncidentAppService : BaseIncidentStub, IIncidentAppService
    {
        public Task<ExecuteResult> ImportWorkflowDesigns(Stream stream)
        {
            return ResponseRequest();
        }

        public Task<ExecuteResult> ValidatorWorkflowDesigns(Stream stream)
        {
            return ResponseRequest();
        }

        public Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            return ResponseRequest();
        }

        public Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            return StubDataTask<WorkflowDesignSummaryGto[]>();
        }

        public Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectorsByName(string workflowName)
        {
            var workflows = StubData<WorkflowDesignSelectorGto[]>().Where(m => m.Name.Contains(workflowName));
            return Task.FromResult(workflows.ToArray());
        }

        public Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            return Task.FromResult(StubData<WorkflowDesignGto[]>().FirstOrDefault(m => m.Id == Guid.Parse(workflowDesignId)));
        }

        public Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            var resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.WorkflowTemplate.en-us.dotx";
            var fileName = "WorkflowTemplate.dotx";
            return ExportTemplate(resourceName, fileName);
        }

        public Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            var resourceName = "Honeywell.GateWay.Incident.ApplicationStub.Template.Workflows.docx";
            var fileName = "Workflows.docx";
            return ExportTemplate(resourceName, fileName);
        }

        private Task<WorkflowTemplateGto> ExportTemplate(string resourceName, string fileName)
        {
            var template = new WorkflowTemplateGto();
            var fs = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (fs != null)
            {
                var filebytes = new byte[fs.Length];
                fs.Read(filebytes, 0, filebytes.Length);
                template.FileBytes = filebytes;
            }
            template.FileName = fileName;
            template.Status = ExecuteStatus.Successful;
            return Task.FromResult(template);
        }


        public Task<IncidentGto> GetIncidentById(string incidentId)
        {
            return Task.FromResult(StubData<IncidentGto[]>().FirstOrDefault(m => m.Id == Guid.Parse(incidentId)));
        }

        public Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            var workflowName = StubData<WorkflowDesignGto[]>()
                .FirstOrDefault(m => m.Id == Guid.Parse(request.WorkflowDesignReferenceId))
                ?.Name;
            var incident = StubData<IncidentGto[]>().FirstOrDefault(m => m.WorkflowName == workflowName);
            if (incident != null) return Task.FromResult(incident.Id.ToString());
            throw new Exception("cannot found the incident");
        }

        public Task<SiteDeviceGto[]> GetSiteDevices()
        {
            return StubDataTask<SiteDeviceGto[]>();
        }

        public Task<DeviceGto> GetDeviceById(string deviceId, string deviceType)
        {
            var devices = StubData<SiteDeviceGto[]>();

            var result = (from site in devices
                select site.Devices.FirstOrDefault(x => x.DeviceId == deviceId)
                into item
                where item != null
                select item).ToList();
            if (result.Any()) return Task.FromResult(result.FirstOrDefault());
            return Task.FromResult(new DeviceGto());
        }

        public Task<ExecuteResult> RespondIncident(string incidentId)
        {
            return ResponseRequest();
        }

        private static Task<ExecuteResult> ResponseRequest()
        {
            var result = new ExecuteResult { Status = ExecuteStatus.Successful };
            return Task.FromResult(result);
        }

        public Task<ExecuteResult> TakeoverIncident(string incidentId)
        {
            return ResponseRequest();
        }

        public Task<ExecuteResult> CloseIncident(string incidentId, string reason)
        {
            return ResponseRequest();
        }

        public Task<ActiveIncidentListGto> GetActiveIncidentList()
        {
            var result = StubData<List<ActiveIncidentGto>>();
            return Task.FromResult(new ActiveIncidentListGto { List = result, Status = ExecuteStatus.Successful });
        }
    }
}
