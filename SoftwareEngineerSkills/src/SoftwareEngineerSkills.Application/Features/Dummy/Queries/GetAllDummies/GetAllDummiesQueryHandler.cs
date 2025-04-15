using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;
using SoftwareEngineerSkills.Common;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;

/// <summary>
/// Handler for the GetAllDummiesQuery
/// </summary>
public class GetAllDummiesQueryHandler : IRequestHandler<GetAllDummiesQuery, Result<List<DummyDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllDummiesQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllDummiesQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="logger">The logger</param>
    public GetAllDummiesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllDummiesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the GetAllDummiesQuery
    /// </summary>
    /// <param name="request">The query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of dummy DTOs</returns>
    public async Task<Result<List<DummyDto>>> Handle(GetAllDummiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving all dummy entities (includeInactive: {IncludeInactive})", request.IncludeInactive);

            // For read operations, we don't need a transaction, but we use the UnitOfWork to get the repository
            var dummies = await _unitOfWork.DummyRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
            var dummyDtos = _mapper.Map<List<DummyDto>>(dummies);

            _logger.LogInformation("Successfully retrieved {Count} dummy entities", dummyDtos.Count);

            return Result<List<DummyDto>>.Success(dummyDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dummy entities");
            return Result<List<DummyDto>>.Failure($"Error retrieving dummy entities: {ex.Message}");
        }
    }
}
