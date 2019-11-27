using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Gateway.Incident.Api;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Infra.Api.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Incident.ApiTests.IncidentControllerTest
{
    public class BaseIncIdentControllerTest : TestOfBase
    {
        protected ServiceProvider ServiceProvider { get; }
        protected IIncidentApi IncidentGateWayApi { get; }
        protected IWorkflowDesignApi WorkflowDesignGateWayApi { get; }
        public BaseIncIdentControllerTest(DIFixture dIFixture)
        {
            ServiceProvider = dIFixture.ServiceProvider;
            IncidentGateWayApi = ServiceProvider.GetService<IIncidentApi>();
            WorkflowDesignGateWayApi = ServiceProvider.GetService<IWorkflowDesignApi>();
        }

        protected string GetFirstWorkflowDesignId()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Result);
            var workflowDesign = workflowDesigns.Result.FirstOrDefault();
            Assert.NotNull(workflowDesign);
            return workflowDesign.Id.ToString();
        }

        protected async Task<string> GetFirstWorkflowDesignReferenceId()
        {
            var workflowDesigns = await WorkflowDesignGateWayApi.GetSelectorsAsync();
            Assert.NotNull(workflowDesigns);
            Assert.NotNull(workflowDesigns.Value);
            var workflowDesign = workflowDesigns.Value.FirstOrDefault();
            Assert.NotNull(workflowDesign);
            return workflowDesign.ReferenceId.ToString();
        }

        protected async Task<WorkflowDesignSummaryGto[]> GetAllWorkflowDesigns()
        {
            var workflowDesigns = await WorkflowDesignGateWayApi.GetListAsync(string.Empty);
            return workflowDesigns.Value;
        }

        protected async Task<ApiResponse> ImportWorkflowDesign()
        {
            var resourceName = "Incident.ApiTests.Data.TestTemplate.docx";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var result = await WorkflowDesignGateWayApi.ImportAsync(stream);
            return result;
        }

        protected async Task DeleteWorkflowDesign()
        {
            var workflowDesigns = GetAllWorkflowDesigns();
            await WorkflowDesignGateWayApi.DeletesAsync(workflowDesigns.Result.Select(m => m.Id.ToString()).ToArray());
        }

        protected async Task DeleteIncident(string incidentId)
        {
            var respondResult = await IncidentGateWayApi.RespondAsync(incidentId);
            Assert.True(respondResult.IsSuccess);

            var closeResult = await IncidentGateWayApi.CloseAsync(incidentId, "test delete");
            Assert.True(closeResult.IsSuccess);
        }

        protected async Task<string> CreateIncident(string deviceId = null, string deviceType = null)
        {
            var workflowDesignReferenceId = await GetFirstWorkflowDesignReferenceId();
            var incident = new CreateIncidentRequestGto
            {
                Description = "incident 1", Priority = "Low", WorkflowDesignReferenceId = workflowDesignReferenceId,
                DeviceId = deviceId, DeviceType = deviceType
            };
            var result = await IncidentGateWayApi.CreateAsync(incident);
            Assert.True(result.IsSuccess);
            return result.Value;
        }

        protected async Task<ApiResponse<Guid[]>> CreateIncidentByAlarm(string alarmId = null)
        {
            alarmId = string.IsNullOrEmpty(alarmId) ? Guid.NewGuid().ToString() : alarmId;
            var workflowDesignReferenceId = await GetFirstWorkflowDesignReferenceId();
            var request = new CreateIncidentByAlarmRequestGto
            {
                WorkflowDesignReferenceId = new Guid(workflowDesignReferenceId),
                Priority = IncidentPriority.High,
                Description = "incident description",
                DeviceId = Guid.NewGuid().ToString(),
                DeviceType = "Door",
                AlarmId = alarmId,
                AlarmData = new AlarmData
                {
                    AlarmType = "AlarmType",
                    Description = "alarm description"
                }
            };

            var result = await IncidentGateWayApi.CreateByAlarmAsync(new[] {request});
            Assert.NotNull(result);
            return result;
        }
    }
}
