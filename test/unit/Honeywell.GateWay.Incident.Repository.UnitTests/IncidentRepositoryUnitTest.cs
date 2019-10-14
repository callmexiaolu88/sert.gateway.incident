using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.CreateIncident;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident;
using Honeywell.Micro.Services.Incident.Api.Incident.Close;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Respond;
using Honeywell.Micro.Services.Incident.Api.Incident.Takeover;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
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
using Xunit;
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
        public async Task WorkflowDesign_GetWorkflowDesignSelectorsByName_Success()
        {
            // arrange
            var selectorResponseDto = MockWorkflowDesignSelectorResponseDto();
            var workflowDesignSelectorRequestDto = new WorkflowDesignSelectorRequestDto()
            {
                WorkflowName = ""
            };
            _workflowDesignApiMock.Setup(x => x.GetSelector(It.IsAny<WorkflowDesignSelectorRequestDto>())).Returns(Task.FromResult(selectorResponseDto));

            // action
            var result = await _incidentRepository.GetWorkflowDesignSelectorsByName(workflowDesignSelectorRequestDto.WorkflowName);

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
        public void GetIncident_Successful()
        {

            //assign
            var incidentId = Guid.NewGuid();
            var incidentDto = MockIncidentDtos(incidentId);
            var incidentResponse = MockIncidentResponse(true, incidentDto);
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(Task.FromResult(incidentResponse));

            var workflowSteps = MockWorkflowSteps();
            var workflows = MocksWorkflowDtos(workflowSteps);
            var workflowResponse = MockWorkflowResponse(true, workflows);
            _mockWorkflowInstanceApi.Setup(x => x.GetWorkflowDetails(It.IsAny<WorkflowDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowResponse));

            //action
            var response = _incidentRepository.GetIncidentById(incidentId.ToString());

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            var incidentGto = response.Result;
            Assert.True(incidentGto.Status == ExecuteStatus.Successful);
            Assert.Equal(incidentGto.State, incidentDto[0].State);
            Assert.Equal(incidentGto.LastUpdateAtUtc, incidentDto[0].LastUpdateAtUtc);
            Assert.Equal(incidentGto.Description, incidentDto[0].Description);
            Assert.Equal(incidentGto.Id, incidentDto[0].Id);
            Assert.Equal(incidentGto.Number, incidentDto[0].Number);
            Assert.Equal(incidentGto.Priority, incidentDto[0].Priority);
            Assert.Equal(incidentGto.Owner, incidentDto[0].Owner);
            Assert.Equal(incidentGto.CreateAtUtc, incidentDto[0].CreateAtUtc);
            Assert.Equal(incidentGto.State, incidentDto[0].State);
            Assert.Equal(incidentGto.WorkflowName, workflows[0].Name);
            Assert.Equal(incidentGto.WorkflowDescription, workflows[0].Description);
            Assert.Equal(incidentGto.WorkflowOwner, workflows[0].Owner);
            Assert.Equal(incidentGto.IncidentSteps[0].Id, workflowSteps[0].Id);
            Assert.Equal(incidentGto.IncidentSteps[0].HelpText, workflowSteps[0].HelpText);
            Assert.Equal(incidentGto.IncidentSteps[0].Instruction, workflowSteps[0].Instruction);
            Assert.Equal(incidentGto.IncidentSteps[0].IsComplete, workflowSteps[0].IsComplete);
            Assert.Equal(incidentGto.IncidentSteps[0].IsOptional, workflowSteps[0].IsOptional);

        }


        [Fact]
        public void GetIncident_Incident_NotFound()
        {
            var incidentId = Guid.NewGuid().ToString();
            var incidentResponse = Task.FromResult(MockIncidentResponse(false, new List<IncidentDto>()));
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(incidentResponse);
            var response = _incidentRepository.GetIncidentById(incidentId);
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
        }

        [Fact]
        public void GetIncident_Incident_ErrorNotFound()
        {
            var incidentId = Guid.NewGuid().ToString();
            var incidentResponse = MockIncidentResponse(false, new List<IncidentDto>());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(Task.FromResult(incidentResponse));
            var response = _incidentRepository.GetIncidentById(incidentId);
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
        }

        [Fact]
        public void GetIncident_Workflow_NotFound()
        {
            var incidentId = Guid.NewGuid();
            var incidentResponse = MockIncidentResponse(true, MockIncidentDtos(incidentId));
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(Task.FromResult(incidentResponse));
            var workflowResponse = MockWorkflowResponse(false, MocksWorkflowDtos(MockWorkflowSteps()));
            _mockWorkflowInstanceApi.Setup(x => x.GetWorkflowDetails(It.IsAny<WorkflowDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowResponse));
            var response = _incidentRepository.GetIncidentById(incidentId.ToString());
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
        }

        [Fact]
        public void GetIncident_Workflow_Error_NotFound()
        {
            var incidentId = Guid.NewGuid();
            var incidentResponse = MockIncidentResponse(true, MockIncidentDtos(incidentId));
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(Task.FromResult(incidentResponse));
            var workflowResponse = MockWorkflowResponse(false, new List<WorkflowDto>());
            _mockWorkflowInstanceApi.Setup(x => x.GetWorkflowDetails(It.IsAny<WorkflowDetailsRequestDto>()))
                .Returns(Task.FromResult(workflowResponse));
            var response = _incidentRepository.GetIncidentById(incidentId.ToString());
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
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
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Respond(It.Is<RespondIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto { IsSuccess = true });

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
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Respond(It.Is<RespondIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto { IsSuccess = false });


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
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Takeover(It.Is<TakeoverIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto { IsSuccess = true });

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
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Takeover(It.Is<TakeoverIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto { IsSuccess = false });

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
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Close(It.Is<CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto { IsSuccess = true });

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
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Close(It.Is<CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto { IsSuccess = false });

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

        private List<WorkflowDto> MocksWorkflowDtos(List<WorkflowStepDto> workflowSteps)
        {
            var workflows = new List<WorkflowDto>
            {
                new WorkflowDto()
                {
                    Id = Guid.NewGuid(),
                    Description = "Any Incident Desc",
                    Name = "SOP1",
                    Number = 1,
                    Status = WorkflowStatus.Active,
                    Version = 1,
                    Owner = "Admin1",
                    WorkflowSteps = workflowSteps.ToArray()
                }
            };
            return workflows;
        }

        private List<WorkflowStepDto> MockWorkflowSteps()
        {
            var workflowSteps = new List<WorkflowStepDto>();
            var workflowStep = new WorkflowStepDto
            {
                Id = Guid.NewGuid(),
                HelpText = "Any Help Test",
                Instruction = "Step Instruction",
                IsComplete = false,
                IsOptional = true
            };
            workflowSteps.Add(workflowStep);
            return workflowSteps;
        }

        private List<IncidentDto> MockIncidentDtos(Guid incidentId)
        {
            var incidents = new List<IncidentDto>
            {
                new IncidentDto
                {
                    Id = incidentId,
                    Description = "Any Incident Desc",
                    Number = 1,
                    CreateAtUtc = DateTime.Now,
                    LastUpdateAtUtc = DateTime.Now,
                    Owner = "Admin1",
                    Priority = IncidentPriority.Low,
                    State = IncidentState.Active,
                    WorkflowId = Guid.NewGuid(),


                }
            };
            return incidents;
        }


        private WorkflowDetailsResponseDto MockWorkflowResponse(bool isSuccess, List<WorkflowDto> details)
        {
            var response = new WorkflowDetailsResponseDto()
            {
                Details = details,
                IsSuccess = isSuccess,
                Message = "Any Valid Message"
            };
            return response;
        }


        private GetIncidentDetailsResponseDto MockIncidentResponse(bool isSuccess, List<IncidentDto> details)
        {
            var response = new GetIncidentDetailsResponseDto()
            {
                Details = details,
                IsSuccess = isSuccess,
                Message = "Any Valid Message"
            };
            return response;
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
