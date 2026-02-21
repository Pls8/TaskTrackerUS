using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProjectTask, TaskDto>()
            .ForMember(d => d.ParentTaskDescription, opt => opt.MapFrom(s => s.ParentTask != null ? s.ParentTask.Description : string.Empty));
        CreateMap<ProjectSite, SiteDto>();
    }
}
