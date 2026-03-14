namespace MoodTracker.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
