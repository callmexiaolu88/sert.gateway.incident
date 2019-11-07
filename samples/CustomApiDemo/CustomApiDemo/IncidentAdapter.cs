using System;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Infra.Api.Abstract;
using Newtonsoft.Json;

namespace CustomApiDemo
{
    public class IncidentAdapter
    {
        private readonly HttpHelper _httpHelper;

        public IncidentAdapter(HttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public void CreateIncident()
        {
            try
            {
                var request = new[]
                {
                    new CreateIncidentByAlarmRequestGto
                    {
                        //you can get the id from GetWorkflowDesignIds method
                        WorkflowDesignReferenceId = new Guid(),
                        Priority = IncidentPriority.High,
                        Description = "Demo description",
                        //the device id is that associates with alarm
                        DeviceId = new Guid().ToString(),
                        DeviceType = "Door",
                        //the trigger id is alarm id
                        AlarmId = new Guid().ToString(),
                        AlarmData = new AlarmData
                        {
                            AlarmType = "Void badge",
                            Description = "alarm description"

                        }
                    }
                };

                var designs = _httpHelper.PostAsync<CreateIncidentByAlarmRequestGto[],
                    ApiResponse<Guid[]>>("api/Incident/CreateByAlarm", request);
                Console.WriteLine(JsonConvert.SerializeObject(designs.Result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void GetIncidentStatusWithAlarmId()
        {
            try
            {
                var request = new[]
                {
                    new Guid().ToString(),
                };

                var workflowDesigns =
                    _httpHelper.PostAsync<string[], ApiResponse<IncidentStatusInfoGto[]>>(
                        "api/Incident/GetStatusByAlarm", request);
                Console.WriteLine(JsonConvert.SerializeObject(workflowDesigns.Result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void GetWorkflowDesigns()
        {
            try
            {
                var request = new[]
                {
                    new Guid(),
                };

                var workflowDesigns =
                    _httpHelper.PostAsync<Guid[], ApiResponse<WorkflowDesignDetailGto[]>>(
                        "api/WorkflowDesign/GetDetails", request);
                Console.WriteLine(JsonConvert.SerializeObject(workflowDesigns.Result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //you can get all workflow design reference id
        public void GetWorkflowDesignIds()
        {
            try
            {
                var summaries =
                    _httpHelper.PostAsync<ApiResponse<WorkflowDesignIdGto[]>>("api/WorkflowDesign/GetIds");

                Console.WriteLine(JsonConvert.SerializeObject(summaries.Result));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
