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
    private readonly IDummyRepository _dummyRepository;
    private readonly ILogger<UpdateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="dummyRepository">The dummy repository</param>
    /// <param name="logger">The logger</param>
    public UpdateDummyCommandHandler(
        IDummyRepository dummyRepository,
        ILogger<UpdateDummyCommandHandler> logger)
    {
        _dummyRepository = dummyRepository ?? throw new ArgumentNullException(nameof(dummyRepository));
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
            
            var dummy = await _dummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found", request.Id);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            dummy.Update(request.Name, request.Description, request.Priority);
            
            await _dummyRepository.UpdateAsync(dummy, cancellationToken);
            
            _logger.LogInformation("Successfully updated dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when updating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error updating dummy entity: {ex.Message}");
        }
    }
}