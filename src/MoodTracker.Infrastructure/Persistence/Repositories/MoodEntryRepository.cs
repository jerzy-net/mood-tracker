using Microsoft.EntityFrameworkCore;
using MoodTracker.Application.Abstractions;
using MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;
using MoodTracker.Domain.Entities;

namespace MoodTracker.Infrastructure.Persistence.Repositories;

public class MoodEntryRepository : IMoodEntryRepository
{
    private readonly MoodTrackerDbContext _dbContext;

    public MoodEntryRepository(MoodTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MoodEntry moodEntry, CancellationToken cancellationToken = default)
    {
        await _dbContext.MoodEntries.AddAsync(moodEntry, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid userId, DateTime recordedAtUtc, CancellationToken cancellationToken = default)
    {
        return _dbContext.MoodEntries.AnyAsync(
            entry => entry.UserId == userId && entry.RecordedAt.Value == recordedAtUtc,
            cancellationToken);
    }

    public async Task<decimal?> GetDailyAverageAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var (start, end) = GetDateRange(date);

        var query = _dbContext.MoodEntries
            .Where(entry => entry.RecordedAt.Value >= start && entry.RecordedAt.Value < end)
            .Select(entry => entry.MoodScore.Value);

        if (!await query.AnyAsync(cancellationToken))
        {
            return null;
        }

        var average = await query.AverageAsync(cancellationToken);
        return Math.Round((decimal)average, 2);
    }

    public async Task<PagedResult<MoodEntry>> GetRawHistoryAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MoodEntries
            .Where(entry => entry.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(entry => entry.RecordedAt.Value)
            .ThenByDescending(entry => entry.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<MoodEntry>(items, page, pageSize, totalCount);
    }

    public async Task<PagedResult<MoodDailyAverage>> GetDailyAveragesAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MoodEntries
            .Where(entry => entry.UserId == userId)
            .GroupBy(entry => entry.RecordedAt.Value.Date)
            .Select(group => new
            {
                group.Key,
                Average = group.Average(entry => entry.MoodScore.Value)
            });

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(result => result.Key)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = items
            .Select(item => new MoodDailyAverage(DateOnly.FromDateTime(item.Key), Math.Round((decimal)item.Average, 2)))
            .ToList();

        return new PagedResult<MoodDailyAverage>(mapped, page, pageSize, totalCount);
    }

    private static (DateTime Start, DateTime End) GetDateRange(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return (start, start.AddDays(1));
    }
}
