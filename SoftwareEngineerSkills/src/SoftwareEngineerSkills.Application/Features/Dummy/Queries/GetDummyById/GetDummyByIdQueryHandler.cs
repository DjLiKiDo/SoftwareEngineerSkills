using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyById;

/// <summary>
/// Handler for the GetDummyByIdQuery
/// </summary>
public class GetDummyByIdQueryHandler : IRequestHandler<GetDummyByIdQuery, Result<DummyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDummyByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDummyByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="logger">The logger</param>
    public GetDummyByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetDummyByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the GetDummyByIdQuery
    /// </summary>
    /// <param name="request">The query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A dummy DTO if found, otherwise a failure result</returns>
    public async Task<Result<DummyDto>> Handle(GetDummyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving dummy entity with ID: {Id}", request.Id);

            // For read operations, we don't need a transaction, but we use the UnitOfWork to get the repository
            var dummy = await _unitOfWork.DummyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (dummy == null)
            {
                _logger.LogWarning("Dummy entity with ID: {Id} not found", request.Id);
                return Result<DummyDto>.Failure($"Dummy entity with ID: {request.Id} not found");
            }

            var dummyDto = _mapper.Map<DummyDto>(dummy);

            _logger.LogInformation("Successfully retrieved dummy entity with ID: {Id}", request.Id);

            return Result<DummyDto>.Success(dummyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dummy entity with ID: {Id}", request.Id);
            return Result<DummyDto>.Failure($"Error retrieving dummy entity: {ex.Message}");
        }
    }
}
