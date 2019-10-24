using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.Gateway.Incident.Api.Workflow.List;
using Honeywell.GateWay.Incident.Application.Workflow;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class WorkflowAppServiceTest : ApplicationServiceTestBase
    {
        private readonly IWorkflowAppService _testObj;
        private readonly Mock<IIncidentRepository> _mockIncidentRepository;

        public WorkflowAppServiceTest()
        {
            _mockIncidentRepository = new Mock<IIncidentRepository>();
            _testObj = new WorkflowAppService(_mockIncidentRepository.Object);
        }

        [Fact]
        public void GetWorkflowDesignIds_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var name = "name";

            var mockResponse = Task.FromResult(ApiResponse.CreateSuccess().To(new GetWorkflowDesignIdentifiersResponseGto
            {
                Identifiers = new List<WorkflowDesignIdGto>
                {
                    new WorkflowDesignIdGto
                    {
                        Name = name,
                        WorkflowDesignReferenceId = id,
                    }
                }
            }));
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignIds())
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetDesignIds();

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Identifiers.Any());
            Assert.True(result.Result.Value.Identifiers.First().WorkflowDesignReferenceId == id);
            Assert.True(result.Result.Value.Identifiers.First().Name == name);
        }

        [Fact]
        public void GetWorkflowDesignIds_ThrowException()
        {
            //Arrange
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignIds())
                .Throws(new Exception());

            //Act
            var result = _testObj.GetDesignIds();

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.False(result.Result.IsSuccess);
            Assert.False(result.Result.Value.Identifiers.Any());
        }

        [Fact]
        public void GetWorkflowDesigns_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockResponse = Task.FromResult(ApiResponse.CreateSuccess().To(new GetWorkflowDesignsResponseGto
            {
                WorkflowDesigns = new List<WorkflowDesignGto>
                {
                    new WorkflowDesignGto
                    {
                        Id = id
                    }
                }
            }));
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesigns(It.IsAny<GetWorkflowDesignsRequestGto>()))
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetDesignDetails(It.IsAny<GetWorkflowDesignsRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.WorkflowDesigns.Any());
            Assert.True(result.Result.Value.WorkflowDesigns.First().Id == id);
        }

        [Fact]
        public void GetWorkflowDesigns_ThrowException()
        {
            //Arrange
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesigns(It.IsAny<GetWorkflowDesignsRequestGto>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetDesignDetails(It.IsAny<GetWorkflowDesignsRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.False(result.Result.IsSuccess);
            Assert.False(result.Result.Value.WorkflowDesigns.Any());
        }

    }
}
