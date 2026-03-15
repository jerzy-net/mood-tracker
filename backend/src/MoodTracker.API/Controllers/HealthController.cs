using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoodTracker.Application.Features.Health;
using MoodTracker.Contracts.Health;

namespace MoodTracker.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ISender _sender;

    public HealthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<HealthCheckResponse>> Get(CancellationToken cancellationToken)
    {
        var status = await _sender.Send(new HealthCheckQuery(), cancellationToken);
        return Ok(new HealthCheckResponse { Status = status });
    }
}
