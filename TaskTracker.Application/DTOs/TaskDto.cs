using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;

namespace TaskTracker.Application.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Owner { get; set; }
    public string? Resource { get; set; }
    public string? Remark { get; set; }
    public decimal? TaskCompletionPercentage { get; set; }
    public decimal? TaskWeightPercentage { get; set; }
    public decimal? TaskWeightedProgressPercentage { get; set; }
    public decimal? SectionProgressPercentage { get; set; }
    public decimal? OverallPlatformProgressPercentage { get; set; }
    
    // Kept for backward compatibility or display
    public string Title { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime? CompletedDate { get; set; }

    public bool IsSection { get; set; }
    public Guid? ParentTaskId { get; set; }
    public string? ParentTaskDescription { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? LastModifiedBy { get; set; }
}
