using Microsoft.Extensions.Configuration;
using Serilog;

namespace MoodTracker.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static LoggerConfiguration ConfigureFromSettings(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        return loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    }
}
