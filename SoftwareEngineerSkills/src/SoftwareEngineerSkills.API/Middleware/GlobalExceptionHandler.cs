using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoftwareEngineerSkills.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace SoftwareEngineerSkills.API.Middleware;

/// <summary>
/// Handles exceptions globally across the application using the modern IExceptionHandler approach
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="environment">The host environment</param>
    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Try to handle the exception
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <param name="exception">The exception that occurred</param>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the operation</param>
    /// <returns>A ValueTask representing the asynchronous operation, containing a boolean indicating if the exception was handled</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

        var statusCode = HttpStatusCode.InternalServerError;
        var title = "Server Error";
        var detail = "An internal server error has occurred.";

        switch (exception)
        {
            case DomainException domainException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Domain Error";
                detail = domainException.Message;
                break;
            case KeyNotFoundException keyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                detail = keyNotFoundException.Message;
                break;
            case ArgumentException argumentException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Bad Request";
                detail = argumentException.Message;
                break;
            case UnauthorizedAccessException unauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                title = "Unauthorized";
                detail = unauthorizedAccessException.Message;
                break;
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Validation Error";
                detail = "One or more validation errors occurred.";

                var problemDetails = new ValidationProblemDetails
                {
                    Status = (int)statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = httpContext.Request.Path,
                    Errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        )
                };

                // Add debugging information when in development
                if (_environment.IsDevelopment())
                {
                    problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
                }

                httpContext.Response.ContentType = "application/problem+json";
                httpContext.Response.StatusCode = (int)statusCode;

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(problemDetails, options);
                await httpContext.Response.WriteAsync(json, cancellationToken);
                return true;
        }

        var standardProblemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        // Add debugging information when in development
        if (_environment.IsDevelopment())
        {
            standardProblemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            standardProblemDetails.Extensions["exception"] = new
            {
                message = exception.Message,
                source = exception.Source,
                stackTrace = exception.StackTrace
            };
        }

        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = (int)statusCode;

        var standardOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var standardJson = JsonSerializer.Serialize(standardProblemDetails, standardOptions);
        await httpContext.Response.WriteAsync(standardJson, cancellationToken);
        return true;
    }
}
