using Asp.Versioning.ApiExplorer;
using MoodTracker.API.Extensions;
using MoodTracker.API.Middleware;
using MoodTracker.Application;
using MoodTracker.Infrastructure;
using MoodTracker.Infrastructure.Logging;
using Serilog;
using InfrastructureServices = MoodTracker.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ConfigureFromSettings(context.Configuration));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiLayer();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(InfrastructureServices.GetCorsPolicyName());

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
