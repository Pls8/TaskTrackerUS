using MediatR;
using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;

namespace TaskTracker.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Owner { get; init; }
    public string? Resource { get; init; }
    public string? Remark { get; init; }
    public decimal? TaskWeightPercentage { get; init; }
    public PriorityLevel Priority { get; init; }
    public bool IsSection { get; init; }
    public Guid? ParentTaskId { get; init; }
}
