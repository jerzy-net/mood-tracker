namespace MoodTracker.Application.Exceptions;

public class DuplicateMoodEntryException : Exception
{
    public DuplicateMoodEntryException(Guid userId, DateTime recordedAtUtc)
        : base($"Mood entry for user '{userId}' at '{recordedAtUtc:o}' already exists.")
    {
        UserId = userId;
        RecordedAtUtc = recordedAtUtc;
    }

    public Guid UserId { get; }

    public DateTime RecordedAtUtc { get; }
}
