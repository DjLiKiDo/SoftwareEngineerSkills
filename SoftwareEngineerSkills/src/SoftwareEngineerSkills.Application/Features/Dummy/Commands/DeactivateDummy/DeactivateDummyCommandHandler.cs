using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeactivateDummy;

/// <summary>
/// Handler for the DeactivateDummyCommand
/// </summary>
public class DeactivateDummyCommandHandler : IRequestHandler<DeactivateDummyCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    public DeactivateDummyCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeactivateDummyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the DeactivateDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the deactivation was successful, otherwise false</returns>
    public async Task<Result<bool>> Handle(DeactivateDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deactivating dummy entity with ID: {Id}", request.Id);
            
            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for deactivation", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            if (!dummy.IsActive)
            {
                _logger.LogInformation("Dummy entity with ID: {Id} is already inactive", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Success(true);
            }
            
            dummy.Deactivate();
            await _unitOfWork.DummyRepository.UpdateAsync(dummy, cancellationToken);
            
            // Save changes and commit transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            _logger.LogInformation("Successfully deactivated dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error deactivating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error deactivating dummy entity: {ex.Message}");
        }
    }
}
