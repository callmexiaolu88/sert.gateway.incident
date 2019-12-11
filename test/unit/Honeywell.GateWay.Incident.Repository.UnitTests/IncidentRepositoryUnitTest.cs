using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.Incident.Create;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.GateWay.Incident.Repository.Incident;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Core.Common.Exceptions;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Actions;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Actions;
using Honeywell.Micro.Services.Workflow.Api.Workflow.AddComment;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Domain.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
using Honeywell.Infra.Services.LiveData.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.Statistics;
using Xunit;
using IncidentGTO = Honeywell.Gateway.Incident.Api.Incident;
using IncidentPriority = Honeywell.Gateway.Incident.Api.Incident.GetDetail.IncidentPriority;
using IncidentStatus = Honeywell.Gateway.Incident.Api.Incident.GetDetail.IncidentStatus;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.List;

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class IncidentRepositoryUnitTest : ApplicationServiceTestBase
    {
        private readonly Mock<IIncidentMicroApi> _mockIncidentMicroApi;
        private readonly Mock<IWorkflowMicroApi> _mockWorkflowMicroApi;
        private readonly Mock<IIncidentFacadeApi> _mockIncidentFacadeApi;
        private readonly Mock<ILiveDataApi> _mockLiveDataApi;

        private readonly IIncidentRepository _incidentRepository;

        public IncidentRepositoryUnitTest()
        {
            _mockIncidentMicroApi = new Mock<IIncidentMicroApi>();
            _mockWorkflowMicroApi = new Mock<IWorkflowMicroApi>();
            _mockIncidentFacadeApi = new Mock<IIncidentFacadeApi>();
            _mockLiveDataApi = new Mock<ILiveDataApi>();
            _incidentRepository = new IncidentRepository(
                _mockIncidentMicroApi.Object,
                _mockWorkflowMicroApi.Object,
                _mockIncidentFacadeApi.Object,
                _mockLiveDataApi.Object);
        }

        private void MockLiveData()
        {
            _mockLiveDataApi.Setup(x => x.SendEventData(It.IsAny<IncidentActivities>()));
        }

        private UpdateStepStatusRequestGto MockStepStatusGto()
        {
            var gto = new UpdateStepStatusRequestGto
            {
                IncidentId = Guid.NewGuid().ToString(), IsHandled = true, WorkflowStepId = Guid.NewGuid().ToString()
            };
            return gto;
        }

        [Fact]
        public async Task UpdateWorkflowStepStatus_Success()
        {
            //arrange
            var stepStatusGto = MockStepStatusGto();
            MockLiveData();
            _mockWorkflowMicroApi
                .Setup(api => api.UpdateStepStatusAsync(It.IsAny<UpdateWorkflowStepStatusRequestDto>()))
                .ReturnsAsync(new WorkflowActionResponseDto());

            //act
            await _incidentRepository.UpdateWorkflowStepStatus(stepStatusGto);

            //assert
            _mockWorkflowMicroApi.Verify(api => api.UpdateStepStatusAsync(It.IsAny<UpdateWorkflowStepStatusRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkflowStepStatus_Failed()
        {
            //arrange
            var stepStatusGto = MockStepStatusGto();
            var mockResponse = ApiResponse.CreateFailed().To<WorkflowActionResponseDto>();
            _mockWorkflowMicroApi
                .Setup(api => api.UpdateStepStatusAsync(It.IsAny<UpdateWorkflowStepStatusRequestDto>()))
                .ReturnsAsync(mockResponse);

            //act
            var act = new Func<Task>(async () => await _incidentRepository.UpdateWorkflowStepStatus(stepStatusGto));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public void UpdateWorkflowStepStatus_ThrowArgumentException_Failed()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _incidentRepository.UpdateWorkflowStepStatus(null));
        }

        [Fact]
        public void GetIncident_Successful()
        {
            //assign
            var incidentId = Guid.NewGuid();
            var incidentDto = MockIncidentDetailDto(incidentId);
            var mockGetDetailsResponseDto = new GetDetailsResponseDto { Details = incidentDto };

            _mockIncidentFacadeApi.Setup(f => f.GetDetailsAsync(It.IsAny<GetIncidentDetailsRequestDto>()))
                .ReturnsAsync(mockGetDetailsResponseDto);

            //action
            var response = _incidentRepository.GetIncidentById(incidentId.ToString());

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);

            var incidentGto = response.Result;
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
        public async Task GetIncident_Failed()
        {
            //arrange
            var incidentId = Guid.NewGuid().ToString();
            var mockResponse = ApiResponse.CreateFailed().To<GetDetailsResponseDto>();
            _mockIncidentFacadeApi.Setup(x => x.GetDetailsAsync(It.IsAny<GetIncidentDetailsRequestDto>()))
                .ReturnsAsync(mockResponse);

            //act
            var response = _incidentRepository.GetIncidentById(incidentId);
            var act = new Func<Task>(async () => await _incidentRepository.GetIncidentById(incidentId));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public void GetIncident_IncidentIdInvalid_ThrowArgumentException()
        {
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

            _mockIncidentFacadeApi.Setup(api => api.CreateAsync(It.IsAny<CreateIncidentRequestDto>()))
                .ReturnsAsync(createIncidentResponse);

            //act
            var incidentId = _incidentRepository.CreateIncident(request);

            //assert
            Assert.Equal(createIncidentResponse.IncidentIds.First().ToString(), incidentId.Result);
        }

        [Fact]
        public async Task CreateIncident_InvalidRequestWorkflowDesignReferenceId_ReturnError()
        {
            
            var request = new CreateIncidentRequestGto
            {
                WorkflowDesignReferenceId = null
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _incidentRepository.CreateIncident(request));
        }

        [Fact]
        public async Task CreateIncident_InvalidRequestPriority_ReturnError()
        {

            var request = new CreateIncidentRequestGto
            {
                WorkflowDesignReferenceId = Guid.NewGuid().ToString()
            };
            await Assert.ThrowsAsync<ArgumentException>(() => _incidentRepository.CreateIncident(request));
        }

        [Fact]
        public void RespondIncident_Success()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi.Setup(api => api.RespondAsync(It.IsAny<IncidentActionRequestDto>()))
                .ReturnsAsync(new IncidentActionResponseDto());

            //act
            _incidentRepository.RespondIncident(incidentId.ToString()).Wait();

            //assert
            _mockIncidentFacadeApi.Verify(api => api.RespondAsync(It.IsAny<IncidentActionRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task RespondIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";

            //act
            var act = new Func<Task>(async () => await _incidentRepository.RespondIncident(incidentId));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public async Task RespondIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            var mockResponse = ApiResponse.CreateFailed().To<IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.RespondAsync(It.Is<IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);


            //act
            var act = new Func<Task>(async () => await _incidentRepository.RespondIncident(incidentId.ToString()));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public void TakeoverIncident_Success()
        {
            //arrange
            MockLiveData();
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.TakeoverAsync(
                        It.Is<IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto());

            //act
            _incidentRepository.TakeoverIncident(incidentId.ToString()).Wait();

            //assert
            _mockIncidentFacadeApi.Verify(
                api => api.TakeoverAsync(It.IsAny<IncidentActionRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task TakeoverIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";

            //act
            var act = new Func<Task>(async () => await _incidentRepository.TakeoverIncident(incidentId));

            //assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task TakeoverIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            var mockResponse = ApiResponse.CreateFailed().To<IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.TakeoverAsync(
                        It.Is<IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

            //act
            var act = new Func<Task>(async () => await _incidentRepository.TakeoverIncident(incidentId.ToString()));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public void CloseIncident_Success()
        {
            //arrange
            MockLiveData();
            var incidentId = Guid.NewGuid();
            var reason = "close reason";
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.CloseAsync(It.Is<CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto());

            //act
            _incidentRepository.CloseIncident(incidentId.ToString(), reason).Wait();

            //assert
            _mockIncidentFacadeApi.Verify(api => api.CloseAsync(It.IsAny<CloseIncidentRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task CloseIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";
            var reason = "close reason";

            //act
            var act = new Func<Task>(async () => await _incidentRepository.CloseIncident(incidentId, reason));

            //assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task CloseIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            var reason = "close reason";
            var mockResponse = ApiResponse.CreateFailed().To<IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.CloseAsync(
                        It.Is<CloseIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

            //act
            var act = new Func<Task>(async () => await _incidentRepository.CloseIncident(incidentId.ToString(), reason));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public void CompleteIncident_Success()
        {
            //arrange
            MockLiveData();
            var incidentId = Guid.NewGuid();
            _mockIncidentFacadeApi.Setup(api =>
                    api.CompleteAsync(It.Is<IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new IncidentActionResponseDto());

            //act
            _incidentRepository.CompleteIncident(incidentId.ToString()).Wait();


            //assert
            _mockIncidentFacadeApi.Verify(api => api.CompleteAsync(It.IsAny<IncidentActionRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task CompleteIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";

            //act
            var act = new Func<Task>(async () => await _incidentRepository.CompleteIncident(incidentId));

            //assert
            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task CompleteIncident_CallMicroServiceFailed_ReturnError()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            var mockResponse = ApiResponse.CreateFailed().To<IncidentActionResponseDto>();
            _mockIncidentFacadeApi
                .Setup(api =>
                    api.CompleteAsync(
                        It.Is<IncidentActionRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(mockResponse);

            //act
            var act = new Func<Task>(async () => await _incidentRepository.CompleteIncident(incidentId.ToString()));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
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
                Priority = Micro.Services.Incident.Domain.Shared.IncidentPriority.High
            };

            var mockIncidentResponse = new GetIncidentListResponseDto
            {
                List = new List<IncidentListItemDto> {mockIncidentListItemDto}
            };

            _mockIncidentMicroApi.Setup(x => x.GetListAsync(It.IsAny<PageRequest<GetIncidentListRequestDto>>())).ReturnsAsync(mockIncidentResponse);

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


            _mockWorkflowMicroApi.Setup(x =>
                    x.GetSummariesAsync(
                        It.Is<WorkflowSummaryRequestDto>(request => request.WorkflowIds.Contains(workflowId))))
                .ReturnsAsync(mockWorkflowSummaryResponse);
            var getListRequest = MockGetIncidentListRequestDto(0, string.Empty);

            //action
            var response = _incidentRepository.GetList(getListRequest);

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(1 == response.Result.Length);
            var activeIncidentGto = response.Result[0];
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
        public async Task GetActiveIncidentList_CallGetActiveList_Failed()
        {
            //assign
            var mockIncidentListResponse = ApiResponse.CreateFailed().To<GetIncidentListResponseDto>();
            _mockIncidentMicroApi.Setup(x => x.GetListAsync(It.IsAny< PageRequest<GetIncidentListRequestDto>>())).ReturnsAsync(mockIncidentListResponse);
            var getListRequest = MockGetIncidentListRequestDto(0, string.Empty);

            //action
            var act = new Func<Task>(async () => await _incidentRepository.GetList(getListRequest));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public async Task CreateIncidentByAlarm_Success()
        {
            //arrange
            var workflowDesignReferenceId = Guid.NewGuid();

            var request = new CreateIncidentByAlarmRequestGto
            {
                WorkflowDesignReferenceId = workflowDesignReferenceId,
                Priority = IncidentPriority.High,
                Description = "incident description",
                DeviceId = Guid.NewGuid().ToString(),
                DeviceType = "Door",
                AlarmId = Guid.NewGuid().ToString(),
                AlarmData = new IncidentGTO.Create.AlarmData
                {
                    AlarmType = "AlarmType",
                    Description = "alarm description",
                    AlarmTimestamp = 1574406225000
                }
            };

            var createIncidentResponse = new CreateIncidentResponseDto
            {
                IncidentIds = new List<Guid>
                {
                    Guid.NewGuid()
                }
            };
            
            _mockIncidentFacadeApi.Setup(api => api.CreateByAlarmAsync(It.IsAny<CreateIncidentByAlarmRequestDto>()))
                .ReturnsAsync(createIncidentResponse);

            //act
            var incidentIds = await _incidentRepository.CreateIncidentByAlarm(new[] {request});

            //assert
            Assert.NotNull(incidentIds);
            Assert.True(incidentIds.Any());
            Assert.Equal(createIncidentResponse.IncidentIds.First(), incidentIds.First());
        }

        [Fact]
        public async Task GetIncidentStatusWithAlarmId_Success()
        {
            //arrange
            var alarmId = Guid.NewGuid().ToString();
            var incidentId = Guid.NewGuid();

            var getIncidentStatusResponse = new GetIncidentStatusResponseDto
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
            };

            _mockIncidentMicroApi.Setup(api => api.GetStatusByTriggerAsync(It.IsAny<GetIncidentStatusRequestDto>()))
                .ReturnsAsync(getIncidentStatusResponse);

            //act
            var statusWithAlarmId = await _incidentRepository.GetIncidentStatusByAlarm(new[] { alarmId });

            //assert
            Assert.NotNull(statusWithAlarmId);
            Assert.NotNull(statusWithAlarmId);
            Assert.True(statusWithAlarmId.Any());
            Assert.Equal(getIncidentStatusResponse.IncidentStatusInfos.First().TriggerId, statusWithAlarmId.First().AlarmId);
            Assert.Equal(IncidentStatus.Active, statusWithAlarmId.First().Status);
            Assert.Equal(incidentId, statusWithAlarmId.First().IncidentId);
        }

        [Fact]
        public async Task GetActivitysAsync_Success()
        {
            var incidentId = Guid.NewGuid();
            var incidentDto = MockIncidentDetailDto(incidentId);
            var mockGetDetailsResponseDto = new GetDetailsResponseDto { Details = incidentDto };

            _mockIncidentFacadeApi.Setup(f => f.GetDetailsAsync(It.IsAny<GetIncidentDetailsRequestDto>()))
                .ReturnsAsync(mockGetDetailsResponseDto);

            var result = await _incidentRepository.GetActivitys(incidentId.ToString());

            //var response = result.IncidentActivities.ToArray();

            Assert.NotNull(result);
            Assert.Equal(incidentDto[0].IncidentActivities[0].Description, result[0].Description);
            Assert.Equal(incidentDto[0].IncidentActivities[0].CreateAtUtc, result[0].CreateAtUtc);
            Assert.Equal(incidentDto[0].IncidentActivities[0].Operator, result[0].Operator);
        }

        [Fact]
        public async Task AddStepComment_Success()
        {
            //arrange
            MockLiveData();
            var response = new WorkflowActionResponseDto();

            _mockWorkflowMicroApi.Setup(api => api.AddStepCommentAsync(It.IsAny<AddStepCommentRequestDto>()))
                .ReturnsAsync(response);

            //act
            var addStepCommentGto = new AddStepCommentRequestGto()
            {
                WorkflowStepId = Guid.NewGuid().ToString(),
                Comment = "this is comment",
                IncidentId = Guid.NewGuid().ToString()
            };
            await _incidentRepository.AddStepComment(addStepCommentGto);

            //assert
            _mockWorkflowMicroApi.Verify(api => api.AddStepCommentAsync(It.IsAny<AddStepCommentRequestDto>()), Times.Once);


        }

        [Fact]
        public async Task AddStepComment_IncidentIdException_Success()
        {
            //arrange
            MockLiveData();
            var response = new WorkflowActionResponseDto();

            _mockWorkflowMicroApi.Setup(api => api.AddStepCommentAsync(It.IsAny<AddStepCommentRequestDto>()))
                .ReturnsAsync(response);

            //act
            var addStepCommentGto = new AddStepCommentRequestGto()
            {
                WorkflowStepId = Guid.NewGuid().ToString(),
                Comment = "this is comment",
                IncidentId = "wrong incidentId"
            };
            await _incidentRepository.AddStepComment(addStepCommentGto);

            //assert
            _mockWorkflowMicroApi.Verify(api => api.AddStepCommentAsync(It.IsAny<AddStepCommentRequestDto>()), Times.Once);

        }


        [Fact]
        public async Task GetStatisticsAsync_Success()
        {
            //arrange
            var deviceId = Guid.NewGuid().ToString();
            var request = new GetIncidentStatisticsRequestDto {DeviceIds = new[] {deviceId}};
            var response = new GetIncidentStatisticstResponseDto();
            response.StatisticsIncident.Add(new IncidentStatisticsDto {ActiveCount = 1,CloseCount = 1,CompletedCount = 1,DeviceId = deviceId});
            _mockIncidentMicroApi.Setup(x => x.GetStatisticsAsync(It.IsAny<GetIncidentStatisticsRequestDto>())).ReturnsAsync(response);

            //act
           var result =  await _incidentRepository.GetStatistics(deviceId);

            //asser.
            Assert.NotNull(result);
            Assert.Equal(response.StatisticsIncident[0].DeviceId, result.DeviceId);
            Assert.Equal(response.StatisticsIncident[0].ActiveCount, result.ActiveCount);
            Assert.Equal(response.StatisticsIncident[0].CloseCount, result.CloseCount);
            Assert.Equal(response.StatisticsIncident[0].CompletedCount, result.CompletedCount);
        }


        [Fact]
        public async Task GetList_Successful()
        {
            //assign
            var workflowId = Guid.NewGuid();
            var deviceId = Guid.NewGuid().ToString();
            var deviceType = "Door";
            var mockIncidentListItemDto = new IncidentListItemDto
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                DeviceType = deviceType,
                WorkflowId = workflowId,
                CreateAtUtc = DateTime.Now,
                Owner = "Admin1",
                State = IncidentState.Active,
                Number = 10,
                Priority = Micro.Services.Incident.Domain.Shared.IncidentPriority.High
            };

            var mockIncidentResponse = new GetIncidentListResponseDto
            {
                List = new List<IncidentListItemDto> { mockIncidentListItemDto }
            };

            var request = new GetIncidentListRequestDto { State = IncidentState.Active,DeviceId = deviceId };

            _mockIncidentMicroApi.Setup(x => x.GetListAsync(It.IsAny<PageRequest<GetIncidentListRequestDto>>())).ReturnsAsync(mockIncidentResponse);

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


            _mockWorkflowMicroApi.Setup(x =>
                    x.GetSummariesAsync(
                        It.Is<WorkflowSummaryRequestDto>(request => request.WorkflowIds.Contains(workflowId))))
                .ReturnsAsync(mockWorkflowSummaryResponse);
            var getListRequest = MockGetIncidentListRequestDto(0, deviceId);


            //action
            var response = _incidentRepository.GetList(getListRequest);

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(1 == response.Result.Length);
            var activeIncidentGto = response.Result[0];
            Assert.Equal(workflowId, activeIncidentGto.WorkflowId);
            Assert.Equal(mockWorkflowSummary.WorkflowDesignName, activeIncidentGto.WorkflowDesignName);
            Assert.Equal(mockWorkflowSummary.TotalSteps, activeIncidentGto.TotalSteps);
            Assert.Equal(mockWorkflowSummary.CompletedSteps, activeIncidentGto.CompletedSteps);
            Assert.Equal(mockIncidentListItemDto.CreateAtUtc, activeIncidentGto.CreateAtUtc);
            Assert.Equal(mockIncidentListItemDto.Owner, activeIncidentGto.Owner);
            Assert.Equal(mockIncidentListItemDto.Number, activeIncidentGto.Number);
            Assert.Equal(mockIncidentListItemDto.Priority.ToString(), activeIncidentGto.Priority.ToString());
        }

        private PageRequest<GetListRequestGto> MockGetIncidentListRequestDto(int state,string deviceId)
        {
            var request = new GetListRequestGto {State = state, DeviceId = deviceId};
            var pageRequest = new PageRequest().To(request);
            return pageRequest;
        }

        [Fact]
        public async Task GetList_CallGetIncidentsList_Failed()
        {
            //assign
            var mockIncidentListResponse = ApiResponse.CreateFailed().To<GetIncidentListResponseDto>();
            _mockIncidentMicroApi.Setup(x => x.GetListAsync(It.IsAny< PageRequest<GetIncidentListRequestDto>>())).ReturnsAsync(mockIncidentListResponse);
            var getListRequest = MockGetIncidentListRequestDto(0, Guid.NewGuid().ToString());
            //action
            var act = new Func<Task>(async () => await _incidentRepository.GetList(getListRequest));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
        }

        [Fact]
        public async Task GetList_CallGetWorkflowList_Failed()
        {
            //assign
            var workflowId = Guid.NewGuid();
            var deviceId = Guid.NewGuid().ToString();
            var deviceType = "Door";
            var mockIncidentListItemDto = new IncidentListItemDto
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                DeviceType = deviceType,
                WorkflowId = workflowId,
                CreateAtUtc = DateTime.Now,
                Owner = "Admin1",
                State = IncidentState.Active,
                Number = 10,
                Priority = Micro.Services.Incident.Domain.Shared.IncidentPriority.High
            };

            var mockIncidentResponse = new GetIncidentListResponseDto
            {
                List = new List<IncidentListItemDto> { mockIncidentListItemDto }
            };
            
            _mockIncidentMicroApi.Setup(x => x.GetListAsync(It.IsAny<PageRequest<GetIncidentListRequestDto>>())).ReturnsAsync(mockIncidentResponse);

            var mockIncidentListResponse = ApiResponse.CreateFailed().To<WorkflowSummaryResponseDto>();
            _mockWorkflowMicroApi.Setup(x =>
                    x.GetSummariesAsync(
                        It.Is<WorkflowSummaryRequestDto>(request => request.WorkflowIds.Contains(workflowId))))
                .ReturnsAsync(mockIncidentListResponse);
            var getListRequest = MockGetIncidentListRequestDto(0, Guid.NewGuid().ToString());

            //action
            var act = new Func<Task>(async () => await _incidentRepository.GetList(getListRequest));

            //assert
            await Assert.ThrowsAsync<HoneywellException>(act);
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

        private WorkflowDesignListResponseDto MockWorkflowDesignListResponseDto()
        {
            var listResponseDto = new WorkflowDesignListResponseDto
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
            return listResponseDto;
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


        #endregion

    }
}
