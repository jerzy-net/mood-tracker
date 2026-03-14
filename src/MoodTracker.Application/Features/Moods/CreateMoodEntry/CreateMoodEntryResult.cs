namespace MoodTracker.Application.Features.Moods.CreateMoodEntry;

public sealed record CreateMoodEntryResult(Guid Id, DateTime RecordedAtUtc, decimal DailyAverage, string ComparisonMessage);
