using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.GateWay.Incident.Repository.Device;
using Honeywell.Infra.Api.Abstract;
using Microsoft.Extensions.Localization.Internal;
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
        
        [Fact]
        public void GetIncidentById_EmptyDevice_Succeed()
        {
            //arrange
            var mockIncident = new IncidentDetailGto
            {
                Description = "Test Incident Description",
            };

            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .ReturnsAsync(mockIncident);

            //act
            var result = _testObj.GetDetailAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Description == mockIncident.Description);
        }

        [Fact]
        public void GetIncidentById_EmptyDevice_Failed()
        {
            //arrange
            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            //act
            var result = _testObj.GetDetailAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetIncidentById_ValidDevice_Succeed()
        {
            //arrange
            var mockDeviceResult = MockDeviceEntities();
            var device = mockDeviceResult.Config[0];
            var mockIncident = new IncidentDetailGto
            {
                Description = "Test Incident Description",
                DeviceId = device.Identifiers.Id,
                DeviceLocation = device.Identifiers.Tag[0],
                DeviceDisplayName = device.Identifiers.Name
            };

            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .ReturnsAsync(mockIncident);

            _mockDeviceRepository.Setup(x => x.GetDeviceById(It.IsAny<string>())).Returns(Task.FromResult(mockDeviceResult));

            //act
            var result = _testObj.GetDetailAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Description == mockIncident.Description);
            Assert.True(result.Result.Value.DeviceDisplayName == mockDeviceResult.Config[0].Identifiers.Name);
            Assert.True(result.Result.Value.DeviceLocation == mockDeviceResult.Config[0].Identifiers.Tag[0]);
        }
        
        [Fact]
        public void CreateIncident_Test()
        {

            var id = Guid.NewGuid().ToString();
            _mockIncidentRepository.Setup(x => x.CreateIncident(It.IsAny<CreateIncidentRequestGto>()))
                .ReturnsAsync(id);
            var result = _testObj.CreateAsync(It.IsAny<CreateIncidentRequestGto>());
            Assert.NotNull(result);
            Assert.True(result.Result.Value == id);
        }

        [Fact]
        public void RespondIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.RespondIncident(It.IsAny<string>()));
            var result = _testObj.RespondAsync(It.IsAny<string>());
            Assert.True(result.Result.IsSuccess);
        }
        [Fact]
        public void TakeoverIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.TakeoverIncident(It.IsAny<string>()));
            var result = _testObj.TakeoverAsync(It.IsAny<string>());
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void CloseIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.CloseIncident(It.IsAny<string>(), It.IsAny<string>()));
            var result = _testObj.CloseAsync(It.IsAny<string>(), It.IsAny<string>());
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void GetDevices_Test()
        {
            var mockDevice = MockDeviceEntities();
            _mockDeviceRepository.Setup(x => x.GetDevices()).ReturnsAsync(mockDevice);
            var result = _testObj.GetSiteDevicesAsync();
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Length == 1);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceDisplayName, mockDevice.Config[0].Identifiers.Name);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceId, mockDevice.Config[0].Identifiers.Id);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceType, mockDevice.Config[0].Type);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceLocation, mockDevice.Config[0].Identifiers.Tag[0]);
            Assert.Equal(result.Result.Value[0].SiteId, mockDevice.Config[0].Relation[0].Id);
            Assert.Equal(result.Result.Value[0].SiteDisplayName, mockDevice.Config[0].Relation[0].EntityId);
        }

        [Fact]
        public void GetActiveIncidentList_Test()
        {

            var mockActiveIncidentGto = new IncidentSummaryGto
            {
                WorkflowId = Guid.NewGuid(),
                WorkflowDesignName = "test"
            };

            _mockIncidentRepository.Setup(x => x.GetActiveIncidentList())
                .ReturnsAsync(new[] {mockActiveIncidentGto});
            var result = _testObj.GetListAsync();
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Length == 1);
            Assert.True(result.Result.Value[0].WorkflowId == mockActiveIncidentGto.WorkflowId);
            Assert.True(result.Result.Value[0].WorkflowDesignName == mockActiveIncidentGto.WorkflowDesignName);
        }

        [Fact]
        public void AddStepComment_Successful()
        {
            var addStepComment = new AddStepCommentRequestGto()
                {WorkflowStepId = It.IsAny<string>(), Comment = It.IsAny<string>()};

            _mockIncidentRepository.Setup(x => x.AddStepComment(addStepComment));

            var result = _testObj.AddStepCommentAsync(addStepComment);
            Assert.True(result.Result.IsSuccess);
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
        public void CreateByAlarm_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            _mockIncidentRepository.Setup(x => x.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto[]>()))
                .ReturnsAsync(new []{id});

            //Act
            var result = _testObj.CreateByAlarmAsync(It.IsAny<CreateIncidentByAlarmRequestGto[]>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Any());
            Assert.True(result.Result.Value.First() == id);
        }

        [Fact]
        public void CreateByAlarm_ThrowException()
        {
            //Arrange
            _mockIncidentRepository.Setup(x => x.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto[]>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.CreateByAlarmAsync(It.IsAny<CreateIncidentByAlarmRequestGto[]>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetIncidentStatusWithAlarmId_Successful()
        {
            //Arrange
            var incidentId = Guid.NewGuid();
            var alarmId = Guid.NewGuid().ToString();
            var mockResponse = new[]
            {
                new IncidentStatusInfoGto
                {
                    IncidentId = incidentId,
                    AlarmId = alarmId,
                    Status = IncidentStatus.Active
                }
            };

            _mockIncidentRepository.Setup(x => x.GetIncidentStatusByAlarm(It.IsAny<string[]>()))
                .ReturnsAsync(mockResponse);

            //Act
            var result = _testObj.GetStatusByAlarmAsync(It.IsAny<string[]>());

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result.Value);
            Assert.True(result.Result.IsSuccess);
            Assert.True(result.Result.Value.Any());
            Assert.True(result.Result.Value.First().IncidentId == incidentId);
            Assert.True(result.Result.Value.First().AlarmId == alarmId);
            Assert.True(result.Result.Value.First().Status == IncidentStatus.Active);
        }

        [Fact]
        public void GetIncidentStatusWithAlarmId_ThrowException()
        {
            //Arrange
            _mockIncidentRepository.Setup(x => x.GetIncidentStatusByAlarm(It.IsAny<string[]>()))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetStatusByAlarmAsync(It.IsAny<string[]>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }
    }
}