using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Honeywell.Facade.Services.Incident.Api;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.GateWay.Incident.Repository.Data;
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Summary;
using Honeywell.Micro.Services.Workflow.Domain.Shared;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class IncidentAppServiceTest : ApplicationServiceTestBase
    {
        private readonly IIncidentAppService _testObj;
        private readonly Mock<IIncidentRepository> _mockIncidentRepository;
        private readonly Mock<IDeviceRepository> _mockDeviceRepository;

        public IncidentAppServiceTest()
        {
            _mockDeviceRepository = new Mock<IDeviceRepository>();
            _mockIncidentRepository = new Mock<IIncidentRepository>();
            _testObj = new IncidentAppService(_mockIncidentRepository.Object,
                _mockDeviceRepository.Object);
        }

        private Task<ExecuteResult> MockExecuteResult()
        {
            return Task.FromResult(new ExecuteResult { Status = ExecuteStatus.Successful });
        }

        private void VerifyResult(Task<ExecuteResult> result)
        {
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public void ImportWorkflowDesigns_Test()
        {
            _mockIncidentRepository.Setup(x => x.ImportWorkflowDesigns(It.IsAny<Stream>()))
                .Returns(MockExecuteResult());
            var result = _testObj.ImportWorkflowDesigns(It.IsAny<Stream>());
            VerifyResult(result);
        }

        [Fact]
        public void ValidatorWorkflowDesigns_Test()
        {
            _mockIncidentRepository.Setup(x => x.ValidatorWorkflowDesigns(It.IsAny<Stream>()))
                .Returns(MockExecuteResult());
            var result = _testObj.ValidatorWorkflowDesigns(It.IsAny<Stream>());
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
        }

        [Fact]
        public void DeleteWorkflowDesigns_Test()
        {
            _mockIncidentRepository.Setup(x => x.DeleteWorkflowDesigns(It.IsAny<string[]>()))
                .Returns(MockExecuteResult());
            var result = _testObj.DeleteWorkflowDesigns(It.IsAny<string[]>());
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
            var allworkflowDesigns = Task.FromResult(new[] { mockDesign });
            _mockIncidentRepository.Setup(x => x.GetAllActiveWorkflowDesigns()).Returns(allworkflowDesigns);
            var result = _testObj.GetAllActiveWorkflowDesigns();
            Assert.NotNull(result);
            Assert.True(result.Result.Length == 1);
            Assert.True(result.Result[0].Id == mockDesign.Id);
        }


        [Fact]
        public void GetWorkflowDesignSelectorsByName_Test()
        {
            var mockDesign = new WorkflowDesignSelectorGto
            {
                Id = Guid.NewGuid()
            };
            var workflowDesigns = Task.FromResult(new[] { mockDesign });
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignSelectorsByName(It.IsAny<string>()))
                .Returns(workflowDesigns);
            var result = _testObj.GetWorkflowDesignSelectorsByName(It.IsAny<string>());
            Assert.NotNull(result);
            Assert.True(result.Result.Length == 1);
            Assert.True(result.Result[0].Id == mockDesign.Id);
        }

        [Fact]
        public void GetWorkflowDesignById_Test()
        {
            var mockDesign = new WorkflowDesignGto
            {
                Id = Guid.NewGuid()
            };
            var workflowDesigns = Task.FromResult(mockDesign);
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignById(It.IsAny<string>()))
                .Returns(workflowDesigns);
            var result = _testObj.GetWorkflowDesignById(It.IsAny<string>());
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
            _mockIncidentRepository.Setup(x => x.DownloadWorkflowTemplate())
                .Returns(mockTemplateTask);
            var result = _testObj.DownloadWorkflowTemplate();
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
            _mockIncidentRepository.Setup(x => x.ExportWorkflowDesigns(It.IsAny<string[]>()))
                .Returns(mockTemplateTask);
            var result = _testObj.ExportWorkflowDesigns(It.IsAny<string[]>());
            Assert.NotNull(result);
            Assert.True(result.Result.FileName == mockTemplate.FileName);
        }

        [Fact]
        public void GetIncidentById_EmptyDevice_Succeed()
        {
            var mockIncident = new IncidentGto
            {
                Description = "Test Incident Description",
                Status = ExecuteStatus.Successful
            };
            var mockIncidentTask = Task.FromResult(mockIncident);
            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .Returns(mockIncidentTask);
            var result = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Successful);
            Assert.True(result.Result.Description == mockIncident.Description);
        }

        [Fact]
        public void GetIncidentById_EmptyDevice_Failed()
        {
            var mockIncident = new IncidentGto
            {
                Status = ExecuteStatus.Error
            };
            var mockIncidentTask = Task.FromResult(mockIncident);
            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .Returns(mockIncidentTask);
            var result = _testObj.GetIncidentById(It.IsAny<string>());
            Assert.NotNull(result);
            Assert.True(result.Result.Status == ExecuteStatus.Error);
        }

        [Fact]
        public void GetIncidentById_ValidDevice_Succeed()
        {
            var mockDeviceResult = MockDeviceEntities();
            var device = mockDeviceResult.Config[0];
            var mockIncident = new IncidentGto
            {
                Description = "Test Incident Description",
                Status = ExecuteStatus.Successful,
                DeviceId = device.Identifiers.Id,
                DeviceLocation = device.Identifiers.Tag[0],
                DeviceDisplayName = device.Identifiers.Name
            };
            var mockIncidentTask = Task.FromResult(mockIncident);
            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .Returns(mockIncidentTask);
            _mockDeviceRepository.Setup(x => x.GetDeviceById(It.IsAny<string>())).Returns(Task.FromResult(mockDeviceResult));

            var result = _testObj.GetIncidentById(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.True(result.Result.Description == mockIncident.Description);
            Assert.True(result.Result.DeviceDisplayName == mockDeviceResult.Config[0].Identifiers.Name);
            Assert.True(result.Result.DeviceLocation == mockDeviceResult.Config[0].Identifiers.Tag[0]);
        }
        
        [Fact]
        public void CreateIncident_Test()
        {

            var id = Guid.NewGuid().ToString();
            var mockResponse = Task.FromResult(id);
            _mockIncidentRepository.Setup(x => x.CreateIncident(It.IsAny<CreateIncidentRequestGto>()))
                .Returns(mockResponse);
            var result = _testObj.CreateIncident(It.IsAny<CreateIncidentRequestGto>());
            Assert.NotNull(result);
            Assert.True(result.Result == id);
        }

        [Fact]
        public void RespondIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.RespondIncident(It.IsAny<string>()))
                .Returns(MockExecuteResult());
            var result = _testObj.RespondIncident(It.IsAny<string>());
            VerifyResult(result);
        }
        [Fact]
        public void TakeoverIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.TakeoverIncident(It.IsAny<string>()))
                .Returns(MockExecuteResult());
            var result = _testObj.TakeoverIncident(It.IsAny<string>());
            VerifyResult(result);
        }

        [Fact]
        public void CloseIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.CloseIncident(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(MockExecuteResult());
            var result = _testObj.CloseIncident(It.IsAny<string>(), It.IsAny<string>());
            VerifyResult(result);
        }

        [Fact]
        public void GetDevices_Test()
        {
            var mockDevice = MockDeviceEntities();
            _mockDeviceRepository.Setup(x => x.GetDevices()).Returns(Task.FromResult(mockDevice));
            var result = _testObj.GetSiteDevices();
            Assert.NotNull(result);
            Assert.True(result.Result.Length == 1);
            Assert.Equal(result.Result[0].Devices[0].DeviceDisplayName, mockDevice.Config[0].Identifiers.Name);
            Assert.Equal(result.Result[0].Devices[0].DeviceId, mockDevice.Config[0].Identifiers.Id);
            Assert.Equal(result.Result[0].Devices[0].DeviceType, mockDevice.Config[0].Type);
            Assert.Equal(result.Result[0].Devices[0].DeviceLocation, mockDevice.Config[0].Identifiers.Tag[0]);
            Assert.Equal(result.Result[0].SiteId, mockDevice.Config[0].Relation[0].Id);
            Assert.Equal(result.Result[0].SiteDisplayName, mockDevice.Config[0].Relation[0].EntityId);
        }

        [Fact]
        public void GetActiveIncidentList_Test()
        {

            var mockActiveIncidentGto = new ActiveIncidentGto
            {
                WorkflowId = Guid.NewGuid(),
                WorkflowDesignName = "test"
            };
            var mockActiveIncidentListGto = new ActiveIncidentListGto();
            mockActiveIncidentListGto.List.Add(mockActiveIncidentGto);

            _mockIncidentRepository.Setup(x => x.GetActiveIncidentList())
                .Returns(Task.FromResult(mockActiveIncidentListGto));
            var result = _testObj.GetActiveIncidentList();
            Assert.NotNull(result);
            Assert.True(result.Result.List.Count == 1);
            Assert.True(result.Result.List[0].WorkflowId == mockActiveIncidentGto.WorkflowId);
            Assert.True(result.Result.List[0].WorkflowDesignName == mockActiveIncidentGto.WorkflowDesignName);
        }

        private DevicesEntity MockDeviceEntities()
        {
            var deviceDisplayName = "Door 1";
            var deviceId = "ProWatch Device Id";
            var deviceType = "Door";
            var deviceEntity = new DeviceEntity
            {
                Identifiers = new IdentifiersEntity
                {
                    Description = "ProWatch Device",
                    Id = deviceId,
                    Name = deviceDisplayName,
                    Tag = new[] { "location1" }
                },
                Relation = new[] { new RelationEntity { EntityId = "Geili Site", Id = "Generaic Device" } },
                Type = deviceType
            };
            return new DevicesEntity { Config = new[] { deviceEntity } };
        }
    }
}