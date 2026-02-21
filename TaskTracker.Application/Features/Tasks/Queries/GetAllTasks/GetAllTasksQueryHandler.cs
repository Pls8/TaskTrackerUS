using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllTasksQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        // Fetch all tasks with children to calculate progress
        var tasks = await _context.ProjectTasks
            .Include(t => t.ChildTasks)
            .Include(t => t.ParentTask)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        // 1. Calculate Section Progress (Dynamically ensure it's correct)
        foreach (var task in tasks.Where(t => t.IsSection))
        {
            // Section Progress = Sum of children's weighted progress
            if (task.ChildTasks.Any())
            {
                task.SectionProgressPercentage = task.ChildTasks.Sum(c => c.TaskWeightedProgressPercentage ?? 0);
            }
            else
            {
                 task.SectionProgressPercentage = 0;
            }
        }

        // 2. Calculate Overall Platform Progress
        var totalPlatformProgress = tasks.Where(t => t.ParentTaskId == null)
            .Sum(t => t.IsSection ? (t.SectionProgressPercentage ?? 0) : (t.TaskWeightedProgressPercentage ?? 0));

        // 3. Map to DTOs
        var dtos = _mapper.Map<List<TaskDto>>(tasks);

        // 4. Assign Global Progress and Propagate Section Progress
        // Create a dictionary for faster lookup if needed, but list is small enough usually.
        var sectionProgressMap = tasks.Where(t => t.IsSection).ToDictionary(t => t.Id, t => t.SectionProgressPercentage);

        foreach (var dto in dtos)
        {
            dto.OverallPlatformProgressPercentage = totalPlatformProgress;

            // If it's a child task, show its Parent's Section Progress
            if (dto.ParentTaskId.HasValue && sectionProgressMap.TryGetValue(dto.ParentTaskId.Value, out var parentProgress))
            {
                dto.SectionProgressPercentage = parentProgress;
            }
        }

        return dtos;
    }
}
