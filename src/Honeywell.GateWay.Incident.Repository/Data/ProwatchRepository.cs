using System.Threading.Tasks;

namespace Honeywell.GateWay.Incident.Repository.Data
{
    public class ProwatchRepository: IProwatchRepository
    {
        private readonly IProwatchApi _prowatchApi;

        public ProwatchRepository(IProwatchApi prowatchApi)
        {
            _prowatchApi = prowatchApi;
        }

        public async Task<ProwatchDevicesEntity> GetDevices()
        {
            var devices = await _prowatchApi.GetDevices();
            return devices;
        }
    }
}
