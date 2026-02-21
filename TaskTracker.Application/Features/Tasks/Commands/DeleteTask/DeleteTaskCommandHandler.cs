using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectTasks
            .Include(t => t.ParentTask)
            .ThenInclude(p => p.ChildTasks)
            .Include(t => t.ChildTasks) // If we delete a parent, what happens to children? Restrict/Cascade?
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
        }

        // 1. If it's a child, remove from parent and redistribute weight to siblings
        if (entity.ParentTaskId.HasValue && entity.ParentTask != null)
        {
            var parent = entity.ParentTask;
            var remainingSiblings = parent.ChildTasks.Where(c => c.Id != entity.Id).ToList();
            int count = remainingSiblings.Count;
            
            if (parent.TaskWeightPercentage.HasValue && count > 0)
            {
                decimal newWeight = parent.TaskWeightPercentage.Value / count;
                foreach (var sibling in remainingSiblings)
                {
                    sibling.TaskWeightPercentage = newWeight;
                    if (sibling.TaskCompletionPercentage.HasValue)
                        sibling.TaskWeightedProgressPercentage = (sibling.TaskCompletionPercentage.Value * sibling.TaskWeightPercentage.Value) / 100;
                }
            }
        }

        // 2. If it's a parent, we configured Restrict delete in EF config, so this will fail if children exist.
        // We should probably check and throw a better error, or delete children (Cascade).
        // For now, let's assume we can't delete a parent with children unless children are deleted first.
        // Or we could implement Cascade manually if EF restrict is in place?
        // Let's rely on EF behavior but if we wanted to support cascade delete logic here:
        // if (entity.ChildTasks.Any()) { _context.ProjectTasks.RemoveRange(entity.ChildTasks); }
        
        // Since we used .OnDelete(DeleteBehavior.Restrict), this .Remove() will throw DbUpdateException if children exist.

        _context.ProjectTasks.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
