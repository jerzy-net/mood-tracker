using FluentValidation;

namespace MoodTracker.Application.Features.Moods.Queries.GetMoodHistory;

public sealed class GetMoodHistoryValidator : AbstractValidator<GetMoodHistoryQuery>
{
    public GetMoodHistoryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEqual(Guid.Empty);

        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.Mode)
            .IsInEnum();
    }
}
