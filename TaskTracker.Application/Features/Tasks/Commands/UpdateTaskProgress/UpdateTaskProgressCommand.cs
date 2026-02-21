using MediatR;

namespace TaskTracker.Application.Features.Tasks.Commands.UpdateTaskProgress;

public record UpdateTaskProgressCommand : IRequest
{
    public Guid Id { get; init; }
    public decimal PercentComplete { get; init; }
}
