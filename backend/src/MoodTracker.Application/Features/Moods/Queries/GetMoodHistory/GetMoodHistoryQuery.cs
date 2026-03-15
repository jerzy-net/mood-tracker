using MediatR;

namespace MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;

public sealed record GetMoodHistoryQuery(Guid UserId, int Page, int PageSize, HistoryAggregationMode Mode)
    : IRequest<MoodHistoryResult>;
