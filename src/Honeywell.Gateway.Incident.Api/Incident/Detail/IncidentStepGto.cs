using System;
using System.Collections.Generic;

namespace Honeywell.Gateway.Incident.Api.Incident.Detail
{
    public class IncidentStepGto
    {
        public Guid Id { get; set; }
        public bool IsHandled { get; set; }
        public bool IsOptional { get; set; }
        public string Instruction { get; set; }
        public string HelpText { get; set; }

        public IList<StepCommentGto> StepComments { get; set; } = new List<StepCommentGto>();
    }
}
