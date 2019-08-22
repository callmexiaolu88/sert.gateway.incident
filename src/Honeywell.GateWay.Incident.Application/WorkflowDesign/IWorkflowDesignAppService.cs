using System;
using System.IO;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.GateWay.Incident.Application.WorkflowDesign
{
    public interface IWorkflowDesignAppService
    {
        ExecuteResult ImportWorkflowDesigns(Stream workflowDesignStream);

        ExecuteResult DeleteWorkflowDesigns(Guid[] workflowDesignId);

        WorkflowDesignSummaryGto[] GetAllActiveWorkflowDesign();

        WorkflowDesignGto[] GetWorkflowDesignsByIds(Guid[] workflowDesignIds);

    }
}
