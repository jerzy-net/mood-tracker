namespace MoodTracker.Contracts.Moods.Requests;

public class CreateMoodEntryRequest
{
    public Guid? UserId { get; set; }

    public int MoodScore { get; set; }

    public DateTime? RecordedAtUtc { get; set; }
}
