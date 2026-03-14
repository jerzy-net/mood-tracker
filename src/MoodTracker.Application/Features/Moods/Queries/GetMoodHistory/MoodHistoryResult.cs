using MoodTracker.Application.Abstractions;

namespace MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;

public sealed record MoodHistoryResult(
    HistoryAggregationMode Mode,
    PagedResult<MoodEntryItem>? RawEntries,
    PagedResult<MoodDailyAverage>? DailyAverages);
