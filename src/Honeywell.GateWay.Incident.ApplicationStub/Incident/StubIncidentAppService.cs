﻿using System;
using System.Threading.Tasks;
using Honeywell.Micro.Services.Incident.Api;
using Honeywell.Micro.Services.Incident.Api.Incident.Create;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.Respond;
using Honeywell.Micro.Services.Incident.Api.Incident.Takeover;

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

        public Task<RespondIncidentResponseDto> Respond(RespondIncidentRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<TakeoverIncidentResponseDto> Takeover(TakeoverIncidentRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
