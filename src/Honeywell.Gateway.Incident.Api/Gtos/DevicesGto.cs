namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class SiteDeviceGto
    {
        public string SiteDisplayName { get; set; }

        public string SiteId { get; set; }

        public DeviceGto[] Devices { get; set; }
    }

    public class DeviceGto
    {
        public string DeviceId { get; set; }

        public string DeviceDisplayName { get; set; }

        public string DeviceType { get; set; }
    }
}
