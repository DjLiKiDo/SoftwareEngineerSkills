using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.ActivateDummy;

/// <summary>
/// Handler for the ActivateDummyCommand
/// </summary>
public class ActivateDummyCommandHandler : IRequestHandler<ActivateDummyCommand, Result<bool>>
{
    private readonly IDummyRepository _dummyRepository;
    private readonly ILogger<ActivateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="dummyRepository">The dummy repository</param>
    /// <param name="logger">The logger</param>
    public ActivateDummyCommandHandler(
        IDummyRepository dummyRepository,
        ILogger<ActivateDummyCommandHandler> logger)
    {
        _dummyRepository = dummyRepository ?? throw new ArgumentNullException(nameof(dummyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the ActivateDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the activation was successful, otherwise false</returns>
    public async Task<Result<bool>> Handle(ActivateDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Activating dummy entity with ID: {Id}", request.Id);
            
            var dummy = await _dummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for activation", request.Id);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            if (dummy.IsActive)
            {
                _logger.LogInformation("Dummy entity with ID: {Id} is already active", request.Id);
                return Result<bool>.Success(true);
            }
            
            dummy.Activate();
            await _dummyRepository.UpdateAsync(dummy, cancellationToken);
            
            _logger.LogInformation("Successfully activated dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error activating dummy entity: {ex.Message}");
        }
    }
}
