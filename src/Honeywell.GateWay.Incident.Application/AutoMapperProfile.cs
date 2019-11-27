using AutoMapper;
using Honeywell.Facade.Services.Incident.Api.Incident.Details;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.getIds;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Statistics;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.List;
using FacadeApi = Honeywell.Facade.Services.Incident.Api.Incident.Create;
using IncidentPriority = Honeywell.Gateway.Incident.Api.Incident.GetDetail.IncidentPriority;
using WorkflowStepDto = Honeywell.Micro.Services.Workflow.Api.Workflow.Details.WorkflowStepDto;

namespace Honeywell.GateWay.Incident.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<WorkflowStepDesignDto, WorkflowStepDesignGto>();

            CreateMap<WorkflowDesignDto, WorkflowDesignDetailGto>();

            CreateMap<WorkflowDesignListDto, WorkflowDesignListGto>();

            CreateMap<WorkflowDesignSelectorDto, WorkflowDesignSelectorGto>();

            CreateMap<IncidentListItemDto, IncidentSummaryGto>();

            CreateMap<IncidentDto, IncidentDetailGto>();
            CreateMap<WorkflowDto, IncidentDetailGto>()
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

            CreateMap<IncidentDetailDto, IncidentDetailGto>()
                .ForMember(dest => dest.IncidentSteps, opt => opt.MapFrom(src => src.WorkflowSteps));

            CreateMap<StepCommentDto, StepCommentGto>();

            //CreateIncidentByAlarm Request
            CreateMap<CreateIncidentByAlarmRequestGto, FacadeApi.CreateIncidentByAlarmDto>();
            CreateMap<IncidentPriority, FacadeApi.IncidentPriority>();
            CreateMap<AlarmData, FacadeApi.AlarmData>();

            //GetWorkflowDesignIds Response
            CreateMap<WorkflowDesignListDto, WorkflowDesignIdGto>()
                .ForMember(dest => dest.WorkflowDesignReferenceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            //GetWorkflowDesigns Request
            CreateMap<GetDetailsRequestGto, WorkflowDesignDetailsRequestDto>();
            //GetWorkflowDesigns Response

            //GetIncidentStatusWithAlarmId Request
            //GetIncidentStatusWithAlarmId Response
            CreateMap<IncidentStatusDto, IncidentStatusInfoGto>()
                .ForMember(dest => dest.AlarmId, opt => opt.MapFrom(src => src.TriggerId));
            CreateMap<IncidentState, IncidentStatus>();

            CreateMap<Honeywell.Facade.Services.Incident.Api.Incident.Create.AlarmData, AlarmData>();

            CreateMap<IncidentActivityDto, ActivityGto>()
                .ForMember(dest => dest.Operator, opt => opt.MapFrom(src => src.Manipulator));
            CreateMap<IncidentStatisticsDto, IncidentStatisticsGto>();
        }
    }
}
