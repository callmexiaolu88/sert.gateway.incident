using AutoMapper;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using WorkflowStepDto = Honeywell.Micro.Services.Workflow.Api.Workflow.Details.WorkflowStepDto;

namespace Honeywell.GateWay.Incident.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<WorkflowStepDesignDto, WorkflowStepDesignGto>();

            CreateMap<WorkflowDesignDto, WorkflowDesignGto>();

            CreateMap<WorkflowDesignSummaryDto, WorkflowDesignSummaryGto>();

            CreateMap<WorkflowDesignSelectorDto, WorkflowDesignSelectorGto>();

            CreateMap<IncidentListItemDto, ActiveIncidentGto>();

            CreateMap<IncidentDto, IncidentGto>();
            CreateMap<WorkflowDto, IncidentGto>()
                .ForMember(dest => dest.WorkflowName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.WorkflowDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.WorkflowOwner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.CreateAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore());
            CreateMap<WorkflowStepDto, IncidentStepGto>();

            CreateMap<ActivityDto, ActivityGto>();
            CreateMap<Facade.Services.Incident.Api.Incident.Details.WorkflowStepDto, IncidentStepGto>();

            CreateMap<IncidentDetailDto, IncidentGto>()
                .ForMember(dest => dest.IncidentSteps, opt => opt.MapFrom(src => src.WorkflowSteps));
            CreateMap<WorkflowStepActivityDto, WorkflowActivitiesGto>();
            
        }
    }
}
