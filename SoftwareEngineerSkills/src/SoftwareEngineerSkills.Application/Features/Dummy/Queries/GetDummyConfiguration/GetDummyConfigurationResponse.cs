namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyConfiguration;

/// <summary>
/// Response model for dummy configuration information
/// </summary>
public record GetDummyConfigurationResponse(
    string ApplicationName,
    string Environment,
    DummySettingsDto DummySettings
);

/// <summary>
/// Data transfer object for dummy settings
/// </summary>
public record DummySettingsDto(
    string Setting1,
    int Setting2,
    bool Setting3
);
