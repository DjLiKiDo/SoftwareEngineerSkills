using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SoftwareEngineerSkills.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that logs information about requests and responses
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResponse">The type of the response</typeparam>
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pipeline handler that logs information about the request and response
    /// </summary>
    /// <param name="request">The request instance</param>
    /// <param name="next">The delegate for the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the next handler in the pipeline</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();

        // Don't log sensitive data or large request objects
        // Instead, log key information and metadata
        _logger.LogInformation("[START] Request {RequestName} {RequestId}", requestName, requestId);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();

            // Log success with execution time
            _logger.LogInformation("[END] Request {RequestName} {RequestId} completed successfully in {ElapsedMilliseconds}ms",
                requestName, requestId, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log failure with exception details
            _logger.LogError(ex, "[ERROR] Request {RequestName} {RequestId} failed after {ElapsedMilliseconds}ms",
                requestName, requestId, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
