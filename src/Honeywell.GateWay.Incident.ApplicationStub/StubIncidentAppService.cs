using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Infra.Api.Abstract;

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

        public Task<WorkflowDesignSelectorListGto> GetWorkflowDesignSelectors()
        {
            var result = StubData<List<WorkflowDesignSelectorGto>>();
            return Task.FromResult(new WorkflowDesignSelectorListGto { List = result, Status = ExecuteStatus.Successful });
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

        public Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled)
        {
            return ResponseRequest();
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
            var incidentInfo = StubData<IncidentGto[]>().First(m => m.Id == Guid.Parse(incidentId));
            if (string.IsNullOrEmpty(incidentInfo.DeviceId))
            {
                return Task.FromResult(incidentInfo);
            }

            var devices = StubData<SiteDeviceGto[]>();
            var deviceList = (from site in devices
                select site.Devices.FirstOrDefault(x => x.DeviceId == incidentInfo.DeviceId)
                into item
                where item != null
                select item).ToList();
            var device = deviceList.First();
            incidentInfo.DeviceDisplayName = device.DeviceDisplayName;
            incidentInfo.DeviceLocation = device.DeviceLocation;
            return Task.FromResult(incidentInfo);
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

        public Task<ExecuteResult> CompleteIncident(string incidentId)
        {
            return ResponseRequest();
        }

        public Task<ActiveIncidentListGto> GetActiveIncidentList()
        {
            var result = StubData<List<ActiveIncidentGto>>();
            return Task.FromResult(new ActiveIncidentListGto { List = result, Status = ExecuteStatus.Successful });
        }

        public Task<ApiResponse<CreateIncidentResponseGto>> CreateByAlarm(
            CreateIncidentByAlarmRequestGto request)
        {
            try
            {
                var response = new CreateIncidentResponseGto();
                foreach (var incidentData in request.CreateIncidentDatas)
                {
                    var workflowName = StubData<WorkflowDesignGto[]>()
                        .FirstOrDefault(m => m.Id == incidentData.WorkflowDesignReferenceId)?.Name;

                    var incident = StubData<IncidentGto[]>().FirstOrDefault(m => m.WorkflowName == workflowName);
                    if (incident != null) response.IncidentIds.Add(incident.Id);
                    throw new Exception("cannot found the incident");
                }

                return Task.FromResult(ApiResponse.CreateSuccess().To(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ApiResponse.CreateFailed(ex).To(new CreateIncidentResponseGto()));
            }
        }

        public Task<ApiResponse<GetIncidentStatusResponseGto>> GetStatusByAlarmId(
            GetIncidentStatusRequestGto request)
        {
            try
            {
                var response = new GetIncidentStatusResponseGto();
                foreach (var id in request.AlarmIds)
                {
                    var incident = StubData<IncidentGto[]>().FirstOrDefault((m => m.DeviceId == id));
                    if (incident == null)
                    {
                        throw new Exception($"cannot found the incident associates with alarm id {id}");
                    }

                    var statusInfoGto = new IncidentStatusInfoGto
                    {
                        AlarmId = id,
                        IncidentId = incident.Id,
                        Status = incident.State
                    };
                    response.IncidentStatusInfos.Add(statusInfoGto);
                }

                return Task.FromResult(ApiResponse.CreateSuccess().To(response));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ApiResponse.CreateFailed(ex).To(new GetIncidentStatusResponseGto()));
            }
        }

        public Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto)
        {
            return ResponseRequest();
        }
    }
}
