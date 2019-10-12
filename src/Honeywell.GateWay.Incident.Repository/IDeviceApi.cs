using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Repository.Data;
using Honeywell.Infra.Client.WebApi.Attributes.HttpMethodAttributes;
using Honeywell.Infra.Client.WebApi.Attributes.RouteAttribute;
using Honeywell.Infra.Core;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IDeviceApi: IRemoteService
    {
        [HttpGet]
        [Route("/PWWebAPI/ISOM/DeviceMgmt/Devices/config")]
        Task<DevicesEntity> GetDevices();

        [HttpGet]
        [Route("/PWWebAPI/ISOM/DeviceMgmt/Devices/config?q=(identifier.id=list({deviceId}))")]
        Task<DevicesEntity> GetDeviceById(string deviceId);
    }
}
