﻿using System.Threading.Tasks;

namespace Honeywell.GateWay.Incident.Repository.Device
{
    public class DeviceRepository: IDeviceRepository
    {
        private readonly IDeviceApi _deviceApi;

        public DeviceRepository(IDeviceApi prowatchApi)
        {
            _deviceApi = prowatchApi;
        }

        public async Task<DevicesEntity> GetDevices()
        {
            var devices = await _deviceApi.GetDevices();
            foreach (var config in devices.Config)
            {
                config.Type = DeviceTypeHelper.GetSystemDeviceType(config.Type);
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
