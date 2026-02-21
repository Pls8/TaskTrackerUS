namespace TaskTracker.Domain.Entities;

public class Milestone
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsAchieved { get; set; }
    
    public Guid SiteId { get; set; }
    public ProjectSite Site { get; set; } = null!;
}
