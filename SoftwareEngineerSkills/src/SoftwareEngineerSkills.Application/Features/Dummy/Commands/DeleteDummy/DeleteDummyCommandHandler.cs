using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeleteDummy;

/// <summary>
/// Handler for the DeleteDummyCommand
/// </summary>
public class DeleteDummyCommandHandler : IRequestHandler<DeleteDummyCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    public DeleteDummyCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteDummyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DeleteDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the deletion was successful, otherwise false</returns>
    public async Task<Result<bool>> Handle(DeleteDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting dummy entity with ID: {Id}", request.Id);
            
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for deletion", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            await _unitOfWork.DummyRepository.DeleteAsync(dummy, cancellationToken);
            
            // Save changes and commit transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.LogInformation("Successfully deleted dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error deleting dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error deleting dummy entity: {ex.Message}");
        }
    }
}
