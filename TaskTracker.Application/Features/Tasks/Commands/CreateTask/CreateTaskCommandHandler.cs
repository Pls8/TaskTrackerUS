using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;

namespace TaskTracker.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            // Ensure dates are UTC
            StartDate = request.StartDate.HasValue ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc) : null,
            EndDate = request.EndDate.HasValue ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc) : null,
            Owner = request.Owner,
            Resource = request.Resource,
            Remark = request.Remark,
            TaskWeightPercentage = request.TaskWeightPercentage,
            TaskCompletionPercentage = 0, // Default to 0
            
            Priority = request.Priority,
            Status = TaskStatus.ToDo,
            
            IsSection = request.IsSection,
            ParentTaskId = request.ParentTaskId,
            
            CreatedAt = DateTime.UtcNow, // Use UtcNow
            CreatedBy = "System" // Placeholder
        };

        // Weight Distribution Logic
        if (request.ParentTaskId.HasValue)
        {
            var parent = await _context.ProjectTasks
                .Include(t => t.ChildTasks)
                .FirstOrDefaultAsync(t => t.Id == request.ParentTaskId.Value, cancellationToken);

            if (parent != null)
            {
                // Add the new entity to the count (it's not in ChildTasks yet)
                int totalChildren = parent.ChildTasks.Count + 1;
                
                if (parent.TaskWeightPercentage.HasValue && totalChildren > 0)
                {
                    decimal distributedWeight = parent.TaskWeightPercentage.Value / totalChildren;
                    
                    // Set this task's weight
                    entity.TaskWeightPercentage = distributedWeight;

                    // Update siblings
                    foreach (var child in parent.ChildTasks)
                    {
                        child.TaskWeightPercentage = distributedWeight;
                        // Recalculate sibling's weighted progress
                        if (child.TaskCompletionPercentage.HasValue)
                        {
                            child.TaskWeightedProgressPercentage = (child.TaskCompletionPercentage.Value * child.TaskWeightPercentage.Value) / 100;
                        }
                    }
                }
            }
        }

        _context.ProjectTasks.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
