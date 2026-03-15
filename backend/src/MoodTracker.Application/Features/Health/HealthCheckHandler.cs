using MediatR;

namespace MoodTracker.Application.Features.Health;

public class HealthCheckHandler : IRequestHandler<HealthCheckQuery, string>
{
    public Task<string> Handle(HealthCheckQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Healthy");
    }
}
