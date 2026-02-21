using TaskTracker.Application.Common.Interfaces;

namespace TaskTracker.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.UtcNow;
}
