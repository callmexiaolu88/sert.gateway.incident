using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.Extensions.Logging;

namespace Honeywell.GateWay.Incident.Application.Incident
{
    public class IncidentAppService :
        ApplicationService,
        IIncidentAppService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IIncidentRepository _incidentRepository;

        public IncidentAppService(IIncidentRepository incidentRepository,
            IDeviceRepository deviceRepository)
        {
            _incidentRepository = incidentRepository;
            _deviceRepository = deviceRepository;
        }

        public async Task<ExecuteResult> ImportWorkflowDesigns(Stream workflowDesignStream)
        {
            return await _incidentRepository.ImportWorkflowDesigns(workflowDesignStream);
        }

        public async Task<ExecuteResult> ValidatorWorkflowDesigns(Stream workflowDesignStream)
        {
            return await _incidentRepository.ValidatorWorkflowDesigns(workflowDesignStream);
        }

        public async Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            return await _incidentRepository.DeleteWorkflowDesigns(workflowDesignIds);
        }

        public async Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns()
        {
            return await _incidentRepository.GetAllActiveWorkflowDesigns();
        }

        public async Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectorsByName(string workflowName)
        {
            return await _incidentRepository.GetWorkflowDesignSelectorsByName(workflowName);
        }

        public async Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            return await _incidentRepository.GetWorkflowDesignById(workflowDesignId);
        }

        public async Task<WorkflowTemplateGto> DownloadWorkflowTemplate()
        {
            return await _incidentRepository.DownloadWorkflowTemplate();
        }

        public async Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds)
        {
            return await _incidentRepository.ExportWorkflowDesigns(workflowIds);
        }

        public async Task<IncidentGto> GetIncidentById(string incidentId)
        {
            return await _incidentRepository.GetIncidentById(incidentId);
        }

        public async Task<string> CreateIncident(CreateIncidentRequestGto request)
        {
            return await _incidentRepository.CreateIncident(request);
        }

        public async Task<DeviceGto[]> GetDevices()
        {
            Logger.LogInformation("call Incident api GetProwatchDeviceList Start");
            var result = await _deviceRepository.GetDevices();
            var devices = result.Config.Select(x => new DeviceGto
            {
                SiteId = x.Relation[0].Id,
                SiteName = x.Relation[0].EntityId,
                DeviceId = x.Identifiers.Id,
                DeviceDisplayName = x.Identifiers.Name,
                DeviceType = x.Type
            });

            Logger.LogInformation("call Incident api GetProwatchDeviceList end");
            return devices.ToArray();
        }

        public async Task<ExecuteResult> RespondIncident(string incidentId)
        {
            return await _incidentRepository.RespondIncident(incidentId);
        }

        public async Task<ExecuteResult> TakeoverIncident(string incidentId)
        {
            return await _incidentRepository.TakeoverIncident(incidentId);
        }

        public async Task<ExecuteResult> CloseIncident(string incidentId, string reason)
        {
            return await _incidentRepository.CloseIncident(incidentId, reason);
        }
    }
}
