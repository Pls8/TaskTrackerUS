using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Persistence.Configurations;

public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.HasKey(t => t.Id);

        // Title is kept but optional/nullable in concept if Description is main, 
        // but for now let's keep it required or default empty if not used.
        // Given the request "Task Description only one that cannot be null",
        // we will ensure Description is Required.
        
        builder.Property(t => t.Description)
            .IsRequired(); // Task Description cannot be null

        // Other fields are nullable by default due to `?` in entity, 
        // but we can be explicit here if needed.
        
        builder.Property(t => t.StartDate).IsRequired(false);
        builder.Property(t => t.EndDate).IsRequired(false);
        builder.Property(t => t.Owner).IsRequired(false);
        builder.Property(t => t.Resource).IsRequired(false);
        builder.Property(t => t.Remark).IsRequired(false);
        
        // Decimal precision configuration is good practice
        builder.Property(t => t.TaskCompletionPercentage).HasPrecision(18, 2);
        builder.Property(t => t.TaskWeightPercentage).HasPrecision(18, 2);
        builder.Property(t => t.TaskWeightedProgressPercentage).HasPrecision(18, 2);
        builder.Property(t => t.SectionProgressPercentage).HasPrecision(18, 2);
        builder.Property(t => t.OverallPlatformProgressPercentage).HasPrecision(18, 2);

        // Hierarchy
        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.ChildTasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting parent if children exist, or use SetNull/Cascade as needed
    }
}
