using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Features.Dummy.DTOs;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;

/// <summary>
/// Handler for the GetAllDummiesQuery
/// </summary>
public class GetAllDummiesQueryHandler : IRequestHandler<GetAllDummiesQuery, Result<List<DummyDto>>>
{
    private readonly IDummyRepository _dummyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllDummiesQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllDummiesQueryHandler"/> class.
    /// </summary>
    /// <param name="dummyRepository">The dummy repository</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="logger">The logger</param>
    public GetAllDummiesQueryHandler(
        IDummyRepository dummyRepository,
        IMapper mapper,
        ILogger<GetAllDummiesQueryHandler> logger)
    {
        _dummyRepository = dummyRepository ?? throw new ArgumentNullException(nameof(dummyRepository));
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
            
            var dummies = await _dummyRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
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
