﻿using AutoMapper;
using Honeywell.Gateway.Incident.Api.Incident.Create;
using Honeywell.Gateway.Incident.Api.Incident.GetDetail;
using Honeywell.Gateway.Incident.Api.Incident.GetList;
using Honeywell.Gateway.Incident.Api.Incident.GetStatus;
using Honeywell.Gateway.Incident.Api.Incident.Statistics;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetDetail;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetList;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetSelector;
using Honeywell.Gateway.Incident.Api.WorkflowDesign.GetIds;
using Honeywell.Micro.Services.Incident.Api.Incident.Create;
using Honeywell.Micro.Services.Incident.Api.Incident.Details;
using Honeywell.Micro.Services.Incident.Api.Incident.List;
using Honeywell.Micro.Services.Incident.Api.Incident.Statistics;
using Honeywell.Micro.Services.Incident.Api.Incident.Status;
using Honeywell.Micro.Services.Incident.Domain.Shared;
using Honeywell.Micro.Services.Workflow.Api.Workflow.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.List;
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

            CreateMap<CreateIncidentByAlarmResponseDto, CreateIncidentByAlarmResponseGto>();
            CreateMap<IncidentAlarmDto, IncidentAlarmGto>();

            CreateMap<IncidentDetailDto, IncidentDetailGto>();
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
            CreateMap<Micro.Services.Incident.Api.Incident.Details.WorkflowStepDto, IncidentStepGto>();

            CreateMap<IncidentDetailDto, IncidentDetailGto>()
                .ForMember(dest => dest.IncidentSteps, opt => opt.MapFrom(src => src.WorkflowSteps));

            CreateMap<StepCommentDto, StepCommentGto>();

            //CreateIncidentByAlarm Request
            CreateMap<CreateIncidentByAlarmRequestGto, CreateIncidentByAlarmDto>();
            CreateMap<IncidentPriority, IncidentPriority>();
            CreateMap<AlarmData, Micro.Services.Incident.Api.Incident.Trigger.AlarmData>();

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

            CreateMap<Micro.Services.Incident.Api.Incident.Trigger.AlarmData, AlarmData>();

            CreateMap<IncidentActivityDto, ActivityGto>()
                .ForMember(dest => dest.Operator, opt => opt.MapFrom(src => src.Manipulator));
            CreateMap<IncidentStatisticsDto, IncidentStatisticsGto>();
        }
    }
}
