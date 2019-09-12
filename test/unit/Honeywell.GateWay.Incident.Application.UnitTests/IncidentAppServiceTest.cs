using Honeywell.GateWay.Incident.Application.Incident;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Workflow.Api;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class IncidentAppServiceTest : ApplicationServiceTestBase
    {
        private readonly Mock<IWorkflowDesignApi> _mockWorkflowDesignApi;
        private readonly Mock<IIncidentMicroApi> _mockIncidentMicroApi;
        private readonly Mock<IWorkflowInstanceApi> _mockWorkflowInstanceApi;

        private readonly IIncidentAppService testObj;

        public IncidentAppServiceTest()
        {
            _mockWorkflowDesignApi = new Mock<IWorkflowDesignApi>();
            _mockIncidentMicroApi = new Mock<IIncidentMicroApi>();
            _mockWorkflowInstanceApi = new Mock<IWorkflowInstanceApi>();
            testObj = new IncidentAppService(_mockWorkflowDesignApi.Object, _mockIncidentMicroApi.Object, _mockWorkflowInstanceApi.Object);
        }

        [Fact]
        public void GetIncident_Successful()
        {

        }


    }
}
