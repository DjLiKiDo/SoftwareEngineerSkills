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
    private readonly IDummyRepository _dummyRepository;
    private readonly ILogger<DeleteDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="dummyRepository">The dummy repository</param>
    /// <param name="logger">The logger</param>
    public DeleteDummyCommandHandler(
        IDummyRepository dummyRepository,
        ILogger<DeleteDummyCommandHandler> logger)
    {
        _dummyRepository = dummyRepository ?? throw new ArgumentNullException(nameof(dummyRepository));
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
            
            var dummy = await _dummyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for deletion", request.Id);
                return Result<bool>.Failure($"Dummy entity with ID: {request.Id} not found");
            }
            
            await _dummyRepository.DeleteAsync(dummy, cancellationToken);
            
            _logger.LogInformation("Successfully deleted dummy entity with ID: {Id}", request.Id);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dummy entity with ID: {Id}", request.Id);
            return Result<bool>.Failure($"Error deleting dummy entity: {ex.Message}");
        }
    }
}