using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.UpdateDummy;

/// <summary>
/// Handler for the UpdateDummyCommand
/// </summary>
public class UpdateDummyCommandHandler : IRequestHandler<UpdateDummyCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    public UpdateDummyCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateDummyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the UpdateDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the update was successful, otherwise false</returns>
    public async Task<Result<bool>> Handle(UpdateDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating dummy entity with ID: {Id}", request.Id);
            
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            dummy.Update(request.Name, request.Description, request.Priority);
            
            await _unitOfWork.DummyRepository.UpdateAsync(dummy, cancellationToken);
            
            // Save changes and commit transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.LogInformation("Successfully updated dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning(ex, "Invalid argument when updating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error updating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error updating dummy entity: {ex.Message}");
        }
    }
}
