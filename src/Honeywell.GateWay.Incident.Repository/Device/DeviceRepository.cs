using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Repository.Data;

namespace Honeywell.GateWay.Incident.Repository.Device
{
    public class DeviceRepository: IDeviceRepository
    {
        private readonly IDeviceApi _deviceApi;
        private const string SystemName = "Prowatch";
        private const string UnderScore = "_";

        public DeviceRepository(IDeviceApi prowatchApi)
        {
            _deviceApi = prowatchApi;
        }

        public async Task<DevicesEntity> GetDevices()
        {
            var devices = await _deviceApi.GetDevices();
            foreach (var config in devices.Config)
            {
                config.Type = $"{SystemName}{UnderScore}{config.Type}";
            }
            return devices;
        }

        public async Task<DevicesEntity> GetDeviceById(string deviceId)
        {
            var device = await _deviceApi.GetDeviceById(deviceId);
            return device;
        }
    }
}
