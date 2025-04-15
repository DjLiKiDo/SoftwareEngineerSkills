using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.CreateDummy;

/// <summary>
/// Handler for the CreateDummyCommand
/// </summary>
public class CreateDummyCommandHandler : IRequestHandler<CreateDummyCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDummyCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDummyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="logger">The logger</param>
    public CreateDummyCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateDummyCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the CreateDummyCommand
    /// </summary>
    /// <param name="request">The command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the newly created dummy entity</returns>
    public async Task<Result<Guid>> Handle(CreateDummyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new dummy entity");

            var dummy = Domain.Entities.Dummy.Create(request.Name, request.Description, request.Priority);

            await _unitOfWork.DummyRepository.AddAsync(dummy, cancellationToken);

            _logger.LogInformation("Successfully created dummy entity with ID: {Id}", dummy.Id);

            return Result<Guid>.Success(dummy.Id);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error when creating dummy entity");
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dummy entity");
            return Result<Guid>.Failure($"Error creating dummy entity: {ex.Message}");
        }
    }
}
