﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public void ImportWorkflowDesigns_Test()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ImportWorkflowDesigns(It.IsAny<Stream>()))
                .Returns(MockExecuteResult());
            var result = _testObj.ImportAsync(It.IsAny<Stream>());
            VerifyResult(result);
        }

        [Fact]
        public void ValidatorWorkflowDesigns_Test()
        {
            _mockWorkflowDesignRepository.Setup(x => x.ValidatorWorkflowDesigns(It.IsAny<Stream>()))
                .Returns(MockExecuteResult());
            var result = _testObj.ValidatorAsync(It.IsAny<Stream>());
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public void DeleteWorkflowDesigns_Test()
        {
            _mockWorkflowDesignRepository.Setup(x => x.DeleteWorkflowDesigns(It.IsAny<string[]>()))
                .Returns(MockExecuteResult());
            var result = _testObj.DeletesAsync(It.IsAny<string[]>());
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public void GetAllActiveWorkflowDesigns_Test()
        {
            var mockDesign = new WorkflowDesignSummaryGto
            {
                Id = Guid.NewGuid()
            };
            var allWorkflowDesigns = Task.FromResult(new[] { mockDesign });
            _mockWorkflowDesignRepository.Setup(x => x.GetAllActiveWorkflowDesigns()).Returns(allWorkflowDesigns);
            var result = _testObj.GetSummariesAsync();
            Assert.NotNull(result);
            Assert.True(result.Result.Length == 1);
            Assert.True(result.Result[0].Id == mockDesign.Id);
        }


        [Fact]
        public void GetWorkflowDesignSelectors_Test()
        {
            // arrange
            var mockDesign = new WorkflowDesignSelectorGto
            {
                Id = Guid.NewGuid()
            };
            var mockWorkflowDesignSelectorListGto = new WorkflowDesignSelectorListGto();
            mockWorkflowDesignSelectorListGto.List.Add(mockDesign);
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignSelectors())
                .Returns((Task.FromResult(mockWorkflowDesignSelectorListGto)));

            // action
            var result = _testObj.GetSelectorsAsync();

            // assert
            Assert.NotNull(result);
            Assert.True(result.Result.List.Count == 1);
            Assert.True(result.Result.List[0].Id == mockDesign.Id);
        }

        [Fact]
        public void GetWorkflowDesignById_Test()
        {
            var mockDesign = new WorkflowDesignGto
            {
                Id = Guid.NewGuid()
            };
            var workflowDesigns = Task.FromResult(mockDesign);
            _mockWorkflowDesignRepository.Setup(x => x.GetWorkflowDesignById(It.IsAny<string>()))
                .Returns(workflowDesigns);
            var result = _testObj.GetByIdAsync(It.IsAny<string>());
            Assert.NotNull(result);
            Assert.True(result.Result.Id == mockDesign.Id);
        }

        [Fact]
        public void DownloadWorkflowTemplate_Test()
        {
            var mockTemplate = new WorkflowTemplateGto
            {
                FileName = "TestSopTemplate"
            };
            var mockTemplateTask = Task.FromResult(mockTemplate);
            _mockWorkflowDesignRepository.Setup(x => x.DownloadWorkflowTemplate())
                .Returns(mockTemplateTask);
            var result = _testObj.DownloadTemplateAsync();
            Assert.NotNull(result);
            Assert.True(result.Result.FileName == mockTemplate.FileName);
        }

        [Fact]
        public void ExportWorkflowDesigns_Test()
        {
            var mockTemplate = new WorkflowTemplateGto
            {
                FileName = "TestSopTemplate"
            };
            var mockTemplateTask = Task.FromResult(mockTemplate);
            _mockWorkflowDesignRepository.Setup(x => x.ExportWorkflowDesigns(It.IsAny<string[]>()))
                .Returns(mockTemplateTask);
            var result = _testObj.ExportsAsync(It.IsAny<string[]>());
            Assert.NotNull(result);
            Assert.True(result.Result.FileName == mockTemplate.FileName);
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
            var result = _testObj.GetIdsAsync();

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
            var result = _testObj.GetDetailsAsync(It.IsAny<GetDetailsRequestGto>());

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
            var result = _testObj.GetDetailsAsync(It.IsAny<GetDetailsRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }
        private Task<ExecuteResult> MockExecuteResult()
        {
            return Task.FromResult(new ExecuteResult { Status = ExecuteStatus.Successful });
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void VerifyResult(Task<ExecuteResult> result)
        {
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
        }
    }
}
