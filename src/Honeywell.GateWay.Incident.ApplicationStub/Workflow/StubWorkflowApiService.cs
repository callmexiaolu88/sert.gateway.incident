using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Create;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.Workflow.List;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Domain.Shared;

namespace Honeywell.GateWay.Incident.ApplicationStub.Workflow
{
    public class StubWorkflowApiService : IWorkflowInstanceApi
    {
        public Task<CreateWorkflowInstanceResponseDto> Create(CreateWorkflowInstanceRequestDto request)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus CompleteWorkflow(Guid workflowId)
        {
            throw new NotImplementedException();
        }

        public ActivityStatus CancelWorkflow(Guid workflowId, string reason)
        {
            throw new NotImplementedException();
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

            var step1 = new WorkflowStepDto()
            {
                Id = Guid.NewGuid(),
                IsComplete = false,
                IsOptional = true,
                Instruction = "Curzon Hall Lobby Court West external Gates can be unlocked remotely.",
                HelpText = "3 gates labelled as: Door 2, 3, 4."
            };

            var step2 = new WorkflowStepDto()
            {
                Id = Guid.NewGuid(),
                IsComplete = true,
                IsOptional = false,
                Instruction = "Curzon Hall Lobby staircase pressurisation can be triggered from fire alarm.",
                HelpText = "Must inform control room if triggering fire alarm."
            };


            var step3 = new WorkflowStepDto()
            {
                Id = Guid.NewGuid(),
                IsComplete = false,
                IsOptional = true,
                Instruction = "Confirm with event manager on final event requirements and special needs.  Provide link to event preparation contract.",
                HelpText = "View event booking calendar. On the calendar, link to event contract."
            };


            var step4 = new WorkflowStepDto()
            {
                Id = Guid.NewGuid(),
                IsComplete = false,
                IsOptional = false,
                Instruction = "If required by the event, engage Energy team for pre-event check.",
                HelpText = "Any cabling on special electricity supply? Backup generators ready?"
            };

            var step5 = new WorkflowStepDto()
            {
                Id = Guid.NewGuid(),
                IsComplete = true,
                IsOptional = true,
                Instruction = "Threat assessment has been performed.",
                HelpText = "Link to security plan for the event."
            };


            var detail = new WorkflowDto()
            {
                Id = Guid.NewGuid(),
                Owner = "admin1",
                Status = WorkflowStatus.Active,
                Number = 137,
                Version = 12,
                Name = "Event Prep Master",
                Description = "This procedure shall be completed 48hours before any events.  This procedure is for Convention Centre buildings.",
                WorkflowSteps = new[] { step1, step2, step3, step4, step5 }
            };


            var response = new WorkflowDetailsResponseDto()
            {
                IsSuccess =  true,
                Details =  new List<WorkflowDto> { detail }
            };
            return Task.FromResult(response);
        }

        public WorkflowInstanceDto[] GetPermissions(WorkflowInstanceDto[] workflows)
        {
            throw new NotImplementedException();
        }
    }
}
