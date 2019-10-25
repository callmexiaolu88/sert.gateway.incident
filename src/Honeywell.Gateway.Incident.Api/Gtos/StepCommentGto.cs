using System;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class StepCommentGto
    {
        public DateTime AtUtc { get; set; }
        public string Operator { get; set; }
        public string Description { get; set; }
    }
}
