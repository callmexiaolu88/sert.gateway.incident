namespace Honeywell.Gateway.Incident.Api.Incident.GetList
{
    public class GetListRequestGto
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int State { get; set; }

        public string DeviceId { get; set; }
    }
}
