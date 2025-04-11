using FluentValidation;
using MediatR;
using SoftwareEngineerSkills.Domain.Common.Models;
using System.Text;

namespace SoftwareEngineerSkills.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that validates incoming requests using FluentValidation
/// </summary>
/// <typeparam name="TRequest">The type of the request</typeparam>
/// <typeparam name="TResponse">The type of the response</typeparam>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">Collection of validators that apply to this request</param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Pipeline handler that validates the request before passing it to the next handler
    /// </summary>
    /// <param name="request">The request instance</param>
    /// <param name="next">The delegate for the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the next handler in the pipeline</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        
        // Run all validators and collect the results
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();
        
        if (failures.Any())
        {
            // Build an error message that includes all validation failures
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendLine("Validation failures:");

            foreach (var failure in failures)
            {
                errorBuilder.AppendLine($"- {failure.PropertyName}: {failure.ErrorMessage}");
            }

            // If the response is a Result type, return a failure Result
            if (typeof(TResponse).IsGenericType && 
                (typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>)))
            {
                // Use reflection to create a failure Result with the error message
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var resultMethod = typeof(Result<>).MakeGenericType(resultType)
                    .GetMethod("Failure", new[] { typeof(string) });
                
                return resultMethod?.Invoke(null, new object[] { errorBuilder.ToString() }) as TResponse 
                    ?? throw new InvalidOperationException("Could not create failure Result");
            }

            // For non-Result responses, throw a validation exception
            throw new ValidationException(failures);
        }

        // If validation passes, continue to the next handler
        return await next();
    }
}