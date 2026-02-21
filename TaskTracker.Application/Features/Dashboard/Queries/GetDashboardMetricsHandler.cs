using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.Features.Dashboard.DTOs;
using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Dashboard.Queries;

public class GetDashboardMetricsHandler : IRequestHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDashboardMetricsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
    {
        // Use AsNoTracking for read-only queries to improve performance
        var query = _context.ProjectTasks.AsNoTracking();

        // 1. Aggregations (Executed on DB side)
        var totalTasks = await query.CountAsync(cancellationToken);
        
        // Handle potentially empty table for averages
        var avgCompletion = totalTasks > 0 
            ? await query.AverageAsync(t => t.TaskCompletionPercentage ?? 0, cancellationToken) 
            : 0;
            
        var avgWeight = totalTasks > 0
            ? await query.AverageAsync(t => t.TaskWeightPercentage ?? 0, cancellationToken)
            : 0;

        var inProgressCount = await query.CountAsync(t => t.Status == TaskStatus.InProgress, cancellationToken);
        var completedCount = await query.CountAsync(t => t.Status == TaskStatus.Completed, cancellationToken);
        
        var overdueCount = await query.CountAsync(t => 
            t.Status != TaskStatus.Completed && 
            t.EndDate.HasValue && 
            DateTime.UtcNow > t.EndDate.Value, cancellationToken);

        // Summing days remaining - careful with client-side evaluation if provider doesn't support date diffs perfectly
        // For simplicity and safety with various DBs, we can fetch just the dates needed or accept client eval for this specific small set
        // But better to filter in DB.
        var remainingDays = await query
            .Where(t => t.Status != TaskStatus.Completed && t.EndDate.HasValue && t.EndDate.Value > DateTime.UtcNow)
            .Select(t => t.EndDate)
            .ToListAsync(cancellationToken);
            
        var totalRemainingDays = remainingDays.Sum(d => (d.GetValueOrDefault() - DateTime.UtcNow).Days);

        // Weighted Progress Sum - handle nullable types safely in expression tree
        var totalWeightedProgress = await query.SumAsync(t => (decimal?)(t.TaskCompletionPercentage * t.TaskWeightPercentage / 100) ?? 0, cancellationToken);

        // Task Distribution (Group By in DB)
        var taskDistribution = await query
            .GroupBy(t => t.Status)
            .Select(g => new TaskStatusDistributionDto
            {
                StatusName = g.Key.ToString(),
                Count = g.Count()
            })
            .ToListAsync(cancellationToken);

        // 2. Efficient List Retrieval (Fetch only what's needed)
        
        // Recent Activities (Top 5)
        var recentTasks = await query
            .OrderByDescending(t => t.LastModifiedAt ?? t.CreatedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        // Current Projects (Top 4 Sections)
        var currentProjects = await query
            .Where(t => t.IsSection)
            .OrderByDescending(t => t.CreatedAt)
            .Take(4)
            .ToListAsync(cancellationToken);

        // Upcoming Deadlines (Top 5)
        var upcomingDeadlines = await query
            .Where(t => t.Status != TaskStatus.Completed && t.EndDate.HasValue && t.EndDate.Value >= DateTime.UtcNow)
            .OrderBy(t => t.EndDate)
            .Take(5)
            .ToListAsync(cancellationToken);

        var metrics = new DashboardMetricsDto
        {
            TotalTasks = totalTasks,
            OverallCompletionPercentage = avgCompletion,
            TasksInProgress = inProgressCount,
            CompletedTasks = completedCount,
            OverdueTasks = overdueCount,
            TotalRemainingDays = totalRemainingDays,
            AverageTaskWeight = avgWeight,
            TotalWeightedProgress = totalWeightedProgress,
            TaskDistribution = taskDistribution,
            
            // Map the limited lists
            RecentActivities = _mapper.Map<List<TaskDto>>(recentTasks),
            CurrentProjects = _mapper.Map<List<TaskDto>>(currentProjects),
            UpcomingDeadlines = _mapper.Map<List<TaskDto>>(upcomingDeadlines)
        };

        return metrics;
    }
}
