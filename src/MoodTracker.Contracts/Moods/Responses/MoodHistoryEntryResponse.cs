namespace MoodTracker.Contracts.Moods.Responses;

public class MoodHistoryEntryResponse
{
    public Guid Id { get; init; }

    public int MoodScore { get; init; }

    public DateTime RecordedAtUtc { get; init; }
}
