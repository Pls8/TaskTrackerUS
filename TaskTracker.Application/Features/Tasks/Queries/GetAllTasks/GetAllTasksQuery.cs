using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Tasks.Queries.GetAllTasks;

public record GetAllTasksQuery : IRequest<List<TaskDto>>;
