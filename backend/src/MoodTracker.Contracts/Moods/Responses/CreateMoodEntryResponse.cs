namespace MoodTracker.Contracts.Moods.Responses;

public class CreateMoodEntryResponse
{
    public Guid Id { get; init; }

    public decimal DailyAverage { get; init; }

    public string ComparisonMessage { get; init; } = string.Empty;

    public DateTime RecordedAtUtc { get; init; }
}
