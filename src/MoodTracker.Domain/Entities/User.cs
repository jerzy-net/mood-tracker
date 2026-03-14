using MoodTracker.Domain.Abstractions;

namespace MoodTracker.Domain.Entities;

public class User : AuditableEntity
{
    private readonly List<MoodEntry> _moodEntries = new();

    private User()
    {
    }

    private User(Guid id, string email, DateTime createdAtUtc)
    {
        Id = id;
        Email = email;
        CreatedAtUtc = createdAtUtc;
    }

    public string Email { get; private set; } = string.Empty;

    public IReadOnlyCollection<MoodEntry> MoodEntries => _moodEntries;

    public static User Create(string email, DateTime createdAtUtc)
    {
        return new User(Guid.NewGuid(), email, createdAtUtc);
    }
}
