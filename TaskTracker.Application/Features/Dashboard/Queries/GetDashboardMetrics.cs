using MediatR;
using TaskTracker.Application.Features.Dashboard.DTOs;

namespace TaskTracker.Application.Features.Dashboard.Queries;

public record GetDashboardMetricsQuery : IRequest<DashboardMetricsDto>;
