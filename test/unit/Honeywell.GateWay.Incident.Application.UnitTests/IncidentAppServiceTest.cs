using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
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
            var incidentRequest = new GetIncidentDetailsRequestDto {Ids = new[] {Guid.NewGuid()}};
            var workflowRequest = new WorkflowDetailsRequestDto {Ids = new[] {Guid.NewGuid()}};
            var incidentResponse = MockIncidentResponse(true,MockIncidentDtos());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(incidentRequest)).Returns(Task.FromResult(incidentResponse));
            var workflowResponse = MockWorkflowResponse(true,MocksWorkflowDtos());
            _mockWorkflowInstanceApi.Setup(x => x.GetWorkflowDetails(workflowRequest))
                .Returns(Task.FromResult(workflowResponse));
            var response = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(response);
        }


        [Fact]
        public void GetIncident_IncidentNotFound1()
        {
            var incidentRequest = new GetIncidentDetailsRequestDto { Ids = new[] { Guid.NewGuid() } };
            var incidentResponse = MockIncidentResponse(true, MockIncidentDtos());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(incidentRequest)).Returns(Task.FromResult(incidentResponse));
            var response = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(response);
        }

        [Fact]
        public void GetIncident_Incident_ErrorNotFound()
        {
            var incidentRequest = new GetIncidentDetailsRequestDto { Ids = new[] { Guid.NewGuid() } };
            var incidentResponse = MockIncidentResponse(true,new List<IncidentDto>());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(incidentRequest)).Returns(Task.FromResult(incidentResponse));
            var response = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(response);
        }

        [Fact]
        public void GetIncident_Workflow_NotFound1()
        {
            var incidentRequest = new GetIncidentDetailsRequestDto { Ids = new[] { Guid.NewGuid() } };
            var workflowRequest = new WorkflowDetailsRequestDto { Ids = new[] { Guid.NewGuid() } };
            var incidentResponse = MockIncidentResponse(true,MockIncidentDtos());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(incidentRequest)).Returns(Task.FromResult(incidentResponse));
            var workflowResponse = MockWorkflowResponse(false,MocksWorkflowDtos());
            _mockWorkflowInstanceApi.Setup(x => x.GetWorkflowDetails(workflowRequest))
                .Returns(Task.FromResult(workflowResponse));
            var response = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(response);
        }

        [Fact]
        public void GetIncident_Workflow_Error_NotFound1()
        {
            var incidentRequest = new GetIncidentDetailsRequestDto { Ids = new[] { Guid.NewGuid() } };
            var workflowRequest = new WorkflowDetailsRequestDto { Ids = new[] { Guid.NewGuid() } };
            var incidentResponse = MockIncidentResponse(true, MockIncidentDtos());
            _mockIncidentMicroApi.Setup(x => x.GetDetails(incidentRequest)).Returns(Task.FromResult(incidentResponse));
            var workflowResponse = MockWorkflowResponse(true,new List<WorkflowDto>());
            _mockWorkflowInstanceApi.Setup(x => x.GetWorkflowDetails(workflowRequest))
                .Returns(Task.FromResult(workflowResponse));
            var response = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(response);
        }


        private List<WorkflowDto> MocksWorkflowDtos()
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
                }
            };
            return workflows;
        }

        private List<IncidentDto> MockIncidentDtos()
        {
            var incidents = new List<IncidentDto>()
            {
                new IncidentDto
                {
                    Id = Guid.NewGuid(),
                    Description = "Any Incident Desc",
                    Number = 1,
                    CreateAtUtc = DateTime.Now,
                    LastUpdateAtUtc = DateTime.Now,
                    Owner = "Admin1",
                    //Priority = byte(IncidentPriority.Low),
                    WorkflowId = Guid.NewGuid()
                }
            };
            return incidents;
        }


        private WorkflowDetailsResponseDto MockWorkflowResponse(bool isSuccess, List<WorkflowDto> details)
        {
            var response = new WorkflowDetailsResponseDto()
            {
                Details = details,
                IsSuccess = isSuccess
            };
            return response;
        }


        private GetIncidentDetailsResponseDto MockIncidentResponse(bool isSuccess, List<IncidentDto> details)
        {
            var response = new GetIncidentDetailsResponseDto()
            {
                Details = details,
                IsSuccess = isSuccess
            };
            return response;
        }
    }
}
