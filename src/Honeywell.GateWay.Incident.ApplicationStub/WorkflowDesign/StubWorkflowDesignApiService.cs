using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Import;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;

namespace Honeywell.GateWay.Incident.ApplicationStub.WorkflowDesign
{
    public class StubWorkflowDesignAppService : IWorkflowDesignApi
    {
        public Task<ImportWorkflowDesignsResponseDto> Imports(Stream workflowDesignStream)
        {
            var result = new ImportWorkflowDesignsResponseDto
            {
                IsSuccess = true
            };
            return Task.FromResult(result);
        }

        public Task<ImportWorkflowDesignsResponseDto> Validate(Stream workflowDesignStream)
        {
            var result = new ImportWorkflowDesignsResponseDto
            {
                IsSuccess = true
            };
            return Task.FromResult(result);
        }

        public Task<ApiResponse> Deletes(WorkflowDesignDeleteRequestDto workflowDesignDeleteRequestDto)
        {
            var result = new ApiResponse
            {
                IsSuccess = true
            };
            return Task.FromResult(result);
        }

        public Task<WorkflowDesignSummaryResponseDto> GetSummaries()
        {
            var result = new WorkflowDesignSummaryResponseDto
            {
                IsSuccess = true,
                Summaries = new List<WorkflowDesignSummaryDto>
                {
                    new WorkflowDesignSummaryDto
                    {
                        Id = MockDetailStore.FirstWorkflowId,
                        Name = MockDetailStore.FirstName,
                        Description = MockDetailStore.FirstDescription
                    },
                    new WorkflowDesignSummaryDto
                    {
                        Id = MockDetailStore.SecondWorkflowId,
                        Name = MockDetailStore.SecondName,
                        Description = MockDetailStore.SecondDescription
                    }
                }
            };
            return Task.FromResult(result);
        }

        public Task<WorkflowDesignResponseDto> GetDetails(WorkflowDesignDetailsRequestDto workflowDesignDetailsRequestDto)
        {
            var result = new WorkflowDesignResponseDto
            {
                IsSuccess = true,
                Details = new List<WorkflowDesignDto>
                {
                    new WorkflowDesignDto
                    {
                        Id = MockDetailStore.FirstWorkflowId,
                        Name = MockDetailStore.FirstName,
                        Description = MockDetailStore.FirstDescription,
                        Steps = new []
                        {
                            new WorkflowStepDesignDto
                            {
                                Id = MockDetailStore.FirstWorkflowStepId,
                                IsOptional = MockDetailStore.FirstWorkflowStepIsOptional,
                                Instruction = MockDetailStore.FirstWorkflowStepInstruction,
                                HelpText = MockDetailStore.FirstWorkflowStepHelpText
                            }
                        }
                    },
                    new WorkflowDesignDto
                    {
                        Id = MockDetailStore.SecondWorkflowId,
                        Name = MockDetailStore.SecondName,
                        Description = MockDetailStore.SecondDescription,
                        Steps = new []
                        {
                            new WorkflowStepDesignDto
                            {
                                Id = MockDetailStore.SecondWorkflowStepId,
                                IsOptional = MockDetailStore.SecondWorkflowStepIsOptional,
                                Instruction = MockDetailStore.SecondWorkflowStepInstruction,
                                HelpText = MockDetailStore.SecondWorkflowStepHelpText
                            }
                        }
                    }
                }.Where(x=>workflowDesignDetailsRequestDto.Ids.Contains(x.Id)).ToList()
            };
            return Task.FromResult(result); 
        }
    }

    public class MockDetailStore
    {
        public static readonly Guid FirstWorkflowId = Guid.Parse("aec60e83-53fb-4e6c-bd78-322264df4c91");
        public static readonly string FirstName = "Asset Damage";
        public static readonly string FirstDescription = "This procedure shall be followed when an asset damage or loss is reported.  ";
        public static readonly Guid FirstWorkflowStepId = Guid.Parse("B19EA2DE-7D05-4D2D-9777-B7C281DF3B6F");
        public static readonly string FirstWorkflowStepInstruction = "Record contact details of person who reported the damage.";
        public static readonly string FirstWorkflowStepHelpText = "";
        public static readonly bool FirstWorkflowStepIsOptional = true;

        public static readonly Guid SecondWorkflowId = Guid.Parse("76caed17-1ad5-4110-855b-366674dfaf58");
        public static readonly string SecondName = "Fire";
        public static readonly string SecondDescription = "This procedure shall be followed when there is a fire alarm or report of fire within the site.";
        public static readonly Guid SecondWorkflowStepId = Guid.Parse("D93D1338-12D9-48EA-A673-F2F6BE4E61EF");
        public static readonly string SecondWorkflowStepInstruction = "Dispatch local first responders to investigate alarm and maintain communications. X37007";
        public static readonly string SecondWorkflowStepHelpText = "Zoom in to region where the smoke detector alarm is. ";
        public static readonly bool SecondWorkflowStepIsOptional = false;
    }
}
