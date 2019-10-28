using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Workflow.Detail;
using Honeywell.Gateway.Incident.Api.Workflow.List;
using Honeywell.GateWay.Incident.Application.Workflow;
using Honeywell.GateWay.Incident.Repository;
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

            var mockResponse = Task.FromResult(new GetWorkflowDesignIdsResponseGto
            {
                WorkflowDesignIds = new List<WorkflowDesignIdGto>
                {
                    new WorkflowDesignIdGto
                    {
                        Name = name,
                        WorkflowDesignReferenceId = id,
                    }
                }
            });
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignIds())
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetDesignIds();

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.WorkflowDesignIds.Any());
            Assert.True(result.Result.Value.WorkflowDesignIds.First().WorkflowDesignReferenceId == id);
            Assert.True(result.Result.Value.WorkflowDesignIds.First().Name == name);
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
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetWorkflowDesigns_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockResponse = Task.FromResult(new GetWorkflowDesignDetailsResponseGto
            {
                WorkflowDesigns = new List<WorkflowDesignGto>
                {
                    new WorkflowDesignGto
                    {
                        Id = id
                    }
                }
            });
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignDetails(It.IsAny<GetWorkflowDesignDetailsRequestGto>()))
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetDesignDetails(It.IsAny<GetWorkflowDesignDetailsRequestGto>());

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
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignDetails(It.IsAny<GetWorkflowDesignDetailsRequestGto>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetDesignDetails(It.IsAny<GetWorkflowDesignDetailsRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

    }
}
