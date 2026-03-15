using MoodTracker.Domain.Abstractions;
using MoodTracker.Domain.ValueObjects;

namespace MoodTracker.Domain.Entities;

public class MoodEntry : AuditableEntity
{
    private MoodEntry()
    {
    }

    private MoodEntry(Guid? userId, MoodScore moodScore, RecordedAt recordedAt, DateTime createdAtUtc)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        MoodScore = moodScore;
        RecordedAt = recordedAt;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid? UserId { get; private set; }

    public MoodScore MoodScore { get; private set; } = null!;

    public RecordedAt RecordedAt { get; private set; } = null!;

    public User? User { get; private set; }

    public static MoodEntry Create(Guid? userId, MoodScore moodScore, RecordedAt recordedAt, DateTime createdAtUtc)
    {
        return new MoodEntry(userId, moodScore, recordedAt, createdAtUtc);
    }

    public void Touch(DateTime utcNow)
    {
        UpdatedAtUtc = utcNow;
    }
}
