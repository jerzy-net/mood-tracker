namespace MoodTracker.Domain.ValueObjects;

public sealed class RecordedAt : ValueObject
{
    private RecordedAt(DateTime valueUtc)
    {
        Value = valueUtc;
    }

    public DateTime Value { get; }

    public DateOnly Date => DateOnly.FromDateTime(Value);

    public static RecordedAt Create(DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value.ToUniversalTime()
        };

        return new RecordedAt(utcValue);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
