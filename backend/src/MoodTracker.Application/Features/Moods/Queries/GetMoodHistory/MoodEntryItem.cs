namespace MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;

public sealed record MoodEntryItem(Guid Id, int MoodScore, DateTime RecordedAtUtc);
