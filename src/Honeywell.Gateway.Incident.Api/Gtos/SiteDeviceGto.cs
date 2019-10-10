namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class SiteDeviceGto
    {
        public string SiteDisplayName { get; set; }

        public string SiteId { get; set; }

        public DeviceGto[] Devices { get; set; }
    }
}
