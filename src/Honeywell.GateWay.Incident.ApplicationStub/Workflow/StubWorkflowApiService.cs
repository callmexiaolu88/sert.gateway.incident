using System;
using System.Threading.Tasks;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Create;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.Workflow.List;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Domain.Shared;

namespace Honeywell.GateWay.Incident.ApplicationStub.Workflow
{
    public class StubWorkflowApiService : BaseIncidentStub, IWorkflowInstanceApi
    {
        public Task<CreateWorkflowInstanceResponseDto> Create(CreateWorkflowInstanceRequestDto request)
        {
            return StubData<CreateWorkflowInstanceResponseDto>();
        }

        public ActivityStatus CompleteWorkflow(Guid workflowId)
        {
            return ActivityStatus.WorkflowIsComplete;
        }

        public ActivityStatus CancelWorkflow(Guid workflowId, string reason)
        {
            return ActivityStatus.WorkflowIsCancelled;
        }

        public WorkflowSummaryDto[] GetWorkflowSummaries(Guid[] workflowIds)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus AddComment(Guid workflowId, string comment)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus AddStepComment(Guid workflowId, Guid workflowStepId, string comment)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus AcknowledgeActivity(Guid workflowId)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus RemoveOwner(Guid workflowId)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus TakeOwnership(Guid workflowId)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus UpdateWorkflowStepStatus(Guid workflowId, Guid workflowStepId, bool isComplete)
        {
            throw new NotImplementedException();
        }

        public WorkflowInstanceDto[] GetWorkflows(Guid[] workflowIds)
        {
            throw new NotImplementedException();
        }

        public Task<WorkflowDetailsResponseDto> GetWorkflowDetails(WorkflowDetailsRequestDto request)
        {
            return StubData<WorkflowDetailsResponseDto>();
        }

        public WorkflowInstanceDto[] GetPermissions(WorkflowInstanceDto[] workflows)
        {
            throw new NotImplementedException();
        }

        public Task<WorkflowSummaryResponseDto> GetWorkflowSummaries(WorkflowSummaryRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
