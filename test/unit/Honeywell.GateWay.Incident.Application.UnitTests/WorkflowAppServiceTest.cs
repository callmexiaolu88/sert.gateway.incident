using System;
using System.IO;
using System.Linq;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSummary;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
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
        public void ImportWorkflowDesigns_Test()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ImportWorkflowDesigns(It.IsAny<Stream>()));
            var result = _testObj.ImportAsync(It.IsAny<Stream>());
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void ImportWorkflowDesigns_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ImportWorkflowDesigns(It.IsAny<Stream>()))
                .ThrowsAsync(new Exception());

            var result = _testObj.ImportAsync(It.IsAny<Stream>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void ValidatorWorkflowDesigns_Test()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ValidatorWorkflowDesigns(It.IsAny<Stream>()))
                .ReturnsAsync(ApiResponse.CreateSuccess());
            var result = _testObj.ValidateAsync(It.IsAny<Stream>());
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
        }
        [Fact]
        public void ValidatorWorkflowDesigns_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ValidatorWorkflowDesigns(It.IsAny<Stream>()))
                .ThrowsAsync(new Exception());

            var result = _testObj.ValidateAsync(It.IsAny<Stream>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void DeleteWorkflowDesigns_Test()
        {
            _mockWorkflowDesignRepository.Setup(x => x.DeleteWorkflowDesigns(It.IsAny<string[]>()));
            var result = _testObj.DeletesAsync(It.IsAny<string[]>());
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void DeleteWorkflowDesigns_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.DeleteWorkflowDesigns(It.IsAny<string[]>())).ThrowsAsync(new Exception());

            var result = _testObj.DeletesAsync(It.IsAny<string[]>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetAllActiveWorkflowDesigns_Test()
        {
            var mockDesign = new WorkflowDesignSummaryGto
            {
                Id = Guid.NewGuid()
            };
            _mockWorkflowDesignRepository.Setup(x => x.GetAllActiveWorkflowDesigns(string.Empty)).ReturnsAsync(new[] { mockDesign });
            var result = _testObj.GetSummariesAsync(string.Empty);
            Assert.True(result.Result.IsSuccess);
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Length == 1);
            Assert.True(result.Result.Value[0].Id == mockDesign.Id);
        }

        [Fact]
        public void GetAllActiveWorkflowDesigns_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.GetAllActiveWorkflowDesigns(string.Empty)).ThrowsAsync(new Exception());

            var result = _testObj.GetSummariesAsync(string.Empty);

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }


        [Fact]
        public void GetWorkflowDesignSelectors_Test()
        {
            // arrange
            var mockDesign = new WorkflowDesignSelectorGto
            {
                Id = Guid.NewGuid()
            };

            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignSelectors())
                .ReturnsAsync(new[] {mockDesign});

            // action
            var result = _testObj.GetSelectorsAsync();

            // assert
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Length == 1);
            Assert.True(result.Result.Value[0].Id == mockDesign.Id);
        }

        [Fact]
        public void GetWorkflowDesignSelectors_ThrowException_Failed()
        {
            // arrange

            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignSelectors())
                .ThrowsAsync(new Exception());

            // action
            var result = _testObj.GetSelectorsAsync();

            // assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetWorkflowDesignById_Test()
        {
            var mockDesign = new WorkflowDesignDetailGto
            {
                Id = Guid.NewGuid()
            };
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignById(It.IsAny<string>()))
                .ReturnsAsync(mockDesign);
            var result = _testObj.GetDetailByIdAsync(It.IsAny<string>());
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Id == mockDesign.Id);
        }

        [Fact]
        public void GetWorkflowDesignById_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignById(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = _testObj.GetDetailByIdAsync(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);

        }

        [Fact]
        public void DownloadWorkflowTemplate_Test()
        {
            var mockTemplate = new WorkflowTemplateGto
            {
                FileName = "TestSopTemplate"
            };
            _mockWorkflowDesignRepository.Setup(x => x.DownloadWorkflowTemplate())
                .ReturnsAsync(mockTemplate);
            var result = _testObj.DownloadTemplateAsync();
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.FileName == mockTemplate.FileName);
        }

        [Fact]
        public void DownloadWorkflowTemplate_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.DownloadWorkflowTemplate())
                .ThrowsAsync(new Exception());

            var result = _testObj.DownloadTemplateAsync();

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);

        }

        [Fact]
        public void ExportWorkflowDesigns_Test()
        {
            var mockTemplate = new WorkflowTemplateGto
            {
                FileName = "TestSopTemplate"
            };
            _mockWorkflowDesignRepository.Setup(x => x.ExportWorkflowDesigns(It.IsAny<string[]>()))
                .ReturnsAsync(mockTemplate);
            var result = _testObj.ExportsAsync(It.IsAny<string[]>());
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.FileName == mockTemplate.FileName);
        }

        [Fact]
        public void ExportWorkflowDesigns_ThrowException_Failed()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ExportWorkflowDesigns(It.IsAny<string[]>()))
                .ThrowsAsync(new Exception());

            var result = _testObj.ExportsAsync(It.IsAny<string[]>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetWorkflowDesignIds_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var name = "name";

            var mockResponse = new WorkflowDesignIdGto
            {
                Name = name,
                WorkflowDesignReferenceId = id,
            };

            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignIds())
                .ReturnsAsync(new[] {mockResponse});

            //Act
            var result = _testObj.GetIdsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Any());
            Assert.True(result.Result.Value.First().WorkflowDesignReferenceId == id);
            Assert.True(result.Result.Value.First().Name == name);
        }

        [Fact]
        public void GetWorkflowDesignIds_ThrowException()
        {
            //Arrange
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignIds())
                .Throws(new Exception());

            //Act
            var result = _testObj.GetIdsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetWorkflowDesigns_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockResponse = new WorkflowDesignDetailGto
            {
                Id = id
            };

            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignDetails(It.IsAny<Guid[]>()))
                .ReturnsAsync(new[] {mockResponse});

            //Act
            var result = _testObj.GetDetailsAsync(It.IsAny<Guid[]>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Any());
            Assert.True(result.Result.Value.First().Id == id);
        }

        [Fact]
        public void GetWorkflowDesigns_ThrowException()
        {
            //Arrange
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignDetails(It.IsAny<Guid[]>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetDetailsAsync(It.IsAny<Guid[]>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }
    }
}
