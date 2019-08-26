using System;
using System.Collections.Generic;
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
        public void WorkflowDesignAppService_GetAllActiveWorkflowDesigns_Success()
        {
            // arrange
            var summaryResponseDto = new WorkflowDesignSummaryResponseDto
            {
                IsSuccess = true,
                Summaries = new List<WorkflowDesignSummaryDto>
                {
                    new WorkflowDesignSummaryDto
                    {
                        Id=Guid.NewGuid(),
                        Name = "workflow design 1",
                        Description = "workflow design 1 description"
                    }
                }
            };

            _workflowDesignApiMock.Setup(x => x.GetSummaries()).Returns(Task.FromResult(summaryResponseDto));

            // action
            var result = _workflowDesignGatewayApi.GetAllActiveWorkflowDesigns();

            // assert
            Assert.True(1 == result.Result.Length);
        }
    }
}
