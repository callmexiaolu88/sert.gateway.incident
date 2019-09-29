using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class ExecuteResult
    {
        public ExecuteStatus Status { get; set; }

        public List<string> ErrorList { get; set; }

        public static ExecuteResult Success = new ExecuteResult { Status = ExecuteStatus.Successful };

        public static ExecuteResult Error = new ExecuteResult {Status = ExecuteStatus.Error};

        public ExecuteResult()
        {
            ErrorList = new List<string>();
        }
    }


    public enum ExecuteStatus
    {
        Successful = 0,
        Warning = 1,
        Error = 2
    }
}
