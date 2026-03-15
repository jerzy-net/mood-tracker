using MediatR;

namespace MoodTracker.Application.Features.Health;

public sealed record HealthCheckQuery : IRequest<string>;
