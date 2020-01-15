using Xunit;

namespace Honeywell.GateWay.Incident.Repository.UnitTests
{
    public class DeviceTypeHelperTest
    {
        [Fact]
        public void DeviceTypeHelperTest_Successful()
        {
            var type = "Door";
            var formatType = DeviceTypeHelper.GetSystemDeviceType(type);
            Assert.NotNull(formatType);
            Assert.Equal("Prowatch_Door", formatType);
        }
    }
}
