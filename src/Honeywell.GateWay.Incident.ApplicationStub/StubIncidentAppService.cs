using System;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubIncidentAppService: BaseIncidentStub,IIncidentAppService
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
            return StubData<WorkflowDesignSummaryGto[]>();
        }

        public Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectorsByName(string workflowName)
        {
            return StubData<WorkflowDesignSelectorGto[]>();
        }

        public Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            return StubData<WorkflowDesignGto>();
        }

        public Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            return StubData<WorkflowTemplateGto>();
        }

        public Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            return StubData<WorkflowTemplateGto>();
        }

        public Task<IncidentGto> GetIncidentById(string incidentId)
        {
            return StubData<IncidentGto>();
        }

        public Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public Task<DeviceGto[]> GetDevices()
        {
            return StubData<DeviceGto[]>();
        }

        public Task<ExecuteResult> RespondIncident(string incidentId)
        {
            return ResponseRequest();
        }

        private static Task<ExecuteResult> ResponseRequest()
        {
            var result = new ExecuteResult {Status = ExecuteStatus.Successful};
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
            return StubData<ActiveIncidentListGto>();
        }
    }
}
