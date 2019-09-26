using System.Threading.Tasks;
using Honeywell.GateWay.Incident.Repository.Data;

namespace Honeywell.GateWay.Incident.Repository
{
    public interface IProwatchRepository
    {
        Task<ProwatchDevicesEntity> GetDevices();
    }
}
