using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Repository.Data;

namespace Honeywell.GateWay.Incident.Repository.Device
{
    public interface IDeviceRepository
    {
        Task<DevicesEntity> GetDevices();
    }
}
