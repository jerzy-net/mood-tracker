using MediatR;
using MoodTracker.Application.Abstractions;
using MoodTracker.Application.Exceptions;
using MoodTracker.Domain.Entities;
using MoodTracker.Domain.ValueObjects;

namespace MoodTracker.Application.Features.Moods.CreateMoodEntry;

public class CreateMoodEntryHandler : IRequestHandler<CreateMoodEntryCommand, CreateMoodEntryResult>
{
    private const string AboveAverage = "Above average";
    private const string BelowAverage = "Below average";
    private const string EqualToAverage = "Equal to average";

    private readonly IMoodEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateMoodEntryHandler(
        IMoodEntryRepository repository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<CreateMoodEntryResult> Handle(CreateMoodEntryCommand request, CancellationToken cancellationToken)
    {
        var recordedAt = RecordedAt.Create(request.RecordedAtUtc ?? _dateTimeProvider.UtcNow);
        var moodScore = MoodScore.Create(request.MoodScore);

        if (request.UserId.HasValue)
        {
            var exists = await _repository.ExistsAsync(request.UserId.Value, recordedAt.Value, cancellationToken);

            if (exists)
            {
                throw new DuplicateMoodEntryException(request.UserId.Value, recordedAt.Value);
            }
        }

        var entry = MoodEntry.Create(request.UserId, moodScore, recordedAt, _dateTimeProvider.UtcNow);

        await _repository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dailyAverage = await _repository.GetDailyAverageAsync(recordedAt.Date, cancellationToken) ?? moodScore.Value;
        var comparison = CompareScoreToAverage(moodScore.Value, dailyAverage);

        return new CreateMoodEntryResult(entry.Id, recordedAt.Value, dailyAverage, comparison);
    }

    private static string CompareScoreToAverage(int score, decimal average)
    {
        if (score > average)
        {
            return AboveAverage;
        }

        if (score < average)
        {
            return BelowAverage;
        }

        return EqualToAverage;
    }
}
