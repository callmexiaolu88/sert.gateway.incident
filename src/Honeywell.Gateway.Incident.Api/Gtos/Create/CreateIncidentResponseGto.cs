using System;
using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Gtos.Create
{
    public class CreateIncidentResponseGto
    {
        public IList<Guid> IncidentIds { get; set; }

        public CreateIncidentResponseGto()
        {
            IncidentIds = new List<Guid>();
        }
    }
}
