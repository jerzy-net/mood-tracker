using MoodTracker.Contracts.Common;

namespace MoodTracker.Contracts.Moods.Responses;

public class MoodHistoryResponse
{
    public string Mode { get; init; } = string.Empty;

    public PagedResponse<MoodHistoryEntryResponse>? RawEntries { get; set; }

    public PagedResponse<MoodDailyAverageResponse>? DailyAverages { get; set; }
}
