using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Persistence.Configurations;

public class ProjectSiteConfiguration : IEntityTypeConfiguration<ProjectSite>
{
    public void Configure(EntityTypeBuilder<ProjectSite> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Location)
            .HasMaxLength(500);
            
        // Ignored properties are calculated properties
        builder.Ignore(s => s.OverallProgress);
    }
}
