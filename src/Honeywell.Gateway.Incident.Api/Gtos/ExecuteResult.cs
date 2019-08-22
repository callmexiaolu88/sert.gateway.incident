using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class ExecuteResult
    {
        public ExecuteStatus Status { get; set; }

        public List<string> ErrorList { get; set; }
    }

    public enum ExecuteStatus
    {
        Successful = 0,
        Warning = 1,
        Error = 2
    }
}
