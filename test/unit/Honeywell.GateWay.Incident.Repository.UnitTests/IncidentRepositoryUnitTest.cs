using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.Incident.Create;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Export;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Import;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Honeywell.Micro.Services.Workflow.Domain.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Micro.Services.Workflow.Api.Workflow;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Action;
using Xunit;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident;
using IncidentPriority = Honeywell.Micro.Services.Incident.Domain.Shared.IncidentPriority;

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class IncidentRepositoryUnitTest : ApplicationServiceTestBase
    {
        private readonly Mock<IWorkflowDesignApi> _workflowDesignApiMock;
        private readonly Mock<IIncidentMicroApi> _mockIncidentMicroApi;
        private readonly Mock<IWorkflowInstanceApi> _mockWorkflowInstanceApi;
        private readonly Mock<IIncidentFacadeApi> _mockIncidentFacadeApi;

        private readonly IIncidentRepository _incidentRepository;

        public IncidentRepositoryUnitTest()
        {
            _workflowDesignApiMock = new Mock<IWorkflowDesignApi>();
            _mockIncidentMicroApi = new Mock<IIncidentMicroApi>();
            _mockWorkflowInstanceApi = new Mock<IWorkflowInstanceApi>();
            _mockIncidentFacadeApi = new Mock<IIncidentFacadeApi>();
            _incidentRepository = new IncidentRepository(
                _workflowDesignApiMock.Object,
                _mockIncidentMicroApi.Object,
                _mockWorkflowInstanceApi.Object,
                _mockIncidentFacadeApi.Object);
        }

        [Fact]
        public async Task WorkflowDesign_DeleteWorkflowDesigns_Success()
        {
            // arrange
            _workflowDesignApiMock.Setup(x => x.Deletes(It.IsAny<WorkflowDesignDeleteRequestDto>()))
                .Returns(Task.FromResult(new ApiResponse { IsSuccess = true }));

            // action
            var result = await _incidentRepository.DeleteWorkflowDesigns(new[] { It.IsAny<Guid>().ToString() });

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
            var result = await _incidentRepository.DeleteWorkflowDesigns(new[] { It.IsAny<Guid>().ToString() });

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
     
            _workflowDesignApiMock.Setup(x => x.GetSelector()).Returns(Task.FromResult(selectorResponseDto));

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
            var summaryResponseDto = new WorkflowDesignSummaryResponseDto { IsSuccess = false, Message = "error 1" };
            _workflowDesignApiMock.Setup(x => x.GetSummaries()).Returns(Task.FromResult(summaryResponseDto));

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
            _workflowDesignApiMock
                .Setup(x => x.GetDetails(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowDesignDto));

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
            var workflowDesignDto = new WorkflowDesignResponseDto { IsSuccess = false, Message = "error 1" };
            _workflowDesignApiMock
                .Setup(x => x.GetDetails(It.IsAny<WorkflowDesignDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowDesignDto));

            // action
            var result = await _incidentRepository.GetWorkflowDesignById(Guid.NewGuid().ToString());

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ImportWorkFlowDesign_SuccessfulAsync()
        {
            // arrange
            var responseDto = new ImportWorkflowDesignsResponseDto { IsSuccess = false };
            responseDto.ImportResponseList.Add(new WorkflowResponseDto("workflow1", new List<string> { "duplicate name", "desc over length" }));
            _workflowDesignApiMock.Setup(x => x.Imports(It.IsAny<Stream>())).
                Returns(Task.FromResult(responseDto));
            // action

            var result = await _incidentRepository.ImportWorkflowDesigns(It.IsAny<Stream>());

            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DownloadWorkflowTemplate_Success()
        {
            var responseDto = new WorkflowDownloadTemplateResultDto() { IsSuccess = true };
            _workflowDesignApiMock.Setup(x => x.DownloadTemplate()).Returns(Task.FromResult(responseDto));
            var result = await _incidentRepository.DownloadWorkflowTemplate();
            // assert
            Assert.True(result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public async Task ExportWorkflowDesigns_Success()
        {
            var guidIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            string[] strIds = guidIds.Select(x => x.ToString()).ToArray();
            ExportWorkflowsResponseDto workflowsDto = new ExportWorkflowsResponseDto() { IsSuccess = true, WorkflowsBytes = new byte[10] };

            _workflowDesignApiMock.Setup(x => x.ExportWorkflows(It.IsAny<ExportWorkflowRequestDto>())).Returns(Task.FromResult(workflowsDto));
            var result = await _incidentRepository.ExportWorkflowDesigns(strIds);

            // assert
            Assert.True(result.Status == ExecuteStatus.Successful);
            Assert.True(result.FileBytes != null && result.FileBytes.Length > 0);
        }

        [Fact]
        public void UpdateWorkflowStepStatus_Success()
        {
            //arrange
            var workflowStepId = Guid.NewGuid();
            _mockWorkflowInstanceApi
                .Setup(api => api.UpdateWorkflowStepStatus(It.IsAny<UpdateWorkflowStepStatusRequestDto>()))
                .ReturnsAsync(new WorkflowActionResponseDto() { IsSuccess = true });

            //act
            var result = _incidentRepository.UpdateWorkflowStepStatus(workflowStepId.ToString(), true);

            //assert
            Assert.Equal(ExecuteStatus.Successful, result.Result.Status);
        }

        [Fact]
        public void UpdateWorkflowStepStatus_Failed()
        {
            //arrange
            var workflowStepId = Guid.NewGuid();
            _mockWorkflowInstanceApi
                .Setup(api => api.UpdateWorkflowStepStatus(It.IsAny<UpdateWorkflowStepStatusRequestDto>()))
                .ReturnsAsync(new WorkflowActionResponseDto() { IsSuccess = false });

            //act
            var result = _incidentRepository.UpdateWorkflowStepStatus(workflowStepId.ToString(), true);

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void GetIncident_Successful()
        {
            //assign
            var incidentId = Guid.NewGuid();
            var incidentDto = MockIncidentDetailDto(incidentId);
            var mockGetDetailsResponseDto = new GetDetailsResponseDto { IsSuccess = true, Details = incidentDto };
            _mockIncidentFacadeApi.Setup(f => f.GetDetails(It.IsAny<GetDetailRequestDto>())).ReturnsAsync(mockGetDetailsResponseDto);

            //action
            var response = _incidentRepository.GetIncidentById(incidentId.ToString());

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            var incidentGto = response.Result;
            Assert.True(incidentGto.Status == ExecuteStatus.Successful);
            Assert.Equal((int)incidentGto.State, incidentDto[0].State);
            Assert.Equal(incidentGto.LastUpdateAtUtc, incidentDto[0].LastUpdateAtUtc);
            Assert.Equal(incidentGto.Description, incidentDto[0].Description);
            Assert.Equal(incidentGto.Id, incidentDto[0].Id);
            Assert.Equal(incidentGto.Number, incidentDto[0].Number);
            Assert.Equal((int)incidentGto.Priority, incidentDto[0].Priority);
            Assert.Equal(incidentGto.Owner, incidentDto[0].Owner);
            Assert.Equal(incidentGto.CreateAtUtc, incidentDto[0].CreateAtUtc);
            Assert.Equal(incidentGto.WorkflowName, incidentDto[0].WorkflowName);
            Assert.Equal(incidentGto.WorkflowDescription, incidentDto[0].WorkflowDescription);
            Assert.Equal(incidentGto.WorkflowOwner, incidentDto[0].WorkflowOwner);
            Assert.Equal(incidentGto.IncidentSteps[0].Id, incidentDto[0].WorkflowSteps[0].Id);
            Assert.Equal(incidentGto.IncidentSteps[0].HelpText, incidentDto[0].WorkflowSteps[0].HelpText);
            Assert.Equal(incidentGto.IncidentSteps[0].Instruction, incidentDto[0].WorkflowSteps[0].Instruction);
            Assert.Equal(incidentGto.IncidentSteps[0].IsHandled, incidentDto[0].WorkflowSteps[0].IsHandled);
            Assert.Equal(incidentGto.IncidentSteps[0].IsOptional, incidentDto[0].WorkflowSteps[0].IsOptional);
            Assert.Equal(incidentGto.IncidentActivities[0].Description, incidentDto[0].IncidentActivities[0].Description);
            Assert.Equal(incidentGto.IncidentActivities[0].CreateAtUtc, incidentDto[0].IncidentActivities[0].CreateAtUtc);
            Assert.Equal(incidentGto.IncidentActivities[0].Operator, incidentDto[0].IncidentActivities[0].Operator);
        }

        [Fact]
        public void GetIncident_Failed()
        {
            var incidentId = Guid.NewGuid().ToString();
            _mockIncidentFacadeApi.Setup(x => x.GetDetails(It.IsAny<GetDetailRequestDto>()))
                .ReturnsAsync(new GetDetailsResponseDto { IsSuccess = false });

            var response = _incidentRepository.GetIncidentById(incidentId);

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
        }

        [Fact]
        public void GetIncident_IncidentIdInvalid_ThrowArgumentException()
        {
            _mockIncidentFacadeApi.Setup(x => x.GetDetails(It.IsAny<GetDetailRequestDto>()))
                .ReturnsAsync(It.IsAny<GetDetailsResponseDto>());

            var throwsAsync = Assert.ThrowsAsync<ArgumentException>(() => _incidentRepository.GetIncidentById(null));
            Assert.Equal("incidentId", throwsAsync.Result.ParamName);
        }

        [Fact]
        public void CreateIncident_Success()
        {
            //arrange
            var workflowDesignReferenceId = Guid.NewGuid().ToString();
            var priority = "HIGH";
            var description = "description";

            var request = new CreateIncidentRequestGto
            {
                WorkflowDesignReferenceId = workflowDesignReferenceId,
                Priority = priority,
                Description = description
            };

            var createIncidentResponse = new CreateIncidentResponseDto
            {
                IsSuccess = true,
                IncidentIds = new List<Guid>()
                {
                    Guid.NewGuid()
                }
            };

            _mockIncidentFacadeApi.Setup(api => api.CreateIncident(It.IsAny<CreateIncidentRequestDto>()))
                .Returns(Task.FromResult(createIncidentResponse));

            //act
            var incidentId = _incidentRepository.CreateIncident(request);

            //assert
            Assert.Equal(createIncidentResponse.IncidentIds.First().ToString(), incidentId.Result);
        }

        [Fact]
        public void RespondIncident_Success()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.RespondIncident(It.Is<FacadeApi.Respond.RespondIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto { IsSuccess = true });

            //act
            var result = _incidentRepository.RespondIncident(incidentId.ToString());

            //assert
            Assert.Equal(ExecuteStatus.Successful, result.Result.Status);
        }

        [Fact]
        public void RespondIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";

            //act
            var result = _incidentRepository.RespondIncident(incidentId);

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void RespondIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.RespondIncident(It.Is<FacadeApi.Respond.RespondIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto { IsSuccess = false });


            //act
            var result = _incidentRepository.RespondIncident(incidentId.ToString());

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void TakeoverIncident_Success()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.TakeoverIncident(It.Is<FacadeApi.Takeover.TakeoverIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto { IsSuccess = true });

            //act
            var result = _incidentRepository.TakeoverIncident(incidentId.ToString());

            //assert
            Assert.Equal(ExecuteStatus.Successful, result.Result.Status);
        }

        [Fact]
        public void TakeoverIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";

            //act
            var result = _incidentRepository.TakeoverIncident(incidentId);

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void TakeoverIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.TakeoverIncident(It.Is<FacadeApi.Takeover.TakeoverIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto() { IsSuccess = false });

            //act
            var result = _incidentRepository.TakeoverIncident(incidentId.ToString());

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void CloseIncident_Success()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            var reason = "close reason";
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.CloseIncident(It.Is<FacadeApi.Close.CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto { IsSuccess = true });

            //act
            var result = _incidentRepository.CloseIncident(incidentId.ToString(), reason);

            //assert
            Assert.Equal(ExecuteStatus.Successful, result.Result.Status);
        }

        [Fact]
        public void CloseIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";
            var reason = "close reason";

            //act
            var result = _incidentRepository.CloseIncident(incidentId, reason);

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void CloseIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            var reason = "close reason";
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.CloseIncident(It.Is<FacadeApi.Close.CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto { IsSuccess = false });

            //act
            var result = _incidentRepository.CloseIncident(incidentId.ToString(), reason);

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }

        [Fact]
        public void GetActiveIncidentList_Successful()
        {
            //assign
            var workflowId = Guid.NewGuid();
            var mockIncidentListItemDto = new IncidentListItemDto
            {
                Id = Guid.NewGuid(),
                WorkflowId = workflowId,
                CreateAtUtc = DateTime.Now,
                Owner = "Admin1",
                State = IncidentState.Active,
                Number = 10,
                Priority = IncidentPriority.High
            };

            var mockIncidentResponse = new GetIncidentListResponseDto()
            {
                List = new List<IncidentListItemDto> { mockIncidentListItemDto },
                IsSuccess = true,
                Message = "Any Valid Message"
            };

            _mockIncidentMicroApi.Setup(x => x.GetActiveList()).Returns(Task.FromResult(mockIncidentResponse));

            var mockWorkflowSummary = new WorkflowSummaryDto
            {
                WorkflowId = workflowId,
                Number = 1,
                Owner = "Admin1",
                WorkflowDesignName = "Any Valid WorkflowDesignName",
                Status = WorkflowStatus.Active,
                TotalSteps = 6,
                CompletedSteps = 3
            };

            var mockWorkflowSummaryResponse = new WorkflowSummaryResponseDto()
            {
                Summaries = new List<WorkflowSummaryDto> { mockWorkflowSummary },
                IsSuccess = true,
                Message = "Any Valid Message"
            };
            _mockWorkflowInstanceApi.Setup(x =>
                    x.GetWorkflowSummaries(
                        It.Is<WorkflowSummaryRequestDto>(request => request.WorkflowIds.Contains(workflowId))))
                .ReturnsAsync(mockWorkflowSummaryResponse);

            //action
            var response = _incidentRepository.GetActiveIncidentList();

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(1 == response.Result.List.Count);
            var activeIncidentGto = response.Result.List[0];
            Assert.Equal(workflowId, activeIncidentGto.WorkflowId);
            Assert.Equal(mockWorkflowSummary.WorkflowDesignName, activeIncidentGto.WorkflowDesignName);
            Assert.Equal(mockWorkflowSummary.TotalSteps, activeIncidentGto.TotalSteps);
            Assert.Equal(mockWorkflowSummary.CompletedSteps, activeIncidentGto.CompletedSteps);
            Assert.Equal(mockIncidentListItemDto.CreateAtUtc, activeIncidentGto.CreateAtUtc);
            Assert.Equal(mockIncidentListItemDto.Owner, activeIncidentGto.Owner);
            Assert.Equal(mockIncidentListItemDto.Number, activeIncidentGto.Number);
            Assert.Equal(mockIncidentListItemDto.Priority, activeIncidentGto.Priority);
        }

        [Fact]
        public void GetActiveIncidentList_CallGetActiveList_Failed()
        {
            //assign
            var mockIncidentListResponse = new GetIncidentListResponseDto()
            {
                List = new List<IncidentListItemDto>(),
                IsSuccess = false,
                Message = "Any Valid Message"
            };
            _mockIncidentMicroApi.Setup(x => x.GetActiveList()).ReturnsAsync(mockIncidentListResponse);

            //action
            var response = _incidentRepository.GetActiveIncidentList();

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
            Assert.True(0 == response.Result.List.Count);
        }

        #region private methods

        private List<IncidentDetailDto> MockIncidentDetailDto(Guid incidentId)
        {
            return new List<IncidentDetailDto>
            {
                new IncidentDetailDto
                {
                    Id = incidentId,
                    Description = "Any Incident Desc",
                    Number = 1,
                    CreateAtUtc = DateTime.Now,
                    LastUpdateAtUtc = DateTime.Now,
                    State = 1,
                    Priority = 2,
                    Owner = "Admin1",
                    WorkflowId = Guid.NewGuid(),
                    WorkflowDescription = "WorkflowDescription",
                    WorkflowName = "WorkflowName",
                    WorkflowOwner = "WorkflowOwner",
                    WorkflowSteps = new List<WorkflowStepDto>{ new WorkflowStepDto{ HelpText = "HelpText" } },
                    IncidentActivities = new List<ActivityDto>{new ActivityDto{Operator = "Operator" } },
                }
            };
        }

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

        private WorkflowDesignSelectorResponseDto MockWorkflowDesignSelectorResponseDto()
        {
            var selectorResponseDto = new WorkflowDesignSelectorResponseDto
            {
                IsSuccess = true,
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
