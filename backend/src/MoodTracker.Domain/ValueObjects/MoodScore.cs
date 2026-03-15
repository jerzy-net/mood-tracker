namespace MoodTracker.Domain.ValueObjects;

public sealed class MoodScore : ValueObject
{
    private const int Minimum = -5;
    private const int Maximum = 5;

    private MoodScore(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static MoodScore Create(int value)
    {
        if (value is < Minimum or > Maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"Mood score must be between {Minimum} and {Maximum}.");
        }

        return new MoodScore(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator int(MoodScore score) => score.Value;
}
