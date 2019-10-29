using System.Collections.Generic;
using Honeywell.Gateway.Incident.Api.Gtos;

namespace Honeywell.Gateway.Incident.Api.Incident.List
{
    public class GetListResponseGto : ExecuteResult
    {
        public IList<IncidentGto> List { get; set; }

        public GetListResponseGto()
        {
            List = new List<IncidentGto>();
        }
    }
}
