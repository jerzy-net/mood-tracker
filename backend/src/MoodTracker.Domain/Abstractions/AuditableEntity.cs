namespace MoodTracker.Domain.Abstractions;

public abstract class AuditableEntity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
