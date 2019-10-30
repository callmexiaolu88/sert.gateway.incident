using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Incident.List
{
    public class GetListResponseGto
    {
        public IList<IncidentGto> List { get; set; }

        public GetListResponseGto()
        {
            List = new List<IncidentGto>();
        }
    }
}
