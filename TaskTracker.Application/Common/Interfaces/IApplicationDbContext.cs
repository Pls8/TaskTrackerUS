using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ProjectTask> ProjectTasks { get; }
    DbSet<ProjectSite> ProjectSites { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
