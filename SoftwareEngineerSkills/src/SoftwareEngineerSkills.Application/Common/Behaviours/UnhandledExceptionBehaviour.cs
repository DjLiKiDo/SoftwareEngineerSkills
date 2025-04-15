using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that handles unhandled exceptions and converts them to appropriate Result objects
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResponse">The type of the response</typeparam>
public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Logger</param>
    public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pipeline handler that catches and processes unhandled exceptions
    /// </summary>
    /// <param name="request">The request instance</param>
    /// <param name="next">The delegate for the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the next handler in the pipeline, or a failure Result if an exception occurs</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            // Log the exception
            _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);

            // Only attempt to convert to Result if the response type is Result<T>
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                // Determine the appropriate error message based on exception type
                string errorMessage = ex switch
                {
                    DomainException domainEx => $"Domain Error: {domainEx.Message}",
                    ArgumentException argEx => $"Argument Error: {argEx.Message}",
                    InvalidOperationException invOpEx => $"Operation Error: {invOpEx.Message}",
                    _ => $"An unexpected error occurred: {ex.Message}"
                };

                // Use reflection to create a failure Result with the error message
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var resultMethod = typeof(Result<>).MakeGenericType(resultType)
                    .GetMethod("Failure", new[] { typeof(string) });

                return resultMethod?.Invoke(null, new object[] { errorMessage }) as TResponse
                    ?? throw new InvalidOperationException("Could not create failure Result");
            }

            // For non-Result responses, re-throw the exception
            throw;
        }
    }
}
