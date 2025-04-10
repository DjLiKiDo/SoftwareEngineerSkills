namespace SoftwareEngineerSkills.Application.Features.Dummy.DTOs;

/// <summary>
/// Data transfer object for a Dummy entity
/// </summary>
public record DummyDto(
    Guid Id,
    string? Name,
    string? Description,
    int Priority,
    bool IsActive,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Data transfer object for creating a Dummy entity
/// </summary>
public record CreateDummyDto(
    string? Name,
    string? Description,
    int Priority = 0
);

/// <summary>
/// Data transfer object for updating a Dummy entity
/// </summary>
public record UpdateDummyDto(
    Guid Id,
    string? Name,
    string? Description,
    int Priority = 0
);