using System.Threading.Tasks;

namespace Honeywell.GateWay.Incident.Repository.Device
{
    public interface IDeviceRepository
    {
        Task<DevicesEntity> GetDevices();

        Task<DevicesEntity> GetDeviceById(string deviceId);
    }
}
