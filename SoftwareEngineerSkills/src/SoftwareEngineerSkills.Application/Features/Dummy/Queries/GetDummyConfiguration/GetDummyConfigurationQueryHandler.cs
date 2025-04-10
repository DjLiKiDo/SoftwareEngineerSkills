using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Domain.Common.Models;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyConfiguration;

/// <summary>
/// Handler for retrieving dummy configuration settings
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GetDummyConfigurationQueryHandler"/> class
/// </remarks>
/// <param name="appSettingsService">The application settings service</param>
/// <param name="logger">The logger</param>
/// <param name="mapper">The AutoMapper instance</param>
public class GetDummyConfigurationQueryHandler(
    IAppSettingsService appSettingsService,
    IMapper mapper,
    ILogger<GetDummyConfigurationQueryHandler> logger) : IRequestHandler<GetDummyConfigurationQuery, Result<GetDummyConfigurationResponse>>
{
    private readonly IAppSettingsService _appSettingsService = appSettingsService ?? throw new ArgumentNullException(nameof(appSettingsService));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILogger<GetDummyConfigurationQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Handles the retrieval of dummy configuration settings
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query response wrapped in a Result</returns>
    public Task<Result<GetDummyConfigurationResponse>> Handle(
        GetDummyConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving dummy configuration information");
            
            var appSettings = _appSettingsService.CurrentSettings;
            
            // Use AutoMapper to map from DummySettings to DummySettingsDto
            var dummySettingsDto = _mapper.Map<DummySettingsDto>(appSettings.DummySettings);
            
            var response = new GetDummyConfigurationResponse(
                ApplicationName: appSettings.ApplicationName,
                Environment: appSettings.Environment.ToString(),
                DummySettings: dummySettingsDto
            );

            _logger.LogDebug("Retrieved configuration for application: {ApplicationName}, environment: {Environment}", 
                response.ApplicationName, response.Environment);
                
            return Task.FromResult(Result<GetDummyConfigurationResponse>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dummy configuration");
            return Task.FromResult(Result<GetDummyConfigurationResponse>.Failure("Error retrieving configuration"));
        }
    }
}
