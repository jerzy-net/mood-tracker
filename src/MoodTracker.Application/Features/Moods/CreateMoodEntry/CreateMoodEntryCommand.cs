using MediatR;

namespace MoodTracker.Application.Features.Moods.CreateMoodEntry;

public sealed record CreateMoodEntryCommand(Guid? UserId, int MoodScore, DateTime? RecordedAtUtc)
    : IRequest<CreateMoodEntryResult>;
