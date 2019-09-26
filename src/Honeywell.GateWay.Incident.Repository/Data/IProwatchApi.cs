using System.Threading.Tasks;
using Honeywell.Infra.Client.WebApi.Attributes.HttpMethodAttributes;
using Honeywell.Infra.Client.WebApi.Attributes.RouteAttribute;
using Honeywell.Infra.Core;

namespace Honeywell.GateWay.Incident.Repository.Data
{
    public interface IProwatchApi: IRemoteService
    {
        [HttpGet]
        [Route("/PWWebAPI/ISOM/DeviceMgmt/Devices/config")]
        Task<ProwatchDevicesEntity> GetDevices();
    }
}
