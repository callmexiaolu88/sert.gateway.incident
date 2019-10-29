using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.GateWay.Incident.Repository.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.DownloadTemplate;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Export;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Import;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
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
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Success()
        {
            // arrange
            _workflowDesignMicroApiMock.Setup(x => x.DeletesAsync(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .ReturnsAsync(ApiResponse.CreateSuccess());

            // action
            var result = await _incidentRepository.DeleteWorkflowDesigns(new[] { It.IsAny<Guid>().ToString() });

            // assert
            Assert.Equal(ExecuteStatus.Successful, result.Status);
        }

        [Fact]
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Failed()
        {
            // arrange
            var mockResult = ApiResponse.CreateFailed("error delete");
            _workflowDesignMicroApiMock.Setup(x => x.DeletesAsync(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .ReturnsAsync(mockResult);

            // action
            var result = await _incidentRepository.DeleteWorkflowDesigns(new[] { It.IsAny<Guid>().ToString() });

            // assert
            Assert.Equal(ExecuteStatus.Error, result.Status);
            Assert.True(1 == result.ErrorList.Count);
        }


        [Fact]
        public async Task WorkflowDesign_GetAllActiveWorkflowDesigns_Success()
        {
            // arrange
            var summaryResponseDto = MockWorkflowDesignSummaryResponseDto();
            var mockResponse = ApiResponse.CreateSuccess().To(summaryResponseDto);
            _workflowDesignMicroApiMock.Setup(x => x.GetSummariesAsync()).ReturnsAsync(mockResponse);

            // action
            var result = await _incidentRepository.GetAllActiveWorkflowDesigns();

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
        public async Task WorkflowDesign_GetWorkflowDesignSelectors_Success()
        {
            // arrange
            var selectorResponseDto = MockWorkflowDesignSelectorResponseDto();
            var mockResponse = ApiResponse.CreateSuccess().To(selectorResponseDto);
            _workflowDesignMicroApiMock.Setup(x => x.GetSelectorAsync()).ReturnsAsync(mockResponse);

            // action
            var result = await _incidentRepository.GetWorkflowDesignSelectors();

            // assert
            Assert.True(1 == result.List.Count);
            foreach (var item in result.List)
            {
                var expectedItem = selectorResponseDto.Selectors.FirstOrDefault(x => x.Id == item.Id);
                Assert.NotNull(expectedItem);
                Assert.Equal(expectedItem.Name, item.Name);
            }
        }

        [Fact]
        public async Task WorkflowDesign_GetAllActiveWorkflowDesigns_Failed()
        {
            // arrange
            var mockResponse = ApiResponse.CreateFailed().To<WorkflowDesignSummaryResponseDto>();
            _workflowDesignMicroApiMock.Setup(x => x.GetSummariesAsync()).ReturnsAsync(mockResponse);

            // action
            var result = await _incidentRepository.GetAllActiveWorkflowDesigns();

            // assert
            Assert.True(0 == result.Length);
        }

        [Fact]
        public async Task WorkflowDesign_GetWorkflowDesignById_Success()
        {
            // arrange
            var workflowDesignDto = MockWorkflowDesignResponseDto();
            var mockResponse = ApiResponse.CreateSuccess().To(workflowDesignDto);
            _workflowDesignMicroApiMock
                .Setup(x => x.GetDetailsAsync(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .ReturnsAsync(mockResponse);

            // action
            var result = await _incidentRepository.GetWorkflowDesignById(Guid.NewGuid().ToString());

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
            var mockResponse = ApiResponse.CreateFailed().To<WorkflowDesignResponseDto>();

            _workflowDesignMicroApiMock
                .Setup(x => x.GetDetailsAsync(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .ReturnsAsync(mockResponse);

            // action
            var result = await _incidentRepository.GetWorkflowDesignById(Guid.NewGuid().ToString());

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ImportWorkFlowDesign_SuccessfulAsync()
        {
            // arrange
            var mockWorkflowResponse = new WorkflowResponseDto("workflow1", new List<string> { "duplicate name", "desc over length" });
            var mockImportWorkflowDesignsResponseDto = new ImportWorkflowDesignsResponseDto
            {
                ImportResponseList = new List<WorkflowResponseDto> { mockWorkflowResponse }
            };

            var responseDto = ApiResponse.CreateSuccess().To(mockImportWorkflowDesignsResponseDto);

            _workflowDesignMicroApiMock.Setup(x => x.ImportsAsync(It.IsAny<Stream>())).
                ReturnsAsync(responseDto);

            // action
            var result = await _incidentRepository.ImportWorkflowDesigns(It.IsAny<Stream>());

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DownloadWorkflowTemplate_Success()
        {
            // arrange
            var responseDto = ApiResponse.CreateSuccess().To(new WorkflowDownloadTemplateResultDto());
            _workflowDesignMicroApiMock.Setup(x => x.DownloadTemplateAsync()).ReturnsAsync(responseDto);
            var result = await _incidentRepository.DownloadWorkflowTemplate();

            // assert
            Assert.True(result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public async Task ExportWorkflowDesigns_Success()
        {
            //assert
            var guidIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var strIds = guidIds.Select(x => x.ToString()).ToArray();
            var workflowsDto = new ExportWorkflowsResponseDto { WorkflowsBytes = new byte[10] };
            var mockResponse = ApiResponse.CreateSuccess().To(workflowsDto);

            _workflowDesignMicroApiMock.Setup(x => x.ExportAsync(It.IsAny<ExportWorkflowRequestDto>()))
                .ReturnsAsync(mockResponse);

            //act
            var result = await _incidentRepository.ExportWorkflowDesigns(strIds);

            //assert
            Assert.True(result.Status == ExecuteStatus.Successful);
            Assert.True(result.FileBytes != null && result.FileBytes.Length > 0);
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


        private WorkflowDesignSummaryResponseDto MockWorkflowDesignSummaryResponseDto()
        {
            var summaryResponseDto = new WorkflowDesignSummaryResponseDto
            {
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

        private WorkflowDesignSelectorResponseDto MockWorkflowDesignSelectorResponseDto()
        {
            var selectorResponseDto = new WorkflowDesignSelectorResponseDto
            {
                Selectors = new List<WorkflowDesignSelectorDto>
                {
                    new WorkflowDesignSelectorDto
                    {
                        Id=Guid.NewGuid(),
                        Name = "workflow design 1",
                        ReferenceId = Guid.NewGuid()
                    }
                }
            };
            return selectorResponseDto;
        }

        private WorkflowDesignResponseDto MockWorkflowDesignResponseDto()
        {
            var details = new WorkflowDesignResponseDto
            {
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
    }
}
