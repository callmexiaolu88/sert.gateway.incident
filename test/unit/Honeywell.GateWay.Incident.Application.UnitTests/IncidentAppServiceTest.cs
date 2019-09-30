using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.GateWay.Incident.Repository;
using Honeywell.GateWay.Incident.Repository.Device;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class IncidentAppServiceTest : ApplicationServiceTestBase
    {
        private IIncidentAppService _testObj;

        public IncidentAppServiceTest()
        {
            var mockDeviceRepository = new Mock<IDeviceRepository>();
            var mockIncidentRepository = new Mock<IIncidentRepository>();
            _testObj = new IncidentAppService(mockIncidentRepository.Object,
                mockDeviceRepository.Object);
        }
    }
}
