using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.GateWay.Incident.Repository.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class WorkflowDesignRepositoryUnitTest : ApplicationServiceTestBase
    {
        private readonly Mock<IWorkflowDesignMicroApi> _workflowDesignMicroApiMock;

        private readonly IWorkflowDesignRepository _incidentRepository;

        public WorkflowDesignRepositoryUnitTest()
        {
            _workflowDesignMicroApiMock = new Mock<IWorkflowDesignMicroApi>();
            _incidentRepository = new WorkflowDesignRepository(_workflowDesignMicroApiMock.Object);
        }

        [Fact]
        public void GetWorkflowDesignIds_Success()
        {
            //arrange
            var summaryResponseDto = ApiResponse.CreateSuccess().To(new WorkflowDesignSummaryResponseDto
            {
                Summaries = new List<WorkflowDesignSummaryDto>
                {
                    new WorkflowDesignSummaryDto
                    {
                        Description = "description",
                        Id = new Guid(),
                        Name = "name",
                        Version = 1
                    }
                }
            });

            _workflowDesignMicroApiMock.Setup(api => api.GetSummariesAsync())
                .Returns(Task.FromResult(summaryResponseDto));

            //act
            var workflowDesignIds = _incidentRepository.GetWorkflowDesignIds();

            //assert
            Assert.NotNull(workflowDesignIds.Result);
            Assert.NotNull(workflowDesignIds.Result.WorkflowDesignIds);
            Assert.True(workflowDesignIds.Result.WorkflowDesignIds.Any());
            Assert.Equal(workflowDesignIds.Result.WorkflowDesignIds.First().WorkflowDesignReferenceId, summaryResponseDto.Value.Summaries.First().Id);
            Assert.Equal(workflowDesignIds.Result.WorkflowDesignIds.First().Name, summaryResponseDto.Value.Summaries.First().Name);
        }

        [Fact]
        public void GetWorkflowDesigns_Success()
        {
            //arrange
            var workflowGuid = Guid.NewGuid();
            var setGuid = Guid.NewGuid();

            var request = new GetDetailsRequestGto
            {
                Ids = new[] { workflowGuid }
            };

            var workflowDesignResponseDto = ApiResponse.CreateSuccess().To(new WorkflowDesignResponseDto
            {
                Details = new List<WorkflowDesignDto>
                {
                    new WorkflowDesignDto
                    {
                        Id = workflowGuid,
                        Description = "description",
                        Name = "name",
                        Version = 1,
                        Steps = new[]
                        {
                            new WorkflowStepDesignDto
                            {
                                Id = setGuid,
                                HelpText = "help text 1",
                                Instruction = "instruction 1",
                                IsOptional = false,
                                ParentStepId = null
                            },
                            new WorkflowStepDesignDto
                            {
                                Id = new Guid(),
                                HelpText = "help text 2",
                                Instruction = "instruction 2",
                                IsOptional = true,
                                ParentStepId = setGuid,
                            }
                        }
                    }
                }
            });

            _workflowDesignMicroApiMock.Setup(api => api.GetDetailsAsync(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowDesignResponseDto));

            //act
            var workflowDesigns = _incidentRepository.GetWorkflowDesignDetails(request);

            //assert
            Assert.NotNull(workflowDesigns.Result);
            Assert.NotNull(workflowDesigns.Result.WorkflowDesigns);
            Assert.True(workflowDesigns.Result.WorkflowDesigns.Any());
            Assert.Equal(workflowGuid, workflowDesignResponseDto.Value.Details.First().Id);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Id, workflowDesigns.Result.WorkflowDesigns.First().Id);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Name, workflowDesigns.Result.WorkflowDesigns.First().Name);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Description, workflowDesigns.Result.WorkflowDesigns.First().Description);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[0].Id, workflowDesigns.Result.WorkflowDesigns.First().Steps[0].Id);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[0].HelpText, workflowDesigns.Result.WorkflowDesigns.First().Steps[0].HelpText);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[0].Instruction, workflowDesigns.Result.WorkflowDesigns.First().Steps[0].Instruction);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[0].IsOptional, workflowDesigns.Result.WorkflowDesigns.First().Steps[0].IsOptional);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[1].Id, workflowDesigns.Result.WorkflowDesigns.First().Steps[1].Id);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[1].HelpText, workflowDesigns.Result.WorkflowDesigns.First().Steps[1].HelpText);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[1].Instruction, workflowDesigns.Result.WorkflowDesigns.First().Steps[1].Instruction);
            Assert.Equal(workflowDesignResponseDto.Value.Details.First().Steps[1].IsOptional, workflowDesigns.Result.WorkflowDesigns.First().Steps[1].IsOptional);
        }
    }
}
