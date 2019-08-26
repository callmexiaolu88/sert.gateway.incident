using System.IO;
using System.Text;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.GateWay.Incident.Application.WorkflowDesign;
using Honeywell.Micro.Services.Workflow.Api;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using Moq;
using Xunit;

namespace Honeywell.GateWay.Incident.Application.UnitTests
{
    public class WorkflowDesignAppServiceUnitTest
    {
        private readonly Mock<IWorkflowDesignApi> _workflowDesignApiMock;

        private readonly IWorkflowDesignGatewayApi _workflowDesignGatewayApi;

        public WorkflowDesignAppServiceUnitTest()
        {
            _workflowDesignApiMock = new Mock<IWorkflowDesignApi>();

            _workflowDesignGatewayApi = new WorkflowDesignAppService(_workflowDesignApiMock.Object);
        }


        [Fact]
        public void Test1()
        {
            // arrange
            WorkflowDesignSummaryResponseDto summaryResponseDto = new WorkflowDesignSummaryResponseDto();

            _workflowDesignApiMock.Setup(x => x.GetSummaries()).Returns(Task.FromResult(summaryResponseDto));

            // action
            var result = _workflowDesignGatewayApi.GetAllActiveWorkflowDesign();

            // assert
            Assert.True(1 == result.Result.Length);
        }


        [Fact]
        public void ImportWorkFlowDesign_Successful()
        {
            // arrange
            WorkflowDesignSummaryResponseDto summaryResponseDto = new WorkflowDesignSummaryResponseDto();

            _workflowDesignApiMock.Setup(x => x.Imports(It.IsAny<Stream>())).Returns();

            // action

            using (var workflowStream = new MemoryStream(Encoding.UTF8.GetBytes("whatever")))
            {
                //var result = _workflowDesignGatewayApi.ImportWorkflowDesigns(workflowStream);

                //// Assert    
                //Assert.True(result);
            }
            

            // assert
       
        }

    }
}
