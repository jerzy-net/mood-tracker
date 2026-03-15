using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MoodTracker.Application.Exceptions;

namespace MoodTracker.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (DuplicateMoodEntryException ex)
        {
            await HandleDuplicateMoodEntryAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnexpectedAsync(context, ex);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed"
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem);
    }

    private static async Task HandleDuplicateMoodEntryAsync(HttpContext context, DuplicateMoodEntryException exception)
    {
        var problem = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Conflict,
            Title = "Duplicate mood entry",
            Detail = exception.Message
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem);
    }

    private async Task HandleUnexpectedAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unexpected error occurred.");

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unexpected error",
            Detail = "An unexpected error occurred."
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem);
    }
}
