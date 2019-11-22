﻿using System;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Infra.Api.Abstract;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IIncidentRepository
    {
        Task UpdateWorkflowStepStatus(UpdateStepStatusRequestGto updateWorkflowStepStatusGto);

        Task<IncidentDetailGto> GetIncidentById(string incidentId);

        Task<string> CreateIncident(CreateIncidentRequestGto request);

        Task RespondIncident(string incidentId);

        Task TakeoverIncident(string incidentId);

        Task CloseIncident(string incidentId, string reason);

        Task CompleteIncident(string incidentId);

        Task AddStepComment(AddStepCommentRequestGto addStepCommentGto);

        Task<IncidentSummaryGto[]> GetActiveIncidentList();

        Task<Guid[]> CreateIncidentByAlarm(CreateIncidentByAlarmRequestGto[] requests);

        Task<IncidentStatusInfoGto[]> GetIncidentStatusByAlarm(string[] alarmIds);

        Task<ActivityGto[]> GetActivitysAsync(string incidentId);

        Task<ApiResponse<IncidentStatisticsGto>> GetStatisticsAsync(string deviceId);
    }
}
