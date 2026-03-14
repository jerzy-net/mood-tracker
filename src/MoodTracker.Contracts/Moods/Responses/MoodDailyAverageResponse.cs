namespace MoodTracker.Contracts.Moods.Responses;

public class MoodDailyAverageResponse
{
    public DateOnly Date { get; init; }

    public decimal Average { get; init; }
}
