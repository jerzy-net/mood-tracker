using MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;
using MoodTracker.Domain.Entities;

namespace MoodTracker.Application.Abstractions;

public interface IMoodEntryRepository
{
    Task AddAsync(MoodEntry moodEntry, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid userId, DateTime recordedAtUtc, CancellationToken cancellationToken = default);

    Task<decimal?> GetDailyAverageAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<PagedResult<MoodEntry>> GetRawHistoryAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<MoodDailyAverage>> GetDailyAveragesAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
}
