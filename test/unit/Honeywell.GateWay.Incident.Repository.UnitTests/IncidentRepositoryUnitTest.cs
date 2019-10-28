using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.Incident.Create;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using IncidentGTO = Honeywell.Gateway.Incident.Api.Incident;
using WorkflowGTO = Honeywell.Gateway.Incident.Api.Workflow;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api.Workflow.AddComment;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Delete;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
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
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Xunit;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident;
using IncidentPriority = Honeywell.Micro.Services.Incident.Domain.Shared.IncidentPriority;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.DownloadTemplate;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Actions;

#pragma warning disable CS0612 // Type or member is obsolete

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class IncidentRepositoryUnitTest : ApplicationServiceTestBase
    {
        private readonly Mock<IWorkflowDesignMicroApi> _workflowDesignMicroApiMock;
        private readonly Mock<IIncidentMicroApi> _mockIncidentMicroApi;
        private readonly Mock<IWorkflowMicroApi> _mockWorkflowMicroApi;
        private readonly Mock<IIncidentFacadeApi> _mockIncidentFacadeApi;

        private readonly IIncidentRepository _incidentRepository;

        public IncidentRepositoryUnitTest()
        {
            _workflowDesignMicroApiMock = new Mock<IWorkflowDesignMicroApi>();
            _mockIncidentMicroApi = new Mock<IIncidentMicroApi>();
            _mockWorkflowMicroApi = new Mock<IWorkflowMicroApi>();
            _mockIncidentFacadeApi = new Mock<IIncidentFacadeApi>();
            _incidentRepository = new IncidentRepository(
                _workflowDesignMicroApiMock.Object,
                _mockIncidentMicroApi.Object,
                _mockWorkflowMicroApi.Object,
                _mockIncidentFacadeApi.Object);
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
            var mockWorkflowResponse = new WorkflowResponseDto("workflow1", new List<string> {"duplicate name", "desc over length"});
            var mockImportWorkflowDesignsResponseDto = new ImportWorkflowDesignsResponseDto
            {
                ImportResponseList = new List<WorkflowResponseDto> {mockWorkflowResponse}
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
        public void UpdateWorkflowStepStatus_Success()
        {
            //arrange
            var workflowStepId = Guid.NewGuid();
            var mockResponse = ApiResponse.CreateSuccess().To<WorkflowActionResponseDto>();
            _mockWorkflowMicroApi
                .Setup(api => api.UpdateStepStatusAsync(It.IsAny<UpdateWorkflowStepStatusRequestDto>()))
                .ReturnsAsync(mockResponse);

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
            var mockResponse = ApiResponse.CreateFailed().To<WorkflowActionResponseDto>();
            _mockWorkflowMicroApi
                .Setup(api => api.UpdateStepStatusAsync(It.IsAny<UpdateWorkflowStepStatusRequestDto>()))
                .ReturnsAsync(mockResponse);

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
            var mockGetDetailsResponseDto = new GetDetailsResponseDto { Details = incidentDto };
            var mockResponse = ApiResponse.CreateSuccess().To(mockGetDetailsResponseDto);

            _mockIncidentFacadeApi.Setup(f => f.GetDetailsAsync(It.IsAny<GetDetailRequestDto>())).ReturnsAsync(mockResponse);

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
            var mockResponse = ApiResponse.CreateFailed().To<GetDetailsResponseDto>();
            _mockIncidentFacadeApi.Setup(x => x.GetDetailsAsync(It.IsAny<GetDetailRequestDto>()))
                .ReturnsAsync(mockResponse);

            var response = _incidentRepository.GetIncidentById(incidentId);

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
        }

        [Fact]
        public void GetIncident_IncidentIdInvalid_ThrowArgumentException()
        {
            _mockIncidentFacadeApi.Setup(x => x.GetDetailsAsync(It.IsAny<GetDetailRequestDto>()))
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
                IncidentIds = new List<Guid>()
                {
                    Guid.NewGuid()
                }
            };

            var mockResponse = ApiResponse.CreateSuccess().To(createIncidentResponse);
            _mockIncidentFacadeApi.Setup(api => api.CreateAsync(It.IsAny<CreateIncidentRequestDto>()))
                .ReturnsAsync(mockResponse);

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
            var mockResponse = ApiResponse.CreateSuccess().To<FacadeApi.IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.RespondAsync(It.Is<FacadeApi.Actions.IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

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
            var mockResponse = ApiResponse.CreateFailed().To<FacadeApi.IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.RespondAsync(It.Is<FacadeApi.Actions.IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);


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
            var mockResponse = ApiResponse.CreateSuccess().To<FacadeApi.IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.TakeoverAsync(
                        It.Is<FacadeApi.Actions.IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

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
            var mockResponse = ApiResponse.CreateFailed().To<FacadeApi.IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.TakeoverAsync(
                        It.Is<FacadeApi.Actions.IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

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
                    api.CloseAsync(It.Is<FacadeApi.Close.CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new FacadeApi.IncidentActionResponseDto());

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
            var mockResponse = ApiResponse.CreateFailed().To<FacadeApi.IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.CloseAsync(
                        It.Is<FacadeApi.Close.CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

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

            var mockIncidentResponse = ApiResponse.CreateSuccess().To(new GetIncidentListResponseDto
            {
                List = new List<IncidentListItemDto> {mockIncidentListItemDto}
            });

            _mockIncidentMicroApi.Setup(x => x.GetListAsync()).ReturnsAsync(mockIncidentResponse);

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
            };

            var mockResponse = ApiResponse.CreateSuccess().To(mockWorkflowSummaryResponse);

            _mockWorkflowMicroApi.Setup(x =>
                    x.GetSummariesAsync(
                        It.Is<WorkflowSummaryRequestDto>(request => request.WorkflowIds.Contains(workflowId))))
                .ReturnsAsync(mockResponse);

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
            Assert.Equal(mockIncidentListItemDto.Priority.ToString(), activeIncidentGto.Priority.ToString());
        }

        [Fact]
        public void GetActiveIncidentList_CallGetActiveList_Failed()
        {
            //assign
            var mockIncidentListResponse = ApiResponse.CreateFailed().To<GetIncidentListResponseDto>();
            _mockIncidentMicroApi.Setup(x => x.GetListAsync()).ReturnsAsync(mockIncidentListResponse);

            //action
            var response = _incidentRepository.GetActiveIncidentList();

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
            Assert.True(0 == response.Result.List.Count);
        }

        [Fact]
        public void CreateIncidentByAlarm_Success()
        {
            //arrange
            var workflowDesignReferenceId = Guid.NewGuid();

            var request = new IncidentGTO.Create.CreateByAlarmRequestGto
            {
                CreateDatas = new[]
                {
                    new IncidentGTO.Create.CreateByAlarmGto
                    {
                        WorkflowDesignReferenceId = workflowDesignReferenceId,
                        Priority =  Honeywell.Gateway.Incident.Api.Gtos.IncidentPriority.High,
                        Description = "incident description",
                        DeviceId = Guid.NewGuid().ToString(),
                        DeviceType = "Door",
                        AlarmId = Guid.NewGuid().ToString(),
                        AlarmData = new IncidentGTO.Create.AlarmData
                        {
                            AlarmType = "AlarmType",
                            Description = "alarm description"
                        }
                    }
                }
            };

            var createIncidentResponse = ApiResponse.CreateSuccess().To(new CreateIncidentResponseDto
            {
                IncidentIds = new List<Guid>()
                {
                    Guid.NewGuid()
                }
            });
            
            _mockIncidentFacadeApi.Setup(api => api.CreateByAlarmAsync(It.IsAny<CreateIncidentByAlarmRequestDto>()))
                .Returns(Task.FromResult(createIncidentResponse));

            //act
            var incidentIds = _incidentRepository.CreateIncidentByAlarm(request);

            //assert
            Assert.NotNull(incidentIds.Result);
            Assert.NotNull(incidentIds.Result.IncidentIds);
            Assert.True(incidentIds.Result.IncidentIds.Any());
            Assert.Equal(createIncidentResponse.Value.IncidentIds.First(), incidentIds.Result.IncidentIds.First());
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

            var request = new WorkflowGTO.Detail.GetWorkflowDesignDetailsRequestGto
            {
                Ids = new[] {workflowGuid}
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
            Assert.Equal(workflowGuid,workflowDesignResponseDto.Value.Details.First().Id);
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

        [Fact]
        public void GetIncidentStatusWithAlarmId_Success()
        {
            //arrange
            var alarmId = Guid.NewGuid().ToString();
            var incidentId = Guid.NewGuid();

            var request = new IncidentGTO.Status.GetStatusByAlarmRequestGto
            {
                AlarmIds = new[] { alarmId }
            };

            var getIncidentStatusResponse = ApiResponse.CreateSuccess().To(new GetIncidentStatusResponseDto
            {
                IncidentStatusInfos = new List<IncidentStatusDto>
                {
                    new IncidentStatusDto
                    {
                        IncidentId = incidentId,
                        TriggerId = alarmId,
                        Status = IncidentState.Active
                    }
                }
            });

            _mockIncidentMicroApi.Setup(api => api.GetStatusByTriggerAsync(It.IsAny<GetIncidentStatusRequestDto>()))
                .Returns(Task.FromResult(getIncidentStatusResponse));

            //act
            var statusWithAlarmId = _incidentRepository.GetIncidentStatusByAlarm(request);

            //assert
            Assert.NotNull(statusWithAlarmId.Result);
            Assert.NotNull(statusWithAlarmId.Result.IncidentStatusInfos);
            Assert.True(statusWithAlarmId.Result.IncidentStatusInfos.Any());
            Assert.Equal(getIncidentStatusResponse.Value.IncidentStatusInfos.First().TriggerId, alarmId);
            Assert.Equal(getIncidentStatusResponse.Value.IncidentStatusInfos.First().TriggerId, statusWithAlarmId.Result.IncidentStatusInfos.First().AlarmId);
            Assert.Equal(IncidentStatus.Active, statusWithAlarmId.Result.IncidentStatusInfos.First().Status);
            Assert.Equal(incidentId, statusWithAlarmId.Result.IncidentStatusInfos.First().IncidentId);
        }

        [Fact]
        public void AddStepComment_Success()
        {
            //arrange
            var response = new WorkflowActionResponseDto();

            _mockWorkflowMicroApi.Setup(api => api.AddStepCommentAsync(It.IsAny<AddStepCommentRequestDto>()))
                .ReturnsAsync(response);

            //act
            var addStepCommentGto = new AddStepCommentGto()
            {
                WorkflowStepId = Guid.NewGuid().ToString(),
                Comment = "this is comment"
            };
            var result = _incidentRepository.AddStepComment(addStepCommentGto);

            //assert
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
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
        #endregion

    }
}
