using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.GateWay.Incident.Repository;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class WorkflowAppServiceTest : ApplicationServiceTestBase
    {
        private readonly IWorkflowDesignAppService _testObj;
        private readonly Mock<IWorkflowDesignRepository> _mockWorkflowDesignRepository;

        public WorkflowAppServiceTest()
        {
            _mockWorkflowDesignRepository = new Mock<IWorkflowDesignRepository>();
            _testObj = new WorkflowDesignAppService(_mockWorkflowDesignRepository.Object);
        }

        [Fact]
        public void GetWorkflowDesignIds_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var name = "name";

            var mockResponse = Task.FromResult(new GetIdsResponseGto
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
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignIds())
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetIds();

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
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignIds())
                .Throws(new Exception());

            //Act
            var result = _testObj.GetIds();

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetWorkflowDesigns_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockResponse = Task.FromResult(new GetDetailsResponseGto
            {
                WorkflowDesigns = new List<WorkflowDesignGto>
                {
                    new WorkflowDesignGto
                    {
                        Id = id
                    }
                }
            });
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignDetails(It.IsAny<GetDetailsRequestGto>()))
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetDetails(It.IsAny<GetDetailsRequestGto>());

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
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignDetails(It.IsAny<GetDetailsRequestGto>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetDetails(It.IsAny<GetDetailsRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

    }
}
