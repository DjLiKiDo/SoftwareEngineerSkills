using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Common.Commands;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that automates transaction management for commands.
/// This behavior starts a transaction before executing a command and commits or rolls it back
/// based on the operation result.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class UnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="unitOfWork">The Unit of Work</param>
    /// <param name="logger">The logger</param>
    public UnitOfWorkBehaviour(
        IUnitOfWork unitOfWork,
        ILogger<UnitOfWorkBehaviour<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the request, wrapping the handler execution in a transaction.
    /// </summary>
    /// <param name="request">The request instance</param>
    /// <param name="next">The delegate for the next action in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The handler response</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType().Name;
        
        try
        {
            _logger.LogInformation("Starting transaction for command {CommandType}", requestType);
            
            // Start the transaction before executing the handler
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            // Execute the handler
            var response = await next();
            
            // Check if the operation was successful
            bool isSuccess = true;
            
            // Verify if the response is a Result and if it was successful
            if (response is IResult result)
            {
                isSuccess = result.IsSuccess;
            }
            
            if (isSuccess)
            {
                _logger.LogInformation("Committing transaction for command {CommandType}", requestType);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            else
            {
                _logger.LogWarning("Rolling back transaction for command {CommandType} due to a logical failure", requestType);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command {CommandType}. Rolling back transaction", requestType);
            
            // In case of exception, rollback the transaction
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            // Re-throw the exception with additional contextual information
            throw new InvalidOperationException($"An error occurred while executing the command of type {requestType}. See inner exception for details.", ex);
        }
    }
}