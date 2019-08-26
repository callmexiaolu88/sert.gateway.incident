﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Infra.Core.Ddd.Application;
using Microsoft.AspNetCore.Http;

namespace Honeywell.GateWay.Incident.Application.WorkflowDesign
{
    public class StubWorkflowDesignAppService : ApplicationService, IWorkflowDesignAppService
    {

        public Task<ExecuteResult> ImportWorkflowDesigns(IFormFile file)
        {
            var result = new ExecuteResult();
            if (file == null)
            {
                result.Status = ExecuteStatus.Error;
                result.ErrorList = new List<string> { "File content cannot be empty!" };
            }
            else
            {
                result.Status = ExecuteStatus.Successful;
            }

            return Task.FromResult(result);
        }

        public Task<ExecuteResult> DeleteWorkflowDesigns(string[] workflowDesignIds)
        {
            var result = new ExecuteResult();
            if (workflowDesignIds == null || workflowDesignIds.Length == 0)
            {
                result.Status = ExecuteStatus.Error;
                result.ErrorList = new List<string> { "Please select at least one sop!" };
            }
            else
            {
                result.Status = ExecuteStatus.Successful;
            }
            return Task.FromResult(result);
        }

        Task<WorkflowDesignSummaryGto[]> IWorkflowDesignGatewayApi.GetAllActiveWorkflowDesign()
        {
            throw new NotImplementedException();
        }

        public Task<WorkflowDesignGto> GetWorkflowDesignById(string workflowDesignId)
        {
            throw new NotImplementedException();
        }

        public WorkflowDesignSummaryGto[] GetAllActiveWorkflowDesign()
        {
            var result = new WorkflowDesignSummaryGto[2];
            result[0] = new WorkflowDesignSummaryGto()
            {
                Id = MockDetailStore.FirstWorkflowId,
                Name = MockDetailStore.FirstName,
                Description = MockDetailStore.FirstDescription
            };
            result[1] = new WorkflowDesignSummaryGto()
            {
                Id = MockDetailStore.SecondWorkflowId,
                Name = MockDetailStore.SecondName,
                Description = MockDetailStore.SecondDescription
            };
            return result;
        }

        public WorkflowDesignGto[] GetWorkflowDesignsByIds(Guid[] workflowDesignIds)
        {
            var result = new List<WorkflowDesignGto>();
            foreach (var item in MockDetailStore.Items)
            {
                if (workflowDesignIds.Contains(item.Id))
                {
                    result.Add(item);
                }
            }
            return result.ToArray();
        }
    }

    public static class MockDetailStore
    {
        public static Guid FirstWorkflowId = Guid.Parse("aec60e83-53fb-4e6c-bd78-322264df4c91");
        public static string FirstName = "Event Prep – Master";
        public static string FirstDescription = "This procedure shall be completed 48hours before any events.";

        public static Guid SecondWorkflowId = Guid.Parse("76caed17-1ad5-4110-855b-366674dfaf58");
        public static string SecondName = "Fire";
        public static string SecondDescription = "This procedure shall be followed when there is a fire alarm or report of fire within the site.";

        public static List<WorkflowDesignGto> Items = new List<WorkflowDesignGto>();

        static MockDetailStore()
        {
            Items.Add(
                new WorkflowDesignGto
                {
                    Id = FirstWorkflowId,
                    Name = FirstName,
                    Description = FirstDescription,
                    Steps = new[]
                    {
                        new WorkflowStepDesignGto{
                            Id = Guid.Parse("76caed17-1ad5-4110-855b-366674dfaf58"),
                            HelpText = "This is an import workflow",
                            Instruction = "Dispatch local first responders to investigate alarm and maintain communications. X37007",
                            IsOptional = false
                        }
                    }
                });
            Items.Add(
                new WorkflowDesignGto
                {
                    Id = SecondWorkflowId,
                    Name = SecondName,
                    Description = SecondDescription,
                    Steps = new[]
                    {
                        new WorkflowStepDesignGto{
                            Id = Guid.Parse("99a877e6-c01a-476c-97ab-93019b2d20e1"),
                            HelpText = "Nothing will be happen",
                            Instruction = "Verify presence of fire and/or smoke, via CCTV cameras or visually by first responders.",
                            IsOptional = false
                        },
                        new WorkflowStepDesignGto{
                            Id = Guid.Parse("103b3663-047e-45d3-bce8-3f5a244d5459"),
                            HelpText = "Display pressurization status",
                            Instruction =  "Verify fire safety automatic response execution in place.",
                            IsOptional = true
                        }
                    }
                });
        }
    }
}
