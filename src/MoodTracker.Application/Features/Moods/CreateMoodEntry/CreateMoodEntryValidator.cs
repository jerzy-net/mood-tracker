using FluentValidation;

namespace MoodTracker.Application.Features.Moods.CreateMoodEntry;

public sealed class CreateMoodEntryValidator : AbstractValidator<CreateMoodEntryCommand>
{
    public CreateMoodEntryValidator()
    {
        RuleFor(command => command.MoodScore)
            .InclusiveBetween(-5, 5);

        RuleFor(command => command.RecordedAtUtc)
            .Must(IsUtcOrUnspecified)
            .WithMessage("RecordedAtUtc must be in UTC or have an unspecified kind (treated as UTC).");

        RuleFor(command => command.UserId)
            .Must(id => !id.HasValue || id != Guid.Empty)
            .WithMessage("UserId cannot be empty when provided.");
    }

    private static bool IsUtcOrUnspecified(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return true;
        }

        return dateTime.Value.Kind is DateTimeKind.Utc or DateTimeKind.Unspecified;
    }
}
