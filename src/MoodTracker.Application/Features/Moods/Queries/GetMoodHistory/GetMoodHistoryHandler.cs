using MediatR;
using MoodTracker.Application.Abstractions;

namespace MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;

public class GetMoodHistoryHandler : IRequestHandler<GetMoodHistoryQuery, MoodHistoryResult>
{
    private readonly IMoodEntryRepository _repository;

    public GetMoodHistoryHandler(IMoodEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<MoodHistoryResult> Handle(GetMoodHistoryQuery request, CancellationToken cancellationToken)
    {
        if (request.Mode == HistoryAggregationMode.Raw)
        {
            var result = await _repository.GetRawHistoryAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
            var items = result.Items
                .Select(entry => new MoodEntryItem(entry.Id, entry.MoodScore.Value, entry.RecordedAt.Value))
                .ToList();

            return new MoodHistoryResult(
                request.Mode,
                new PagedResult<MoodEntryItem>(items, result.Page, result.PageSize, result.TotalCount),
                null);
        }

        var averages = await _repository.GetDailyAveragesAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
        var averageItems = averages.Items.ToList();

        return new MoodHistoryResult(
            request.Mode,
            null,
            new PagedResult<MoodDailyAverage>(averageItems, averages.Page, averages.PageSize, averages.TotalCount));
    }
}
