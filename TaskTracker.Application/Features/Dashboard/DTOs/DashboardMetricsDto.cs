using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Dashboard.DTOs;

public class DashboardMetricsDto
{
    public int TotalTasks { get; set; }
    public decimal OverallCompletionPercentage { get; set; }
    public int TasksInProgress { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TotalRemainingDays { get; set; }
    
    // Chart data
    public List<TaskStatusDistributionDto> TaskDistribution { get; set; } = new();
    
    // Additional metrics for the new columns
    public decimal AverageTaskWeight { get; set; }
    public decimal TotalWeightedProgress { get; set; }

    // New Lists for Dashboard Redesign
    public List<TaskDto> RecentActivities { get; set; } = new();
    public List<TaskDto> CurrentProjects { get; set; } = new();
    public List<TaskDto> UpcomingDeadlines { get; set; } = new();
}

public class TaskStatusDistributionDto
{
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}
