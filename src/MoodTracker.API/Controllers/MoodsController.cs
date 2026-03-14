using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoodTracker.Application.Features.Moods.CreateMoodEntry;
using MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;
using MoodTracker.Contracts.Common;
using MoodTracker.Contracts.Moods.Requests;
using MoodTracker.Contracts.Moods.Responses;

namespace MoodTracker.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MoodsController : ControllerBase
{
    private readonly ISender _sender;

    public MoodsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<CreateMoodEntryResponse>> Create(
        [FromBody] CreateMoodEntryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateMoodEntryCommand(request.UserId, request.MoodScore, request.RecordedAtUtc);
        var result = await _sender.Send(command, cancellationToken);

        var response = new CreateMoodEntryResponse
        {
            Id = result.Id,
            DailyAverage = result.DailyAverage,
            ComparisonMessage = result.ComparisonMessage,
            RecordedAtUtc = result.RecordedAtUtc
        };

        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        return CreatedAtAction(nameof(GetHistory), new { version, userId = request.UserId }, response);
    }

    [HttpGet("history")]
    public async Task<ActionResult<MoodHistoryResponse>> GetHistory(
        [FromQuery] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] HistoryAggregationMode mode = HistoryAggregationMode.Raw,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMoodHistoryQuery(userId, page, pageSize, mode);
        var result = await _sender.Send(query, cancellationToken);

        var response = MapHistoryResponse(result);
        return Ok(response);
    }

    private static MoodHistoryResponse MapHistoryResponse(MoodHistoryResult result)
    {
        var response = new MoodHistoryResponse
        {
            Mode = result.Mode.ToString()
        };

        if (result.RawEntries is not null)
        {
            response.RawEntries = new PagedResponse<MoodHistoryEntryResponse>
            {
                Items = result.RawEntries.Items
                    .Select(entry => new MoodHistoryEntryResponse
                    {
                        Id = entry.Id,
                        MoodScore = entry.MoodScore,
                        RecordedAtUtc = entry.RecordedAtUtc
                    })
                    .ToList(),
                Page = result.RawEntries.Page,
                PageSize = result.RawEntries.PageSize,
                TotalCount = result.RawEntries.TotalCount
            };
        }

        if (result.DailyAverages is not null)
        {
            response.DailyAverages = new PagedResponse<MoodDailyAverageResponse>
            {
                Items = result.DailyAverages.Items
                    .Select(item => new MoodDailyAverageResponse
                    {
                        Date = item.Date,
                        Average = item.Average
                    })
                    .ToList(),
                Page = result.DailyAverages.Page,
                PageSize = result.DailyAverages.PageSize,
                TotalCount = result.DailyAverages.TotalCount
            };
        }

        return response;
    }
}
