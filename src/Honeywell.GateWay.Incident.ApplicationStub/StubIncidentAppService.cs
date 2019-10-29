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
using Honeywell.Infra.Core.Common.Exceptions;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class StubIncidentAppService : BaseIncidentStub, IIncidentAppService
    {
  
        public Task<ExecuteResult> UpdateWorkflowStepStatus(string workflowStepId, bool isHandled)
        {
            return ResponseRequest();
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
            CreateByAlarmRequestGto request)
        {
            try
            {
                var response = new CreateIncidentResponseGto();
                foreach (var incidentData in request.CreateDatas)
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
                return Task.FromResult(ApiResponse.CreateFailed(ex).To<CreateIncidentResponseGto>());
            }
        }

        public Task<ApiResponse<GetStatusByAlarmResponseGto>> GetStatusByAlarm(
            GetStatusByAlarmRequestGto request)
        {
            try
            {
                var response = new GetStatusByAlarmResponseGto();
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
                return Task.FromResult(ApiResponse.CreateFailed(ex).To<GetStatusByAlarmResponseGto>());
            }
        }

        public Task<ExecuteResult> AddStepComment(AddStepCommentGto addStepCommentGto)
        {
            return ResponseRequest();
        }
    }
}
