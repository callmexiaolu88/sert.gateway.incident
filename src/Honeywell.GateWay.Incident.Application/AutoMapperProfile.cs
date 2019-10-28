using AutoMapper;
using FacadeApi= Honeywell.Facade.Services.Incident.Api.Incident.Create;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.Status;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.Detail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;
using IncidentPriority = Honeywell.Gateway.Incident.Api.Gtos.IncidentPriority;
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

            CreateMap<StepCommentDto, StepCommentGto>();

            //CreateIncidentByAlarm Request
            CreateMap<CreateByAlarmRequestGto, FacadeApi.CreateIncidentByAlarmRequestDto>()
                .ForMember(dest => dest.CreateIncidentDatas, opt => opt.MapFrom(src => src.CreateDatas));
            CreateMap<CreateByAlarmGto, FacadeApi.CreateIncidentByAlarmDto>();
            CreateMap<IncidentPriority, FacadeApi.IncidentPriority>();
            CreateMap<AlarmData, FacadeApi.AlarmData>();
            //CreateIncidentByAlarm Response
            CreateMap<FacadeApi.CreateIncidentResponseDto, CreateIncidentResponseGto>();

            //GetWorkflowDesignIds Response
            CreateMap<WorkflowDesignSummaryResponseDto, GetWorkflowDesignIdsResponseGto>()
                .ForMember(dest => dest.WorkflowDesignIds, opt => opt.MapFrom(src => src.Summaries));
            CreateMap<WorkflowDesignSummaryDto, WorkflowDesignIdGto>()
                .ForMember(dest => dest.WorkflowDesignReferenceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            //GetWorkflowDesigns Request
            CreateMap<GetWorkflowDesignDetailsRequestGto, WorkflowDesignDetailsRequestDto>();
            //GetWorkflowDesigns Response
            CreateMap<WorkflowDesignResponseDto, GetWorkflowDesignDetailsResponseGto>()
                .ForMember(dest => dest.WorkflowDesigns, opt => opt.MapFrom(src => src.Details));

            //GetIncidentStatusWithAlarmId Request
            CreateMap<GetStatusByAlarmRequestGto, GetIncidentStatusRequestDto>()
                .ForMember(dest => dest.TriggerIds, opt => opt.MapFrom(src => src.AlarmIds));
            //GetIncidentStatusWithAlarmId Response
            CreateMap<GetIncidentStatusResponseDto, GetStatusByAlarmResponseGto>();
            CreateMap<IncidentStatusDto, IncidentStatusInfoGto>()
                .ForMember(dest => dest.AlarmId, opt => opt.MapFrom(src => src.TriggerId));
            CreateMap<IncidentState, IncidentStatus>();
        }
    }
}
