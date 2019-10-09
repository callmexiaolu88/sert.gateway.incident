using System;
using System.Collections.Generic;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class ActiveIncidentListGto : ExecuteResult
    {
        public IList<ActiveIncidentGto> List { get; set; }

        public ActiveIncidentListGto()
        {
            List = new List<ActiveIncidentGto>();
        }
    }
}
