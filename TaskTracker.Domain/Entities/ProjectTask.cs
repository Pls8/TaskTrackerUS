using TaskTracker.Domain.Enums;
using TaskTracker.Domain.Interfaces;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;

namespace TaskTracker.Domain.Entities;

public class ProjectTask : IAuditableEntity
{
    public Guid Id { get; set; }
    
    // Mapped from "Task Description" - Required
    public string Description { get; set; } = string.Empty;
    
    // Mapped from "Start Date" - Nullable
    public DateTime? StartDate { get; set; }
    
    // Mapped from "End Date" - Nullable (Renamed from DueDate to align with PDF)
    public DateTime? EndDate { get; set; }
    
    // Mapped from "Owner" - Nullable
    public string? Owner { get; set; }
    
    // Mapped from "Resource" - Nullable
    public string? Resource { get; set; }
    
    // Mapped from "Remark" - Nullable
    public string? Remark { get; set; }
    
    // Mapped from "Task Completion (%)" - Nullable
    public decimal? TaskCompletionPercentage { get; set; }
    
    // Mapped from "Task Weight (%)" - Nullable
    public decimal? TaskWeightPercentage { get; set; }
    
    // Mapped from "Task Weighted Progress (%)" - Nullable
    public decimal? TaskWeightedProgressPercentage { get; set; }
    
    // Mapped from "Section Progress (%)" - Nullable
    public decimal? SectionProgressPercentage { get; set; }
    
    // Mapped from "Overall Platform Progress (%)" - Nullable
    public decimal? OverallPlatformProgressPercentage { get; set; }
    
    // Kept existing properties that might still be useful or need migration
    public string Title { get; set; } = string.Empty; 
    public PriorityLevel Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime? CompletedDate { get; set; }

    // Hierarchy
    public bool IsSection { get; set; }
    public Guid? ParentTaskId { get; set; }
    public ProjectTask? ParentTask { get; set; }
    public ICollection<ProjectTask> ChildTasks { get; set; } = new List<ProjectTask>();
    
    // Auditing
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
