using System.ComponentModel.DataAnnotations;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Web.ViewModels;

public class UpdateTaskViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string? Owner { get; set; }
    
    public string? Resource { get; set; }
    
    public string? Remark { get; set; }
    
    public decimal? TaskWeightPercentage { get; set; }
    
    public decimal? TaskCompletionPercentage { get; set; }
    
    [Required]
    public PriorityLevel Priority { get; set; }

    public bool IsSection { get; set; }
    public Guid? ParentTaskId { get; set; }
}
