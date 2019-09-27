﻿using System.IO;
using Honeywell.Infra.Core;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api
{
    public interface IIncidentApi : IRemoteService
    {
        Task<ExecuteResult> ImportWorkflowDesigns(Stream stream);

        Task<ExecuteResult> ValidatorWorkflowDesigns(Stream stream);

        Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds);

        Task<WorkflowDesignSummaryGto[]> GetAllActiveWorkflowDesigns();

        Task<WorkflowDesignSelectorGto[]> GetWorkflowDesignSelectorsByName(string workflowName);

        Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId);

        Task<WorkflowTemplateGto> DownloadWorkflowTemplate();

        Task<WorkflowTemplateGto> ExportWorkflowDesigns(string[] workflowIds);

        Task<IncidentGto> GetIncidentById(string incidentId);

        Task<string> CreateIncident(CreateIncidentRequestGto request);

        Task<DeviceGto[]> GetDevices();
    }
}