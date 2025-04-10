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
    private readonly IDummyRepository _dummyRepository;
    private readonly ILogger<DeactivateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="dummyRepository">The dummy repository</param>
    /// <param name="logger">The logger</param>
    public DeactivateDummyCommandHandler(
        IDummyRepository dummyRepository,
        ILogger<DeactivateDummyCommandHandler> logger)
    {
        _dummyRepository = dummyRepository ?? throw new ArgumentNullException(nameof(dummyRepository));
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
            
            var dummy = await _dummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for deactivation", request.Id);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            if (!dummy.IsActive)
            {
                _logger.LogInformation("Dummy entity with ID: {Id} is already inactive", request.Id);
                return Result<bool>.Success(true);
            }
            
            dummy.Deactivate();
            await _dummyRepository.UpdateAsync(dummy, cancellationToken);
            
            _logger.LogInformation("Successfully deactivated dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error deactivating dummy entity: {ex.Message}");
        }
    }
}