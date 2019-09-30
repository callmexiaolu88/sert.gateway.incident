using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Facade.Services.Incident.Api.CreateIncident;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Close;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Respond;
using Honeywell.Micro.Services.Incident.Api.Incident.Takeover;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Domain.Shared;
using Moq;
using Xunit;
using IncidentPriority = Honeywell.Micro.Services.Incident.Domain.Shared.IncidentPriority;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class IncidentAppServiceTest : ApplicationServiceTestBase
    {
        private readonly Mock<IIncidentMicroApi> _mockIncidentMicroApi;
        private readonly Mock<IWorkflowInstanceApi> _mockWorkflowInstanceApi;
        private readonly Mock<IIncidentFacadeApi> _mockIncidentFacadeApi;
        private readonly Mock<IDeviceRepository> _mockDeviceRespository;

        private readonly IIncidentAppService _testObj;

        public IncidentAppServiceTest()
        {
            var mockWorkflowDesignApi = new Mock<IWorkflowDesignApi>();
            _mockIncidentMicroApi = new Mock<IIncidentMicroApi>();
            _mockWorkflowInstanceApi = new Mock<IWorkflowInstanceApi>();
            _mockIncidentFacadeApi = new Mock<IIncidentFacadeApi>();
            _mockDeviceRespository = new Mock<IDeviceRepository>();
            _testObj = new IncidentAppService(
                mockWorkflowDesignApi.Object, 
                _mockIncidentMicroApi.Object,
                _mockWorkflowInstanceApi.Object, 
                _mockIncidentFacadeApi.Object,
                _mockDeviceRespository.Object);
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
            var response = _testObj.GetIncidentById(incidentId.ToString());

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
        public void GetIncident_Incident_ErrorNotFound()
        {
            var incidentId = Guid.NewGuid().ToString();
            var incidentResponse = MockIncidentResponse(false, new List<IncidentDto>());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(Task.FromResult(incidentResponse));
            var response = _testObj.GetIncidentById(incidentId);
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
            var response = _testObj.GetIncidentById(incidentId.ToString());
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Status == ExecuteStatus.Error);
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
                List = new List<IncidentListItemDto> {mockIncidentListItemDto},
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
                Summaries = new List<WorkflowSummaryDto> {mockWorkflowSummary},
                IsSuccess = true,
                Message = "Any Valid Message"
            };

            _mockWorkflowInstanceApi.Setup(x =>
                    x.GetWorkflowSummaries(
                        It.Is<WorkflowSummaryRequestDto>(request => request.WorkflowIds.Contains(workflowId))))
                .ReturnsAsync(mockWorkflowSummaryResponse);

            //action
            var response = _testObj.GetActiveIncidentList();

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(1 == response.Result.List.Count);
            var activeIncidentGto = response.Result.List[0];
            Assert.Equal(workflowId,activeIncidentGto.WorkflowId);
            Assert.Equal(mockWorkflowSummary.WorkflowDesignName,activeIncidentGto.WorkflowDesignName);
            Assert.Equal(mockWorkflowSummary.TotalSteps,activeIncidentGto.TotalSteps);
            Assert.Equal(mockWorkflowSummary.CompletedSteps,activeIncidentGto.CompletedSteps);
            Assert.Equal(mockIncidentListItemDto.CreateAtUtc,activeIncidentGto.CreateAtUtc);
            Assert.Equal(mockIncidentListItemDto.Owner,activeIncidentGto.Owner);
            Assert.Equal(mockIncidentListItemDto.Number,activeIncidentGto.Number);
            Assert.Equal(mockIncidentListItemDto.Priority,activeIncidentGto.Priority);
        }

        [Fact]
        public void GetActiveIncidentList_ActiveIncidentListCount_EqualZero()
        {
            //assign
            var incidentListResponse = Task.FromResult(MockIncidentListResponse(false, new List<IncidentListItemDto>()));
            _mockIncidentMicroApi.Setup(x => x.GetActiveList()).Returns(incidentListResponse);

            //action
            var response = _testObj.GetActiveIncidentList();

            //assert
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(0 == response.Result.List.Count);
        }


        [Fact]
        public void GetIncident_Incident_NotFound()
        {
            var incidentId = Guid.NewGuid().ToString();
            var incidentResponse = Task.FromResult(MockIncidentResponse(false, new List<IncidentDto>()));
            _mockIncidentMicroApi.Setup(x => x.GetDetails(It.IsAny<GetIncidentDetailsRequestDto>())).Returns(incidentResponse);
            var response = _testObj.GetIncidentById(incidentId);
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
            var response = _testObj.GetIncidentById(incidentId.ToString());
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
                IncidentId = Guid.NewGuid()
            };

            _mockIncidentFacadeApi.Setup(api => api.CreateIncident(It.IsAny<CreateIncidentRequestDto>()))
                .Returns(Task.FromResult(createIncidentResponse));

            //act
            var incidentId = _testObj.CreateIncident(request);

            //assert
            Assert.Equal(createIncidentResponse.IncidentId.ToString(), incidentId.Result);
        }


        [Fact]
        public void RespondIncident_Success()
        {
            //arrange
            var incidentId = Guid.NewGuid();
            _mockIncidentMicroApi
                .Setup(api =>
                    api.Respond(It.Is<RespondIncidentRequestDto>(request => request.IncidentId == incidentId)))
                .ReturnsAsync(new RespondIncidentResponseDto {IsSuccess = true});

            //act
            var result = _testObj.RespondIncident(incidentId.ToString());

            //assert
            Assert.Equal(ExecuteStatus.Successful, result.Result.Status);
        }

        [Fact]
        public void RespondIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";
           
            //act
            var result = _testObj.RespondIncident(incidentId);

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
                .ReturnsAsync(new RespondIncidentResponseDto { IsSuccess = false });


            //act
            var result = _testObj.RespondIncident(incidentId.ToString());

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
                .ReturnsAsync(new TakeoverIncidentResponseDto {IsSuccess = true});

            //act
            var result = _testObj.TakeoverIncident(incidentId.ToString());

            //assert
            Assert.Equal(ExecuteStatus.Successful, result.Result.Status);
        }

        [Fact]
        public void TakeoverIncident_InvalidIncidentId_ReturnError()
        {
            //arrange
            var incidentId = "wrong incident id";

            //act
            var result = _testObj.TakeoverIncident(incidentId);

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
                .ReturnsAsync(new TakeoverIncidentResponseDto { IsSuccess = false });

            //act
            var result = _testObj.TakeoverIncident(incidentId.ToString());

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
                .ReturnsAsync(new CloseIncidentResponseDto { IsSuccess = true });

            //act
            var result = _testObj.CloseIncident(incidentId.ToString(), reason);

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
            var result = _testObj.CloseIncident(incidentId, reason);

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
                .ReturnsAsync(new CloseIncidentResponseDto { IsSuccess = false });

            //act
            var result = _testObj.CloseIncident(incidentId.ToString(), reason);

            //assert
            Assert.Equal(ExecuteStatus.Error, result.Result.Status);
        }


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


        private GetIncidentListResponseDto MockIncidentListResponse(bool isSuccess, List<IncidentListItemDto> list)
        {
            var response = new GetIncidentListResponseDto()
            {
                List = list,
                IsSuccess = isSuccess,
                Message = "Any Valid Message"
            };
            return response;
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
    }
}
