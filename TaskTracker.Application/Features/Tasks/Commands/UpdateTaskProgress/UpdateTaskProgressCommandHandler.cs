using MediatR;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;

namespace TaskTracker.Application.Features.Tasks.Commands.UpdateTaskProgress;

public class UpdateTaskProgressCommandHandler : IRequestHandler<UpdateTaskProgressCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTaskProgressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTaskProgressCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectTasks
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Task with ID {request.Id} not found.");
        }

        entity.TaskCompletionPercentage = request.PercentComplete;

        if (entity.TaskCompletionPercentage == 100)
        {
            entity.Status = TaskStatus.Completed;
            entity.CompletedDate = DateTime.UtcNow; // Use UtcNow
        }
        else if (entity.TaskCompletionPercentage > 0 && entity.Status == TaskStatus.ToDo)
        {
            entity.Status = TaskStatus.InProgress;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
