using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Import;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class WorkflowDesignAppServiceUnitTest : ApplicationServiceTestBase
    {
        private readonly Mock<IWorkflowDesignApi> _workflowDesignApiMock;

        private readonly IWorkflowDesignGatewayApi _workflowDesignGatewayApi;

        public WorkflowDesignAppServiceUnitTest()
        {
            _workflowDesignApiMock = new Mock<IWorkflowDesignApi>();
            _workflowDesignGatewayApi = new WorkflowDesignAppService(_workflowDesignApiMock.Object);
        }

        [Fact]
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Success()
        {
            // arrange
            _workflowDesignApiMock.Setup(x => x.Deletes(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .Returns(Task.FromResult(new ApiResponse { IsSuccess = true }));

            // action
            var result = await _workflowDesignGatewayApi.DeleteWorkflowDesigns(new[] { It.IsAny<Guid>().ToString() });

            // assert
            Assert.Equal(ExecuteStatus.Successful, result.Status);
        }

        [Fact]
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Failed()
        {
            // arrange
            var mockResult = new ApiResponse { IsSuccess = false, Message = "error delete" };
            _workflowDesignApiMock.Setup(x => x.Deletes(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .Returns(Task.FromResult(mockResult));

            // action
            var result = await _workflowDesignGatewayApi.DeleteWorkflowDesigns(new[] { It.IsAny<Guid>().ToString() });

            // assert
            Assert.Equal(ExecuteStatus.Error, result.Status);
            Assert.True(1 == result.ErrorList.Count);
            Assert.Equal(mockResult.Message, result.ErrorList[0]);
        }


        [Fact]
        public async Task WorkflowDesign_GetAllActiveWorkflowDesigns_Success()
        {
            // arrange
            var summaryResponseDto = MockWorkflowDesignSummaryResponseDto();
            _workflowDesignApiMock.Setup(x => x.GetSummaries()).Returns(Task.FromResult(summaryResponseDto));

            // action
            var result = await _workflowDesignGatewayApi.GetAllActiveWorkflowDesigns();

            // assert
            Assert.True(1 == result.Length);

            foreach (var item in result)
            {
                var expectedItem = summaryResponseDto.Summaries.FirstOrDefault(x => x.Id == item.Id);
                Assert.NotNull(expectedItem);
                Assert.Equal(expectedItem.Name, item.Name);
                Assert.Equal(expectedItem.Description, item.Description);
            }
        }

        [Fact]
        public async Task WorkflowDesign_GetAllActiveWorkflowDesigns_Failed()
        {
            // arrange
            var summaryResponseDto = new WorkflowDesignSummaryResponseDto { IsSuccess = false, Message = "error 1" };
            _workflowDesignApiMock.Setup(x => x.GetSummaries()).Returns(Task.FromResult(summaryResponseDto));

            // action
            var result = await _workflowDesignGatewayApi.GetAllActiveWorkflowDesigns();

            // assert
            Assert.True(0 == result.Length);
        }

        [Fact]
        public async Task WorkflowDesign_GetWorkflowDesignById_Success()
        {
            // arrange
            var workflowDesignDto = MockWorkflowDesignResponseDto();
            _workflowDesignApiMock
                .Setup(x => x.GetDetails(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowDesignDto));

            // action
            var result = await _workflowDesignGatewayApi.GetWorkflowDesignById(Guid.NewGuid().ToString());

            // assert
            Assert.NotNull(result);
            var expectedItem = workflowDesignDto.Details.FirstOrDefault(x => x.Id == result.Id);
            Assert.NotNull(expectedItem);
            Assert.Equal(expectedItem.Name, result.Name);
            Assert.Equal(expectedItem.Description, result.Description);
            Assert.Equal(expectedItem.Steps.Length, result.Steps.Length);
            foreach (var step in result.Steps)
            {
                var exceptedStep = expectedItem.Steps.FirstOrDefault(x => x.Id == step.Id);
                Assert.NotNull(exceptedStep);
                Assert.Equal(exceptedStep.IsOptional, step.IsOptional);
                Assert.Equal(exceptedStep.HelpText, step.HelpText);
                Assert.Equal(exceptedStep.Instruction, step.Instruction);
            }
        }

        [Fact]
        public async Task WorkflowDesign_GetWorkflowDesignById_Failed()
        {
            // arrange
            var workflowDesignDto = new WorkflowDesignResponseDto { IsSuccess = false, Message = "error 1" };
            _workflowDesignApiMock
                .Setup(x => x.GetDetails(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowDesignDto));

            // action
            var result = await _workflowDesignGatewayApi.GetWorkflowDesignById(Guid.NewGuid().ToString());

            // assert
            Assert.Null(result);
        }


        [Fact]
        public async Task ImportWorkFlowDesign_SuccessfulAsync()
        {
            // arrange
            var responseDto = new ImportWorkflowDesignsResponseDto {IsSuccess = false};
            responseDto.ImportResponseList.Add(new WorkflowResponseDto("workflow1",new List<string>{"duplicate name","desc over length"}));
            _workflowDesignApiMock.Setup(x => x.ImportWorkflows(It.IsAny<Stream>())).
                Returns(Task.FromResult(responseDto));
            // action

            var result = await _workflowDesignGatewayApi.ImportWorkflowDesigns(It.IsAny<Stream>());

            // assert
            Assert.NotNull(result);
        }


        #region private methods

        private WorkflowDesignSummaryResponseDto MockWorkflowDesignSummaryResponseDto()
        {
            var summaryResponseDto = new WorkflowDesignSummaryResponseDto
            {
                IsSuccess = true,
                Summaries = new List<WorkflowDesignSummaryDto>
                {
                    new WorkflowDesignSummaryDto
                    {
                        Id=Guid.NewGuid(),
                        Name = "workflow design 1",
                        Description = "workflow design 1 description"
                    }
                }
            };
            return summaryResponseDto;
        }


        private WorkflowDesignResponseDto MockWorkflowDesignResponseDto()
        {
            var details = new WorkflowDesignResponseDto
            {
                IsSuccess = true,
                Details = new List<WorkflowDesignDto>
                {
                    new WorkflowDesignDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "workflow design 1",
                        Description = "workflow design 1 description",
                        Steps = new []
                        {
                            new WorkflowStepDesignDto
                            {
                                Id = Guid.NewGuid(),
                                IsOptional = true,
                                Instruction = "instruction 1",
                                HelpText = "help test 1"
                            },
                            new WorkflowStepDesignDto
                            {
                                Id = Guid.NewGuid(),
                                IsOptional = false,
                                Instruction = "instruction 2",
                                HelpText = "help test 2"
                            }
                        }
                    }
                }
            };

            return details;
        }
        #endregion

    }
}
