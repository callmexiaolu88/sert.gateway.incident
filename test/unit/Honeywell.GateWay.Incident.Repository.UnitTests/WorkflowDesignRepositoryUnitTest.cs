using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Create;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Update;
using Honeywell.GateWay.Incident.Repository.WorkflowDesign;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.DownloadTemplate;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Create;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Export;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Import;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.List;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Update;
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
        public async Task ValidateAsync_ValidatorWorkflowDesigns_Success()
        {
            var mockResponse = new ImportWorkflowDesignsResponseDto();
            mockResponse.ImportResponseList = new List<WorkflowResponseDto>()
            {
                new WorkflowResponseDto()
                {
                    Errors = new List<string>(){ "IsSuccess" }
                }
            };

            _workflowDesignMicroApiMock.Setup(x => x.ValidateAsync(It.IsAny<Stream>()))
                .ReturnsAsync(mockResponse);

            var result = await _incidentRepository.ValidatorWorkflowDesigns(It.IsAny<Stream>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(mockResponse.ImportResponseList[0].Errors[0], result.Messages[0].Message);
        }

        [Fact]
        public async Task ValidateAsync_ValidatorWorkflowDesigns_CreateFailed()
        {
            var mockResponse = new ImportWorkflowDesignsResponseDto();
            mockResponse.ImportResponseList = new List<WorkflowResponseDto>()
            {
                new WorkflowResponseDto()
                {
                    Errors = new List<string>(){ "IsFail" }
                }
            };

            _workflowDesignMicroApiMock.Setup(x => x.ValidateAsync(It.IsAny<Stream>()))
                .ReturnsAsync(ApiResponse.CreateFailed().To(mockResponse));

            var response = await _incidentRepository.ValidatorWorkflowDesigns(It.IsAny<Stream>());          

            Assert.NotNull(response);
            Assert.False(response.IsSuccess);
            Assert.Equal(mockResponse.ImportResponseList[0].Errors[0], response.Messages[0].Message);
        }

        [Fact]
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Success()
        {
            // arrange
            _workflowDesignMicroApiMock.Setup(x => x.DeletesAsync(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .ReturnsAsync(ApiResponse.CreateSuccess());

            // action
            await _incidentRepository.DeleteWorkflowDesigns(new[] {Guid.NewGuid().ToString()});

            // assert
            _workflowDesignMicroApiMock.Verify(x => x.DeletesAsync(It.IsAny<WorkflowDesignDeleteRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Failed()
        {
            // arrange
            var mockResult = ApiResponse.CreateFailed("error delete");
            _workflowDesignMicroApiMock.Setup(x => x.DeletesAsync(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .ReturnsAsync(mockResult);

            // action
            var act = new Func<Task>(async () => await _incidentRepository.DeleteWorkflowDesigns(new[] {It.IsAny<Guid>().ToString()}));

            // assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }


        [Fact]
        public async Task WorkflowDesign_GetWorkflowDesignList_Success()
        {
            // arrange
            var summaryResponseDto = MockWorkflowDesignListResponseDto();
            _workflowDesignMicroApiMock.Setup(x => x.GetListAsync(It.IsAny<WorkflowDesignListRequestDto>())).ReturnsAsync(summaryResponseDto);

            // action
            var result = await _incidentRepository.GetWorkflowDesignList(string.Empty);

            // assert
            Assert.True(1 == result.Length);

            foreach (var item in result)
            {
                var expectedItem = summaryResponseDto.Lists.FirstOrDefault(x => x.Id == item.Id);
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
            _workflowDesignMicroApiMock.Setup(x => x.GetSelectorAsync()).ReturnsAsync(selectorResponseDto);

            // action
            var result = await _incidentRepository.GetWorkflowDesignSelectors();

            // assert
            Assert.True(1 == result.Length);
            foreach (var item in result)
            {
                var expectedItem = selectorResponseDto.Selectors.FirstOrDefault(x => x.Id == item.Id);
                Assert.NotNull(expectedItem);
                Assert.Equal(expectedItem.Name, item.Name);
            }
        }

        [Fact]
        public async Task WorkflowDesign_GetWorkflowDesignList_Failed()
        {
            // arrange
            var mockResponse = ApiResponse.CreateFailed().To<WorkflowDesignListResponseDto>();
            _workflowDesignMicroApiMock.Setup(x => x.GetListAsync(It.IsAny<WorkflowDesignListRequestDto>())).ReturnsAsync(mockResponse);

            // action
            var act = new Func<Task>(async () => await _incidentRepository.GetWorkflowDesignList(string.Empty));

            // assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public async Task WorkflowDesign_GetWorkflowDesignById_Success()
        {
            // arrange
            var workflowDesignDto = MockWorkflowDesignResponseDto();
            _workflowDesignMicroApiMock
                .Setup(x => x.GetDetailsAsync(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .ReturnsAsync(workflowDesignDto);

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
            var act = new Func<Task>(async () => await _incidentRepository.GetWorkflowDesignById(Guid.NewGuid().ToString()));

            // assert
            await Assert.ThrowsAsync<HoneywellException>(act);
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

            _workflowDesignMicroApiMock.Setup(x => x.ImportsAsync(It.IsAny<Stream>())).
                ReturnsAsync(mockImportWorkflowDesignsResponseDto);

            // action
            await _incidentRepository.ImportWorkflowDesigns(It.IsAny<Stream>());

            // assert
            _workflowDesignMicroApiMock.Verify(x => x.ImportsAsync(It.IsAny<Stream>()), Times.Once);
        }


        [Fact]
        public async Task CreateWorkFlowDesign_Successful()
        {
            // arrange
            var mockCreateWorkflowDesignRequestGto = new CreateWorkflowDesignRequestGto
            {
                Description = "This procedure shall be completed 48hours before any events.",
                Name = "Event Prep Master 1",
                Steps ={
                    new CreateWorkflowStepDesignGto(false,
                        "Confirm with event manager on final event requirements and special needs",
                        "View event booking calendar.On the calendar, link to event contract"),
                    new CreateWorkflowStepDesignGto(true,
                        "If required by the event, engage Energy team for pre-event check.",
                        "Any cabling on special electricity supply ? Backup generators ready ? ")
                }
            };
            var mockCreateWorkflowDesignResponse = new CreateWorkflowDesignResponseDto("workflow1", Guid.NewGuid());
             _workflowDesignMicroApiMock.Setup(x => x.CreateAsync(It.IsAny<CreateWorkflowDesignRequestDto>())).
                ReturnsAsync(mockCreateWorkflowDesignResponse);

            // action
            await _incidentRepository.CreateWorkflowDesign(mockCreateWorkflowDesignRequestGto);

            // assert
            _workflowDesignMicroApiMock.Verify(x => x.CreateAsync(It.IsAny<CreateWorkflowDesignRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkFlowDesign_Successful()
        {
            // arrange
            var mockUpdateWorkflowDesignRequestGto = new UpdateWorkflowDesignRequestGto
            {
                Description = "This procedure shall be completed 48hours before any events.",
                Name = "Event Prep Master 1",
                Id = Guid.NewGuid(),
                Steps ={
                    new UpdateWorkflowStepDesignGto(false,
                        "Confirm with event manager on final event requirements and special needs",
                        "View event booking calendar.On the calendar, link to event contract"),
                    new UpdateWorkflowStepDesignGto(true,
                        "If required by the event, engage Energy team for pre-event check.",
                        "Any cabling on special electricity supply ? Backup generators ready ? ")
                }
            };
            var mockUpdateWorkflowDesignResponse = new UpdateWorkflowDesignResponseDto("workflow1", Guid.NewGuid());
            _workflowDesignMicroApiMock.Setup(x => x.UpdateAsync(It.IsAny<UpdateWorkflowDesignRequestDto>())).
                ReturnsAsync(mockUpdateWorkflowDesignResponse);

            // action
            await _incidentRepository.UpdateWorkflowDesign(mockUpdateWorkflowDesignRequestGto);

            // assert
            _workflowDesignMicroApiMock.Verify(x => x.UpdateAsync(It.IsAny<UpdateWorkflowDesignRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task DownloadWorkflowTemplate_Success()
        {
            // arrange
            _workflowDesignMicroApiMock.Setup(x => x.DownloadTemplateAsync())
                .ReturnsAsync(new WorkflowDownloadTemplateResultDto());
            var _ = await _incidentRepository.DownloadWorkflowTemplate();

            // assert
            _workflowDesignMicroApiMock.Verify(x => x.DownloadTemplateAsync(), Times.Once);
        }

        [Fact]
        public async Task ExportWorkflowDesigns_Success()
        {
            //assert
            var guidIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var strIds = guidIds.Select(x => x.ToString()).ToArray();
            var workflowsDto = new ExportWorkflowsResponseDto { WorkflowsBytes = new byte[10] };

            _workflowDesignMicroApiMock.Setup(x => x.ExportAsync(It.IsAny<ExportWorkflowRequestDto>()))
                .ReturnsAsync(workflowsDto);

            //act
            var result = await _incidentRepository.ExportWorkflowDesigns(strIds);

            //assert
            Assert.True(result.FileBytes != null && result.FileBytes.Length > 0);
        }

        [Fact]
        public async Task ExportWorkflowDesigns_ThrowException_Fail()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _incidentRepository.ExportWorkflowDesigns(null));
        }

        [Fact]
        public void GetWorkflowDesignIds_Success()
        {
            //arrange
            var listResponseDto = new WorkflowDesignListResponseDto
            {
                Lists = new List<WorkflowDesignListDto>
                {
                    new WorkflowDesignListDto
                    {
                        Description = "description",
                        Id = new Guid(),
                        Name = "name",
                        Version = 1
                    }
                }
            };

            _workflowDesignMicroApiMock.Setup(api => api.GetListAsync(It.IsAny<WorkflowDesignListRequestDto>()))
                .ReturnsAsync(listResponseDto);

            //act
            var workflowDesignIds = _incidentRepository.GetWorkflowDesignIds();

            //assert
            Assert.NotNull(workflowDesignIds.Result);
            Assert.NotNull(workflowDesignIds.Result);
            Assert.True(workflowDesignIds.Result.Any());
            Assert.Equal(workflowDesignIds.Result.First().WorkflowDesignReferenceId, listResponseDto.Lists.First().Id);
            Assert.Equal(workflowDesignIds.Result.First().Name, listResponseDto.Lists.First().Name);
        }

        [Fact]
        public void GetWorkflowDesigns_Success()
        {
            //arrange
            var workflowGuid = Guid.NewGuid();
            var setGuid = Guid.NewGuid();

            var workflowDesignResponseDto = new WorkflowDesignResponseDto
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
            };

            _workflowDesignMicroApiMock.Setup(api => api.GetDetailsAsync(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .ReturnsAsync(workflowDesignResponseDto);

            //act
            var workflowDesigns = _incidentRepository.GetWorkflowDesignDetails(new[] { workflowGuid });

            //assert
            Assert.NotNull(workflowDesigns.Result);
            Assert.NotNull(workflowDesigns.Result);
            Assert.True(workflowDesigns.Result.Any());
            Assert.Equal(workflowGuid, workflowDesignResponseDto.Details.First().Id);
            Assert.Equal(workflowDesignResponseDto.Details.First().Id, workflowDesigns.Result.First().Id);
            Assert.Equal(workflowDesignResponseDto.Details.First().Name, workflowDesigns.Result.First().Name);
            Assert.Equal(workflowDesignResponseDto.Details.First().Description, workflowDesigns.Result.First().Description);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[0].Id, workflowDesigns.Result.First().Steps[0].Id);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[0].HelpText, workflowDesigns.Result.First().Steps[0].HelpText);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[0].Instruction, workflowDesigns.Result.First().Steps[0].Instruction);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[0].IsOptional, workflowDesigns.Result.First().Steps[0].IsOptional);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[1].Id, workflowDesigns.Result.First().Steps[1].Id);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[1].HelpText, workflowDesigns.Result.First().Steps[1].HelpText);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[1].Instruction, workflowDesigns.Result.First().Steps[1].Instruction);
            Assert.Equal(workflowDesignResponseDto.Details.First().Steps[1].IsOptional, workflowDesigns.Result.First().Steps[1].IsOptional);
        }


        private WorkflowDesignListResponseDto MockWorkflowDesignListResponseDto()
        {
            var summaryResponseDto = new WorkflowDesignListResponseDto
            {
                Lists = new List<WorkflowDesignListDto>
                {
                    new WorkflowDesignListDto
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
