using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.GateWay.Incident.Repository.Data;
using Honeywell.GateWay.Incident.Repository.Device;
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

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
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
        public void GetWorkflowDesignSelectors_Test()
        {
            // arrange
            var mockDesign = new WorkflowDesignSelectorGto
            {
                Id = Guid.NewGuid()
            };
            var mockWorkflowDesignSelectorListGto = new WorkflowDesignSelectorListGto();
            mockWorkflowDesignSelectorListGto.List.Add(mockDesign);
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignSelectors())
                .Returns((Task.FromResult(mockWorkflowDesignSelectorListGto)));

            // action
            var result = _testObj.GetWorkflowDesignSelectors();

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

        [Fact]
        public void CreateIncidentByAlarm_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockResponse = Task.FromResult(new CreateIncidentResponseGto
            {
                IncidentIds = new List<Guid>() {id}
            });
            _mockIncidentRepository.Setup(x => x.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto>()))
                .Returns(mockResponse);

            //Act
            var result = _testObj.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.IncidentIds.Any());
            Assert.True(result.Result.Value.IncidentIds.First() == id);
        }

        [Fact]
        public void CreateIncidentByAlarm_ThrowException()
        {
            //Arrange
            _mockIncidentRepository.Setup(x => x.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.False(result.Result.IsSuccess);
            Assert.False(result.Result.Value.IncidentIds.Any());
        }

        [Fact]
        public void GetWorkflowDesignIds_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            var name = "name";

            var mockResponse = Task.FromResult(new GetWorkflowDesignIdentifiersResponseGto
            {
                Identifiers = new List<WorkflowDesignIdentifierGto>
                {
                    new WorkflowDesignIdentifierGto
                    {
                        Name = name,
                        WorkflowDesignReferenceId = id,
                    }
                }
            });
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesignIds())
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetWorkflowDesignIds();

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
            var result = _testObj.GetWorkflowDesignIds();

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
            var mockResponse = Task.FromResult(new GetWorkflowDesignsResponseGto
            {
                WorkflowDesigns = new List<WorkflowDesignGto>
                {
                    new WorkflowDesignGto
                    {
                        Id = id
                    }
                }
            });
            _mockIncidentRepository.Setup(x => x.GetWorkflowDesigns(It.IsAny<GetWorkflowDesignsRequestGto>()))
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetWorkflowDesigns(It.IsAny<GetWorkflowDesignsRequestGto>());

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
            var result = _testObj.GetWorkflowDesigns(It.IsAny<GetWorkflowDesignsRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.False(result.Result.IsSuccess);
            Assert.False(result.Result.Value.WorkflowDesigns.Any());
        }

        [Fact]
        public void GetIncidentStatusWithAlarmId_Successful()
        {
            //Arrange
            var incidentId = Guid.NewGuid();
            var alarmId = Guid.NewGuid().ToString();
            var mockResponse = Task.FromResult(new GetIncidentStatusResponseGto
            {
                IncidentStatusInfos = new List<IncidentStatusInfoGto>
                {
                    new IncidentStatusInfoGto
                    {
                        IncidentId = incidentId,
                        AlarmId = alarmId,
                        Status = IncidentStatus.Active
                    }
                }
            });
            _mockIncidentRepository.Setup(x => x.GetIncidentStatusWithAlarmId(It.IsAny<GetIncidentStatusRequestGto>()))
                .Returns(mockResponse);

            //Act
            var result = _testObj.GetIncidentStatusWithAlarmId(It.IsAny<GetIncidentStatusRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.IncidentStatusInfos.Any());
            Assert.True(result.Result.Value.IncidentStatusInfos.First().IncidentId == incidentId);
            Assert.True(result.Result.Value.IncidentStatusInfos.First().AlarmId == alarmId);
            Assert.True(result.Result.Value.IncidentStatusInfos.First().Status == IncidentStatus.Active);
        }

        [Fact]
        public void GetIncidentStatusWithAlarmId_ThrowException()
        {
            //Arrange
            _mockIncidentRepository.Setup(x => x.GetIncidentStatusWithAlarmId(It.IsAny<GetIncidentStatusRequestGto>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetIncidentStatusWithAlarmId(It.IsAny<GetIncidentStatusRequestGto>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.False(result.Result.IsSuccess);
            Assert.False(result.Result.Value.IncidentStatusInfos.Any());
        }
    }
}