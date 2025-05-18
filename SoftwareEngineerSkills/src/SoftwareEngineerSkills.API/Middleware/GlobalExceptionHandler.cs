using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations; // Required for ValidationException, assuming it's standard
// If ValidationException is custom, its namespace would be different.
// Assuming NotFoundException is a custom exception, its namespace would be needed.
// For e.g. using SoftwareEngineerSkills.Application.Exceptions;

namespace SoftwareEngineerSkills.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = CreateProblemDetails(httpContext, exception);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            // NotFoundException => StatusCodes.Status404NotFound, // Assuming NotFoundException is custom
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(exception),
            Detail = exception.Message,
            Instance = context.Request.Path
        };
        
        // Add specific error details for validation exceptions
        if (exception is ValidationException validationException)
        {
            // Assuming ValidationException has an 'Errors' property similar to FluentValidation's ValidationException
            // This might need adjustment based on the actual ValidationException structure.
            // problemDetails.Extensions["errors"] = validationException.Errors; 
        }

        return problemDetails;
    }

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            ValidationException => "Validation Error",
            // NotFoundException => "Resource Not Found",
            UnauthorizedAccessException => "Unauthorized Access",
            _ => "An unexpected error occurred"
        };
}
