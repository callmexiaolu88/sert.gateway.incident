using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Repository.Device;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class DeviceRepositoryUnitTest : ApplicationServiceTestBase
    {
        private readonly Mock<IDeviceApi> _mockDeviceApi;

        private readonly DeviceRepository _deviceRepository;

        public DeviceRepositoryUnitTest()
        {
            _mockDeviceApi = new Mock<IDeviceApi>();

            _deviceRepository = new DeviceRepository(
                _mockDeviceApi.Object);
        }

        [Fact]
        public async Task DeviceRepository_GetDevices_Success()
        {
            // arrange
            var mockResult = MockDevicesEntity();
            _mockDeviceApi.Setup(x => x.GetDevices())
                .ReturnsAsync(mockResult);

            // action
            var result = await _deviceRepository.GetDevices();

            // assert
            Assert.NotNull(result);
            Assert.Equal(result.Config.Length, mockResult.Config.Length);
            Assert.Equal(result.Config[0].Identifiers.Id, mockResult.Config[0].Identifiers.Id);
            Assert.Equal(result.Config[0].Identifiers.Description, mockResult.Config[0].Identifiers.Description);
            Assert.Equal(result.Config[0].Identifiers.Name, mockResult.Config[0].Identifiers.Name);
            Assert.Equal(result.Config[0].Identifiers.Tag.Length, mockResult.Config[0].Identifiers.Tag.Length);
            Assert.Equal(result.Config[0].Identifiers.Tag[0], mockResult.Config[0].Identifiers.Tag[0]);
            Assert.Equal(result.Config[0].Relation.Length, mockResult.Config[0].Relation.Length);
            Assert.Equal(result.Config[0].Relation[0].Id, mockResult.Config[0].Relation[0].Id);
            Assert.Equal(result.Config[0].Relation[0].EntityId, mockResult.Config[0].Relation[0].EntityId);
            Assert.Equal(result.Config[0].Type, mockResult.Config[0].Type);
        }

        [Fact]
        public async Task DeviceRepository_GetDeviceById_Success()
        {
            // arrange
            var mockResult = MockDevicesEntity();
            _mockDeviceApi.Setup(x => x.GetDeviceById(It.IsAny<string>()))
                .ReturnsAsync(mockResult);

            // action
            var result = await _deviceRepository.GetDeviceById(It.IsAny<string>());

            // assert
            Assert.NotNull(result);
            Assert.Equal(result.Config.Length, mockResult.Config.Length);
            Assert.Equal(result.Config[0].Identifiers.Id, mockResult.Config[0].Identifiers.Id);
            Assert.Equal(result.Config[0].Identifiers.Description, mockResult.Config[0].Identifiers.Description);
            Assert.Equal(result.Config[0].Identifiers.Name, mockResult.Config[0].Identifiers.Name);
            Assert.Equal(result.Config[0].Identifiers.Tag.Length, mockResult.Config[0].Identifiers.Tag.Length);
            Assert.Equal(result.Config[0].Identifiers.Tag[0], mockResult.Config[0].Identifiers.Tag[0]);
            Assert.Equal(result.Config[0].Relation.Length, mockResult.Config[0].Relation.Length);
            Assert.Equal(result.Config[0].Relation[0].Id, mockResult.Config[0].Relation[0].Id);
            Assert.Equal(result.Config[0].Relation[0].EntityId, mockResult.Config[0].Relation[0].EntityId);
            Assert.Equal(result.Config[0].Type, mockResult.Config[0].Type);
        }

        private DevicesEntity MockDevicesEntity()
        {
            var mockResult = new DevicesEntity
            {
                Config = new[]
                {
                    new DeviceEntity
                    {
                        Identifiers = new IdentifiersEntity
                        {
                            Id="1",
                            Description = "device 1",
                            Name = "device display name 1",
                            Tag = new []{"location 1"}
                        },
                        Relation = new []
                        {
                            new RelationEntity
                            {
                                Id = "site id 1",
                                EntityId = "site display name 1",
                                Name = ""
                            }
                        },
                        Type = "door",
                        SubType = ""
                    }
                }
            };
            return mockResult;
        }
    }
}
