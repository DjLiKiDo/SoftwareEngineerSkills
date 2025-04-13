using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.UpdateDummy;

/// <summary>
/// Handler for the UpdateDummyCommand
/// </summary>
public class UpdateDummyCommandHandler : IRequestHandler<UpdateDummyCommand, Result<Unit>>
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
    /// <returns>Unit result indicating success or failure</returns>
    public async Task<Result<Unit>> Handle(UpdateDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating dummy entity with ID: {Id}", request.Id);
            
            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found", request.Id);
                return Result<Unit>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            dummy.Update(request.Name, request.Description, request.Priority);
            
            await _unitOfWork.DummyRepository.UpdateAsync(dummy, cancellationToken);
            
            _logger.LogInformation("Successfully updated dummy entity with ID: {Id}", request.Id);
            
            return Result<Unit>.Success(Unit.Value);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when updating dummy entity with ID: {Id}", request.Id);
            return Result<Unit>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dummy entity with ID: {Id}", request.Id);
            return Result<Unit>.Failure($"Error updating dummy entity: {ex.Message}");
        }
    }
}
