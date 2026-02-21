using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class ProjectSite
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public SiteStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Calculated properties
    public decimal OverallProgress => CalculateProgress();
    
    // Navigation
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    private decimal CalculateProgress()
    {
        if (Tasks == null || !Tasks.Any())
            return 0;
            
        // Use TaskCompletionPercentage if available, otherwise 0
        return Tasks.Average(t => t.TaskCompletionPercentage ?? 0);
    }
}
