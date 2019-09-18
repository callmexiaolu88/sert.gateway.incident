using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Create;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Honeywell.Micro.Services.Incident.Domain.Shared;

namespace Honeywell.GateWay.Incident.ApplicationStub.Instance
{
    class StubIncidentAppService : IIncidentMicroApi
    {
        public Task<CreateIncidentResponseDto> Create(CreateIncidentRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<GetIncidentDetailsResponseDto> GetDetails(GetIncidentDetailsRequestDto request)
        {
            var detail = new IncidentDto()
            {
                Id = Guid.NewGuid(),
                Description = "One morning, when Gregor Samsa woke from troubled dreams, he found himself transformed in his bed into a" +
                "horrible vermin.He lay on his armour - like back," +
                "and if he lifted his head a little he could see his brown belly",
                Number = 137,
                CreateAtUtc = DateTime.UtcNow,
                LastUpdateAtUtc = DateTime.UtcNow,
                Owner = "Admin1",
                Priority = IncidentPriority.High,
                State = IncidentState.Active,
                WorkflowId = Guid.NewGuid(),
            };
            var response = new GetIncidentDetailsResponseDto()
            {
                IsSuccess =  true,
                Details = new List<IncidentDto>() { detail }
            };
            return Task.FromResult(response);
        }
    }
}
