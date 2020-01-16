using System;
using System.Collections.Generic;
using System.Linq;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Incident.AddStepComment;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Gateway.Incident.Api.Incident.UpdateStepStatus;
using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.Infra.Api.Abstract;
using Honeywell.Infra.Services.Isom.Api;
using Honeywell.Infra.Services.Isom.Api.Custom;
using Honeywell.Infra.Services.Isom.Api.Custom.Camera.GetCamera;
using Honeywell.Security.Isom.Common;
using Moq;
using Proxy.Honeywell.Security.ISOM.Devices;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class IncidentAppServiceTest : ApplicationServiceTestBase
    {
        private readonly IIncidentAppService _testObj;
        private readonly Mock<IIncidentRepository> _mockIncidentRepository;
        private readonly Mock<IDeviceFacadeApi> _mockDeviceFacadeApi;
        private readonly Mock<ICameraFacadeApi> _mockCameraFacadeApi;


        public IncidentAppServiceTest()
        {
            _mockIncidentRepository = new Mock<IIncidentRepository>();
            _mockDeviceFacadeApi = new Mock<IDeviceFacadeApi>();
            _mockCameraFacadeApi = new Mock<ICameraFacadeApi>();
            _testObj = new IncidentAppService(
                _mockIncidentRepository.Object,
                _mockCameraFacadeApi.Object,
                _mockDeviceFacadeApi.Object);
        }

        [Fact]
        public void UpdateWorkflowStepStatus_Normal_Succeed()
        {
            //arrange

            _mockIncidentRepository.Setup(x => x.UpdateWorkflowStepStatus(It.IsAny<UpdateStepStatusRequestGto>()));

            //act
            var result = _testObj.UpdateStepStatusAsync(It.IsAny<UpdateStepStatusRequestGto>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void UpdateWorkflowStepStatus_ThrowException_Failed()
        {
            //arrange

            _mockIncidentRepository.Setup(x => x.UpdateWorkflowStepStatus(It.IsAny<UpdateStepStatusRequestGto>())).ThrowsAsync(new Exception());

            //act
            var result = _testObj.UpdateStepStatusAsync(It.IsAny<UpdateStepStatusRequestGto>());

            //assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
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
        public void GetIncidentById_ValidDevice_AlarmTrigger_Succeed()
        {
            //arrange
            var mockDeviceResult = MockDeviceEntities();
            var device = mockDeviceResult.config[0];
            var alarmId = "432543353454235";
            var mockIncident = new IncidentDetailGto
            {
                Description = "Test Incident Description",
                DeviceId = device.identifiers.id,
                DeviceLocation = device.identifiers.tag[0],
                DeviceDisplayName = device.identifiers.name,
                TriggerType = IncidentTriggerType.Alarm,
                TriggerId = alarmId,
                AlarmData = new AlarmData { AlarmUtcDateTime = DateTime.UtcNow, AlarmType = "fasdfasd" }
            };

            var cameraInfo = new GetCameraInfo() { CameraNum = "1", CameraId = "1", CameraName = "1" };

            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .ReturnsAsync(mockIncident);

            _mockDeviceFacadeApi.Setup(x => x.GetDeviceDetails(It.IsAny<string>(), It.IsAny<DataFilters>()))
                .Returns(mockDeviceResult.config[0]);

            _mockCameraFacadeApi.Setup(x => x.GetCameraByAlarmId(It.IsAny<string>())).Returns(cameraInfo);

            //act
            var result = _testObj.GetDetailAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.Value.CameraNumber == cameraInfo.CameraId);
            Assert.True(result.Result.Value.EventTimeStamp == new DateTimeOffset(mockIncident.AlarmData.AlarmUtcDateTime).ToUnixTimeMilliseconds());
            Assert.True(result.Result.Value.Description == mockIncident.Description);
            Assert.True(result.Result.Value.DeviceDisplayName == mockDeviceResult.config[0].identifiers.name);
            Assert.True(result.Result.Value.DeviceLocation == mockDeviceResult.config[0].identifiers.tag[0]);
        }

        [Fact]
        public void GetIncidentById_ValidDevice_GetCameraFailed()
        {
            //arrange
            var mockDeviceResult = MockDeviceEntities();
            var device = mockDeviceResult.config[0];
            var alarmId = "432543353454235";
            var createDate = DateTime.Now;
            var eventTimeStamp = new DateTimeOffset(createDate).ToUnixTimeMilliseconds();
            var mockIncident = new IncidentDetailGto
            {
                Description = "Test Incident Description",
                DeviceId = device.identifiers.id,
                DeviceLocation = device.identifiers.tag[0],
                DeviceDisplayName = device.identifiers.name,
                TriggerType = IncidentTriggerType.Manual,
                TriggerId = alarmId,
                CreateAtUtc = createDate
            };

            var cameraInfo = ApiResponse.CreateFailed().To<GetCameraInfo>();

            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .ReturnsAsync(mockIncident);

            _mockDeviceFacadeApi.Setup(x => x.GetDeviceDetails(It.IsAny<string>(), It.IsAny<DataFilters>()))
                .Returns(mockDeviceResult.config[0]);

            _mockCameraFacadeApi.Setup(x => x.GetCameraByLogicDeviceId(It.IsAny<string>())).Returns(cameraInfo);

            //act
            var result = _testObj.GetDetailAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.Value.EventTimeStamp == eventTimeStamp);
            Assert.True(result.Result.Value.Description == mockIncident.Description);
            Assert.True(result.Result.Value.DeviceDisplayName == mockDeviceResult.config[0].identifiers.name);
            Assert.True(result.Result.Value.DeviceLocation == mockDeviceResult.config[0].identifiers.tag[0]);
            Assert.Null(result.Result.Value.CameraNumber);
        }


        [Fact]
        public void GetIncidentById_ValidDevice_ManualTrigger_Succeed()
        {
            //arrange
            var mockDeviceResult = MockDeviceEntities();
            var device = mockDeviceResult.config[0];
            var alarmId = "432543353454235";
            var createDate = DateTime.Now;
            var eventTimeStamp = new DateTimeOffset(createDate).ToUnixTimeMilliseconds();
            var mockIncident = new IncidentDetailGto
            {
                Description = "Test Incident Description",
                DeviceId = device.identifiers.id,
                DeviceLocation = device.identifiers.tag[0],
                DeviceDisplayName = device.identifiers.name,
                TriggerType = IncidentTriggerType.Manual,
                TriggerId = alarmId,
                CreateAtUtc = createDate
            };

            var cameraInfo = new GetCameraInfo { CameraNum = "1", CameraId = "1", CameraName = "1" };

            _mockIncidentRepository.Setup(x => x.GetIncidentById(It.IsAny<string>()))
                .ReturnsAsync(mockIncident);

            _mockDeviceFacadeApi.Setup(x => x.GetDeviceDetails(It.IsAny<string>(), It.IsAny<DataFilters>()))
                .Returns(mockDeviceResult.config[0]);

            _mockCameraFacadeApi.Setup(x => x.GetCameraByLogicDeviceId(It.IsAny<string>())).Returns(cameraInfo);

            //act
            var result = _testObj.GetDetailAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.Value.CameraNumber == cameraInfo.CameraId);
            Assert.True(result.Result.Value.EventTimeStamp == eventTimeStamp);
            Assert.True(result.Result.Value.Description == mockIncident.Description);
            Assert.True(result.Result.Value.DeviceDisplayName == mockDeviceResult.config[0].identifiers.name);
            Assert.True(result.Result.Value.DeviceLocation == mockDeviceResult.config[0].identifiers.tag[0]);
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
        public void CreateIncident_ThrowException_Failed()
        {

            var id = Guid.NewGuid().ToString();
            _mockIncidentRepository.Setup(x => x.CreateIncident(It.IsAny<CreateIncidentRequestGto>()))
                .ThrowsAsync(new Exception());

            var result = _testObj.CreateAsync(It.IsAny<CreateIncidentRequestGto>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void RespondIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.RespondIncident(It.IsAny<string>()));
            var result = _testObj.RespondAsync(It.IsAny<string>());
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void RespondIncident_ThrowException_Failed()
        {
            _mockIncidentRepository.Setup(x => x.RespondIncident(It.IsAny<string>())).ThrowsAsync(new Exception());

            var result = _testObj.RespondAsync(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }
        [Fact]
        public void TakeoverIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.TakeoverIncident(It.IsAny<string>()));
            var result = _testObj.TakeoverAsync(It.IsAny<string>());
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void TakeoverIncident_ThrowException_Failed()
        {
            _mockIncidentRepository.Setup(x => x.TakeoverIncident(It.IsAny<string>())).ThrowsAsync(new Exception());

            var result = _testObj.TakeoverAsync(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void CloseIncident_Test()
        {
            _mockIncidentRepository.Setup(x => x.CloseIncident(It.IsAny<string>(), It.IsAny<string>()));
            var result = _testObj.CloseAsync(It.IsAny<string>(), It.IsAny<string>());
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void CloseIncident_ThrowException_Failed()
        {
            _mockIncidentRepository.Setup(x => x.CloseIncident(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            var result = _testObj.CloseAsync(It.IsAny<string>(), It.IsAny<string>());

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetDevices_Test()
        {
            var mockDevice = MockDeviceEntities();
            _mockDeviceFacadeApi.Setup(x => x.GetDeviceList(It.IsAny<DataFilters>())).Returns(mockDevice);
            var result = _testObj.GetSiteDevicesAsync();
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Length == 1);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceDisplayName, mockDevice.config[0].identifiers.name);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceId, mockDevice.config[0].identifiers.id);
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceType, DeviceTypeHelper.GetSystemDeviceType(mockDevice.config[0].type.ToString()));
            Assert.Equal(result.Result.Value[0].Devices[0].DeviceLocation, mockDevice.config[0].identifiers.tag[0]);
            Assert.Equal(result.Result.Value[0].SiteId, mockDevice.config[0].relation[0].id);
            Assert.Equal(result.Result.Value[0].SiteDisplayName, mockDevice.config[0].relation[0].entityId);
        }

        [Fact]
        public void GetDevices_ThrowException_Failed()
        {
            var mockDevice = MockDeviceEntities();
            _mockDeviceFacadeApi.Setup(x => x.GetDeviceList(It.IsAny<DataFilters>())).Throws(new Exception());

            var result = _testObj.GetSiteDevicesAsync();

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }


        [Fact]
        public void CompleteIncident_Normal_Succeed()
        {
            //arrange

            _mockIncidentRepository.Setup(x => x.CompleteIncident(It.IsAny<string>()));

            //act
            var result = _testObj.CompleteAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void CompleteIncident_ThrowException_Failed()
        {
            //arrange
            _mockIncidentRepository.Setup(x => x.CompleteIncident(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            //act
            var result = _testObj.CompleteAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetActiveIncidentList_Test()
        {

            var mockActiveIncidentGto = new IncidentSummaryGto
            {
                WorkflowId = Guid.NewGuid(),
                WorkflowDesignName = "test"
            };
            var mockRequest = MockGetListRequestGto();
            _mockIncidentRepository.Setup(x => x.GetList(mockRequest))
                .ReturnsAsync(new[] { mockActiveIncidentGto });
            var result = _testObj.GetListAsync(mockRequest);
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Length == 1);
            Assert.True(result.Result.Value[0].WorkflowId == mockActiveIncidentGto.WorkflowId);
            Assert.True(result.Result.Value[0].WorkflowDesignName == mockActiveIncidentGto.WorkflowDesignName);
        }

        [Fact]
        public void GetActiveIncidentList_ThrowException_Failed()
        {
            var mockRequest = MockGetListRequestGto();
            _mockIncidentRepository.Setup(x => x.GetList(mockRequest)).ThrowsAsync(new Exception());

            var result = _testObj.GetListAsync(mockRequest);

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);

        }

        [Fact]
        public void GetActivitysAsync_Normal_Succeed()
        {
            //arrange

            _mockIncidentRepository.Setup(x => x.GetActivitys(It.IsAny<string>()));

            //act
            var result = _testObj.GetActivitysAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void GetActivitysAsync_ThrowException_Failed()
        {
            //arrange
            _mockIncidentRepository.Setup(x => x.GetActivitys(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            //act
            var result = _testObj.GetActivitysAsync(It.IsAny<string>());

            //assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void AddStepComment_Successful()
        {
            var addStepComment = new AddStepCommentRequestGto()
            { WorkflowStepId = It.IsAny<string>(), Comment = It.IsAny<string>() };

            _mockIncidentRepository.Setup(x => x.AddStepComment(addStepComment));

            var result = _testObj.AddStepCommentAsync(addStepComment);
            Assert.True(result.Result.IsSuccess);
        }

        [Fact]
        public void AddStepComment_ThrowException_Failed()
        {
            var addStepComment = new AddStepCommentRequestGto()
            { WorkflowStepId = It.IsAny<string>(), Comment = It.IsAny<string>() };

            _mockIncidentRepository.Setup(x => x.AddStepComment(addStepComment)).ThrowsAsync(new Exception());

            var result = _testObj.AddStepCommentAsync(addStepComment);

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        private DeviceConfigList MockDeviceEntities()
        {
            var deviceDisplayName = "Door 1";
            var deviceId = "ProWatch Device Id";
            var deviceEntity = new DeviceConfig
            {
                identifiers = new DeviceIdentifiers
                {
                    description = "ProWatch Device",
                    id = deviceId,
                    name = deviceDisplayName,
                    tag = new[] { "location1" }.ToList()
                },
                relation = new[] { new DeviceRelation { entityId = "Geili Site", id = "Generaic Device" } }.ToList(),
                type = DeviceType.Door
            };
            return new DeviceConfigList { config = new List<DeviceConfig> { deviceEntity } };
        }

        [Fact]
        public void CreateByAlarm_Successful()
        {
            //Arrange
            var id = Guid.NewGuid();
            _mockIncidentRepository.Setup(x => x.CreateIncidentByAlarm(It.IsAny<CreateIncidentByAlarmRequestGto[]>()))
                .ReturnsAsync(new[] { id });

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


        [Fact]
        public void GetStatistics_Successful()
        {
            //Arrange
            var deviceId = Guid.NewGuid().ToString();
            var incidentStatisticsGto = new IncidentStatisticsGto
            {
                DeviceId = deviceId,
                UnAssignedCount = 1,
                ActiveCount = 1,
                CloseCount = 1,
                CompletedCount = 1
            };
            _mockIncidentRepository.Setup(x => x.GetStatistics(deviceId))
                .ReturnsAsync(incidentStatisticsGto);

            //Act
            var result = _testObj.GetStatisticsAsync(deviceId);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Result.IsSuccess);
            Assert.Equal(result.Result.Value.UnAssignedCount, incidentStatisticsGto.UnAssignedCount);
            Assert.Equal(result.Result.Value.ActiveCount, incidentStatisticsGto.ActiveCount);
            Assert.Equal(result.Result.Value.DeviceId, incidentStatisticsGto.DeviceId);
            Assert.Equal(result.Result.Value.CloseCount, incidentStatisticsGto.CloseCount);
            Assert.Equal(result.Result.Value.CompletedCount, incidentStatisticsGto.CompletedCount);
        }

        [Fact]
        public void GetStatistics_ThrowException()
        {
            //Arrange
            var deviceId = Guid.NewGuid().ToString();
            _mockIncidentRepository.Setup(x => x.GetStatistics(deviceId))
                .Throws(new Exception());

            //Act
            var result = _testObj.GetStatisticsAsync(deviceId);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }

        [Fact]
        public void GetListByDevice_Successful()
        {
            var mockActiveIncidentGto = new IncidentSummaryGto
            {
                WorkflowId = Guid.NewGuid(),
                WorkflowDesignName = "test"
            };
            var mockRequest = MockGetListRequestGto();
            _mockIncidentRepository.Setup(x => x.GetList(mockRequest))
                .ReturnsAsync(new[] { mockActiveIncidentGto });
            var result = _testObj.GetListAsync(mockRequest);
            Assert.NotNull(result);
            Assert.True(result.Result.Value.Length == 1);
            Assert.True(result.Result.Value[0].WorkflowId == mockActiveIncidentGto.WorkflowId);
            Assert.True(result.Result.Value[0].WorkflowDesignName == mockActiveIncidentGto.WorkflowDesignName);
        }

        private PageRequest<GetListRequestGto> MockGetListRequestGto()
        {
            var request = new GetListRequestGto
            {
                State = 1,
                DeviceId = Guid.NewGuid().ToString()
            };
            return new PageRequest().To(request);
        }

        [Fact]
        public void GetListByDevice_ThrowException()
        {

            var mockRequest = MockGetListRequestGto();
            _mockIncidentRepository.Setup(x => x.GetList(mockRequest)).ThrowsAsync(new Exception());

            var result = _testObj.GetListAsync(mockRequest);

            Assert.NotNull(result);
            Assert.False(result.Result.IsSuccess);
        }
    }
}