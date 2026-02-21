using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskStatus = TaskTracker.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Define the 3 main Sections with their weights
        var sections = new List<(string Title, decimal Weight)>
        {
            ("Infrastructure & Setup", 30m),
            ("Integration Development", 30m),
            ("Testing & Deployment", 40m)
        };

        var sectionEntities = new Dictionary<string, ProjectTask>();

        foreach (var (title, weight) in sections)
        {
            var section = await context.ProjectTasks.FirstOrDefaultAsync(t => t.Title == title);
            if (section == null)
            {
                section = new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Description = title, // Description same as title for sections
                    IsSection = true,
                    TaskWeightPercentage = weight,
                    Status = TaskStatus.InProgress,
                    Priority = PriorityLevel.High,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    Owner = "Project Manager"
                };
                await context.ProjectTasks.AddAsync(section);
                Console.WriteLine($"[DatabaseSeeder] Created section: {title}");
            }
            else
            {
                // Ensure weight is correct even if exists
                if (section.TaskWeightPercentage != weight)
                {
                    section.TaskWeightPercentage = weight;
                    context.Entry(section).State = EntityState.Modified;
                }
            }
            sectionEntities[title] = section;
        }
        
        await context.SaveChangesAsync();

        // Define tasks and map them to sections
        // We use a tuple: (Title, SectionTitle, Description, ...)
        var tasksData = new List<(string Title, string Section, string Desc, decimal Completion, DateTime Start, DateTime End)>
        {
            // Section 1: Infrastructure & Setup (30%)
            ("Install OS", "Infrastructure & Setup", "Install OS on test machines", 100, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-2)),
            ("Test OS", "Infrastructure & Setup", "Verify OS installation", 100, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1)),
            ("Prepare physical servers", "Infrastructure & Setup", "This will be for temporary use. ITMS virtual server (1pcs): 48 core CPU, 64GB memory, 1.2TB SSD disk, CentOS 8", 100, new DateTime(2026, 1, 6), new DateTime(2026, 1, 16)),
            ("Network connection setup", "Infrastructure & Setup", "Connect to Driver & Vehicle Database, ROP current Violation Management System, and Active Directory", 50, new DateTime(2026, 1, 6), new DateTime(2026, 1, 23)),
            
            // Section 2: Integration Development (30%)
            ("Install ITMS test platform", "Integration Development", "R&D team will be on ROP office", 100, new DateTime(2026, 1, 19), new DateTime(2026, 1, 20)),
            ("Align integration protocol", "Integration Development", "Discuss integration approach with stakeholders", 50, new DateTime(2026, 1, 19), new DateTime(2026, 1, 19)),
            ("Check API documentation", "Integration Development", "Check compliance of current API documentation", 80, new DateTime(2026, 1, 20), new DateTime(2026, 1, 23)),
            ("Update API documents", "Integration Development", "Update API documents for Driver & Vehicle database", 50, new DateTime(2026, 1, 23), new DateTime(2026, 1, 31)),
            ("Active Directory access", "Integration Development", "Provide Active Directory server access parameters", 20, new DateTime(2026, 2, 1), new DateTime(2026, 2, 15)),

            // Section 3: Testing & Deployment (40%)
            ("Integration: Driver & Vehicle DB", "Testing & Deployment", "Program integration to ROP Driver & Vehicle database", 0, new DateTime(2026, 3, 1), new DateTime(2026, 3, 6)),
            ("Integration: Violation System", "Testing & Deployment", "Program integration to ROP existing violation management system", 0, new DateTime(2026, 3, 8), new DateTime(2026, 3, 13)),
            ("Integration: Active Directory & SSO", "Testing & Deployment", "Program integration to ROP Active Directory system & SSO", 0, new DateTime(2026, 3, 15), new DateTime(2026, 3, 20)),
            ("Test and bug resolving", "Testing & Deployment", "Comprehensive testing and bug fixes", 0, new DateTime(2026, 3, 22), new DateTime(2026, 3, 31)),
            ("Deployment Plan", "Testing & Deployment", "ITMS Platform Integration & Deployment Plan", 0, new DateTime(2026, 4, 1), new DateTime(2026, 4, 5))
        };

        var existingTasks = await context.ProjectTasks.Where(t => !t.IsSection).ToListAsync();
        var existingTitles = existingTasks.Select(t => t.Title).ToHashSet();

        foreach (var data in tasksData)
        {
            if (!existingTitles.Contains(data.Title))
            {
                var section = sectionEntities[data.Section];
                var task = new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    Title = data.Title,
                    Description = data.Desc,
                    IsSection = false,
                    ParentTaskId = section.Id,
                    TaskCompletionPercentage = data.Completion,
                    Status = data.Completion == 100 ? TaskStatus.Completed : (data.Completion > 0 ? TaskStatus.InProgress : TaskStatus.ToDo),
                    Priority = PriorityLevel.Medium,
                    StartDate = DateTime.SpecifyKind(data.Start, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(data.End, DateTimeKind.Utc),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    Owner = "System"
                };

                // We will calculate weights after adding all tasks to a section
                // But for seeding, we can pre-calculate if we know the count.
                // However, let's just add them and then run a distribution pass.
                
                await context.ProjectTasks.AddAsync(task);
            }
        }
        
        await context.SaveChangesAsync();

        // Post-processing: Calculate weights and progress for ALL tasks (including existing ones if any)
        foreach (var section in sectionEntities.Values)
        {
            // Reload children
            var children = await context.ProjectTasks.Where(t => t.ParentTaskId == section.Id).ToListAsync();
            
            if (children.Any() && section.TaskWeightPercentage.HasValue)
            {
                decimal weightPerTask = section.TaskWeightPercentage.Value / children.Count;
                
                foreach (var child in children)
                {
                    child.TaskWeightPercentage = weightPerTask;
                    if (child.TaskCompletionPercentage.HasValue)
                    {
                        child.TaskWeightedProgressPercentage = (child.TaskCompletionPercentage.Value * child.TaskWeightPercentage.Value) / 100;
                    }
                }
                
                // Calculate Section Progress
                section.SectionProgressPercentage = children.Sum(c => c.TaskWeightedProgressPercentage ?? 0);
            }
        }
        
        await context.SaveChangesAsync();
        Console.WriteLine("[DatabaseSeeder] Seeding and weight calculation completed.");
    }
}