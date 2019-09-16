using AutoMapper;
using Honeywell.Gateway.Incident.Api.Gtos;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Details;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Selector;
using Honeywell.Micro.Services.Workflow.Api.WorkflowDesign.Summary;

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
        }
    }
}
