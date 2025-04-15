using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeleteDummy;

/// <summary>
/// Handler for the DeleteDummyCommand
/// </summary>
public class DeleteDummyCommandHandler : IRequestHandler<DeleteDummyCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteDummyCommandHandler> _logger;
    private readonly IPublisher _publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    /// <param name="publisher">The event publisher</param>
    public DeleteDummyCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteDummyCommandHandler> logger,
        IPublisher publisher)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    /// <summary>
    /// Handles the DeleteDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unit result indicating success or failure</returns>
    public async Task<Result<Unit>> Handle(DeleteDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting dummy entity with ID: {Id}", request.Id);

            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found for deletion", request.Id);
                return Result<Unit>.Failure($"Dummy entity with ID: {request.Id} not found");
            }

            await _unitOfWork.DummyRepository.DeleteAsync(dummy, cancellationToken);

            // Publish domain event after deletion
            await _publisher.Publish(new DummyDeletedEvent(request.Id), cancellationToken);

            _logger.LogInformation("Successfully deleted dummy entity with ID: {Id}", request.Id);

            return Result<Unit>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dummy entity with ID: {Id}", request.Id);
            return Result<Unit>.Failure($"Error deleting dummy entity: {ex.Message}");
        }
    }
}
