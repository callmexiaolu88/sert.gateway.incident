using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Domain.Shared;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class IncidentAppServiceTest : ApplicationServiceTestBase
    {
        private readonly Mock<IIncidentMicroApi> _mockIncidentMicroApi;
        private readonly Mock<IWorkflowInstanceApi> _mockWorkflowInstanceApi;

        private readonly IIncidentAppService _testObj;

        public IncidentAppServiceTest()
        {
            var mockWorkflowDesignApi = new Mock<IWorkflowDesignApi>();
            _mockIncidentMicroApi = new Mock<IIncidentMicroApi>();
            _mockWorkflowInstanceApi = new Mock<IWorkflowInstanceApi>();
            _testObj = new IncidentAppService(mockWorkflowDesignApi.Object, _mockIncidentMicroApi.Object, _mockWorkflowInstanceApi.Object);
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
    }
}
