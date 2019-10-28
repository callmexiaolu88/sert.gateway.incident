using System;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class ActivityGto
    {
        public DateTime CreateAtUtc { get; set; }

        public string Operator { get; set; }

        public string Description { get; set; }

        public string DescriptionLocalized { get; set; }
    }
}
