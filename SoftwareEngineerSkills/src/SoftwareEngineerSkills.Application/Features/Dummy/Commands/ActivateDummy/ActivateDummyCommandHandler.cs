using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.ActivateDummy;

/// <summary>
/// Handler for the ActivateDummyCommand
/// </summary>
public class ActivateDummyCommandHandler : IRequestHandler<ActivateDummyCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    public ActivateDummyCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ActivateDummyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the ActivateDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unit result indicating success or failure</returns>
    public async Task<Result<Unit>> Handle(ActivateDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Activating dummy entity with ID: {Id}", request.Id);
            
            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for activation", request.Id);
                return Result<Unit>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            if (dummy.IsActive)
            {
                _logger.LogInformation("Dummy entity with ID: {Id} is already active", request.Id);
                return Result<Unit>.Success(Unit.Value);
            }
            
            dummy.Activate();
            await _unitOfWork.DummyRepository.UpdateAsync(dummy, cancellationToken);
            
            _logger.LogInformation("Successfully activated dummy entity with ID: {Id}", request.Id);
            
            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating dummy entity with ID: {Id}", request.Id);
            return Result<Unit>.Failure($"Error activating dummy entity: {ex.Message}");
        }
    }
}
