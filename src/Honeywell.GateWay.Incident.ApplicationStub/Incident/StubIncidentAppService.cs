using System.Threading.Tasks;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Create;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;

namespace Honeywell.GateWay.Incident.ApplicationStub.Incident
{
    public class StubIncidentAppService : BaseIncidentStub, IIncidentMicroApi
    {
        public Task<CreateIncidentResponseDto> Create(CreateIncidentRequestDto request)
        {
            return StubData<CreateIncidentResponseDto>();
        }

        public Task<GetIncidentDetailsResponseDto> GetDetails(GetIncidentDetailsRequestDto request)
        {
            return StubData<GetIncidentDetailsResponseDto>();
        }
    }
}
