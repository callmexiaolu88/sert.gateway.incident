namespace Honeywell.Gateway.Incident.Api.Incident.GetSiteDevice
{
    public class SiteDeviceGto
    {
        public string SiteDisplayName { get; set; }

        public string SiteId { get; set; }

        public DeviceGto[] Devices { get; set; }
    }
}
