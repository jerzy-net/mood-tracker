using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodTracker.Application.Abstractions;
using MoodTracker.Infrastructure.Persistence;
using MoodTracker.Infrastructure.Persistence.Repositories;
using MoodTracker.Infrastructure.Services;

namespace MoodTracker.Infrastructure;

public static class DependencyInjection
{
    private const string CorsPolicyName = "CorsPolicy";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               configuration["Database:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services.AddDbContext<MoodTrackerDbContext>(options =>
            options.UseNpgsql(connectionString, builder =>
                builder.MigrationsAssembly(typeof(MoodTrackerDbContext).Assembly.FullName)));

        services.AddHealthChecks()
            .AddNpgSql(connectionString);

        services.AddScoped<IMoodEntryRepository, MoodEntryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        ConfigureCors(services, configuration);

        return services;
    }

    public static string GetCorsPolicyName() => CorsPolicyName;

    private static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(
                CorsPolicyName,
                policy =>
                {
                    var allowedOrigins = origins.Length > 0 ? origins : new[] { "http://localhost:5173" };
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }
}
