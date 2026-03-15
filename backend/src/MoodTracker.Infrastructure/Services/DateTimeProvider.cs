using MoodTracker.Application.Abstractions;

namespace MoodTracker.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
