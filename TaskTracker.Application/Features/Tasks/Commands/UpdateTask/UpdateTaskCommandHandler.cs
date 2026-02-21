using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;

namespace TaskTracker.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectTasks
            .Include(t => t.ParentTask)
            .ThenInclude(p => p.ChildTasks)
            .Include(t => t.ChildTasks)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
        }

        // Check if parent changed
        bool parentChanged = entity.ParentTaskId != request.ParentTaskId;
        Guid? oldParentId = entity.ParentTaskId;
        Guid? newParentId = request.ParentTaskId;

        entity.Description = request.Description;
        entity.Title = request.Title;
        entity.StartDate = request.StartDate.HasValue ? DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Utc) : null;
        entity.EndDate = request.EndDate.HasValue ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc) : null;
        entity.Owner = request.Owner;
        entity.Resource = request.Resource;
        entity.Remark = request.Remark;
        entity.Priority = request.Priority;
        entity.IsSection = request.IsSection;
        entity.ParentTaskId = request.ParentTaskId;

        // Weight Distribution Logic
        // If parent changed, we need to recalculate for old parent siblings AND new parent siblings
        if (parentChanged)
        {
            // 1. Handle Old Parent (Remove from siblings)
            if (oldParentId.HasValue)
            {
                var oldParent = await _context.ProjectTasks
                    .Include(t => t.ChildTasks)
                    .FirstOrDefaultAsync(t => t.Id == oldParentId.Value, cancellationToken);
                
                if (oldParent != null)
                {
                    // Filter out the current entity which is moving away
                    var remainingSiblings = oldParent.ChildTasks.Where(c => c.Id != entity.Id).ToList();
                    int count = remainingSiblings.Count;
                    
                    if (oldParent.TaskWeightPercentage.HasValue && count > 0)
                    {
                        decimal newWeight = oldParent.TaskWeightPercentage.Value / count;
                        foreach (var sibling in remainingSiblings)
                        {
                            sibling.TaskWeightPercentage = newWeight;
                            if (sibling.TaskCompletionPercentage.HasValue)
                                sibling.TaskWeightedProgressPercentage = (sibling.TaskCompletionPercentage.Value * sibling.TaskWeightPercentage.Value) / 100;
                        }
                    }
                }
            }

            // 2. Handle New Parent (Add to siblings)
            if (newParentId.HasValue)
            {
                var newParent = await _context.ProjectTasks
                    .Include(t => t.ChildTasks)
                    .FirstOrDefaultAsync(t => t.Id == newParentId.Value, cancellationToken);
                
                if (newParent != null)
                {
                    // Current entity is effectively added to count
                    int count = newParent.ChildTasks.Where(c => c.Id != entity.Id).Count() + 1;
                    
                    if (newParent.TaskWeightPercentage.HasValue && count > 0)
                    {
                        decimal newWeight = newParent.TaskWeightPercentage.Value / count;
                        
                        // Set current entity weight
                        entity.TaskWeightPercentage = newWeight;

                        // Update siblings
                        foreach (var sibling in newParent.ChildTasks.Where(c => c.Id != entity.Id))
                        {
                            sibling.TaskWeightPercentage = newWeight;
                            if (sibling.TaskCompletionPercentage.HasValue)
                                sibling.TaskWeightedProgressPercentage = (sibling.TaskCompletionPercentage.Value * sibling.TaskWeightPercentage.Value) / 100;
                        }
                    }
                }
            }
            else
            {
                // Moved to root (no parent) -> Keep manual weight or reset?
                // User input weight is used if provided, otherwise keep as is
                if (request.TaskWeightPercentage.HasValue)
                    entity.TaskWeightPercentage = request.TaskWeightPercentage;
            }
        }
        else
        {
            // Parent didn't change
            // If this task IS a parent (Section) and its weight changed, redistribute to children
            if (entity.IsSection && request.TaskWeightPercentage.HasValue && entity.TaskWeightPercentage != request.TaskWeightPercentage)
            {
                entity.TaskWeightPercentage = request.TaskWeightPercentage;
                if (entity.ChildTasks.Any())
                {
                    decimal childWeight = entity.TaskWeightPercentage.Value / entity.ChildTasks.Count;
                    foreach (var child in entity.ChildTasks)
                    {
                        child.TaskWeightPercentage = childWeight;
                        if (child.TaskCompletionPercentage.HasValue)
                            child.TaskWeightedProgressPercentage = (child.TaskCompletionPercentage.Value * child.TaskWeightPercentage.Value) / 100;
                    }
                }
            }
            else if (!entity.ParentTaskId.HasValue && request.TaskWeightPercentage.HasValue)
            {
                // Root task, just update weight
                entity.TaskWeightPercentage = request.TaskWeightPercentage;
            }
        }
        
        // Update completion logic
        if (request.TaskCompletionPercentage.HasValue)
        {
            entity.TaskCompletionPercentage = request.TaskCompletionPercentage;
            
            if (entity.TaskCompletionPercentage == 100)
            {
                entity.Status = TaskStatus.Completed;
                if (!entity.CompletedDate.HasValue)
                    entity.CompletedDate = DateTime.UtcNow;
            }
            else if (entity.TaskCompletionPercentage > 0)
            {
                entity.Status = TaskStatus.InProgress;
                entity.CompletedDate = null;
            }
            else
            {
                entity.Status = TaskStatus.ToDo;
                entity.CompletedDate = null;
            }
        }

        // Calculate weighted progress (Self)
        if (entity.TaskCompletionPercentage.HasValue && entity.TaskWeightPercentage.HasValue)
        {
            entity.TaskWeightedProgressPercentage = (entity.TaskCompletionPercentage.Value * entity.TaskWeightPercentage.Value) / 100;
        }
        else
        {
            entity.TaskWeightedProgressPercentage = 0;
        }

        // Recalculate Section Progress for Parent (if exists)
        // We need to do this after saving? Or we can do it now if we have the parent loaded.
        // It's safer to do a separate pass or ensure context tracks changes.
        // For simplicity, let's update the parent's SectionProgressPercentage now if loaded.
        
        if (entity.ParentTaskId.HasValue)
        {
            // We might need to reload parent to get updated child values if we modified them above
            // But since we modified tracked entities, EF Core should have the latest values in memory.
            var parent = entity.ParentTask ?? await _context.ProjectTasks
                .Include(t => t.ChildTasks)
                .FirstOrDefaultAsync(t => t.Id == entity.ParentTaskId.Value, cancellationToken);

            if (parent != null)
            {
                // Sum of children's weighted progress
                // Note: The current entity's WeightedProgress is already updated in memory above
                decimal totalWeightedProgress = parent.ChildTasks.Sum(c => c.TaskWeightedProgressPercentage ?? 0);
                parent.SectionProgressPercentage = totalWeightedProgress;
                
                // Also update Parent's Weighted Progress? 
                // "Section Progress (%) is driven(calculated) from Task Weighted Progress"
                // If the parent is just a container, SectionProgress IS its progress.
            }
        }

        // Update Overall Platform Progress
        // This is a global metric. Usually we'd calculate this on the fly in the Dashboard,
        // but if we need to store it, we'd need a place.
        // The user requirement says "Overall Platform Progress is driven from Section Progress".
        // If we store it on every task, it's redundant. Let's assume we just calculate it in Dashboard query.
        // But we can update the current task's OverallPlatformProgressPercentage property if it's meant to hold a snapshot?
        // Let's leave it as null/unused on the task entity for now and rely on Dashboard calculation,
        // unless the user specifically wants to see it on the grid row. 
        // If on the grid row, we'd need to fetch ALL sections to calculate it. That's expensive for every update.
        // Let's assume the Dashboard handles the global view.

        await _context.SaveChangesAsync(cancellationToken);
    }
}
