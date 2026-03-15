namespace MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;

public sealed record MoodDailyAverage(DateOnly Date, decimal Average);
