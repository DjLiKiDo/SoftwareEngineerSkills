using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.API.Middleware;

/// <summary>
/// Middleware for handling exceptions globally across the application
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    /// <param name="environment">The web host environment</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
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
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                detail = notFoundException.Message;
                break;
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                title = "Resource Not Found";
                detail = exception.Message;
                break;
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                title = "Bad Request";
                detail = exception.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                title = "Unauthorized";
                detail = exception.Message;
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
                    Instance = context.Request.Path,
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
                    problemDetails.Extensions["traceId"] = context.TraceIdentifier;
                }

                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)statusCode;

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(problemDetails, options);
                await context.Response.WriteAsync(json);
                return;
        }

        var standardProblemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Add debugging information when in development
        if (_environment.IsDevelopment())
        {
            standardProblemDetails.Extensions["traceId"] = context.TraceIdentifier;
            standardProblemDetails.Extensions["exception"] = new
            {
                message = exception.Message,
                source = exception.Source,
                stackTrace = exception.StackTrace
            };
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var standardOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var standardJson = JsonSerializer.Serialize(standardProblemDetails, standardOptions);
        await context.Response.WriteAsync(standardJson);
    }
}