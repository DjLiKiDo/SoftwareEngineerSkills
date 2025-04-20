namespace SoftwareEngineerSkills.Application.Features.Dummy.DTOs;

/// <summary>
/// Data transfer object for a Dummy entity
/// </summary>
public record DummyDto
{
    /// <summary>
    /// Gets the unique identifier of the dummy entity
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Gets the name of the dummy entity
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets the description of the dummy entity
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Gets the priority level of the dummy entity
    /// </summary>
    public int Priority { get; init; }
    
    /// <summary>
    /// Gets whether the dummy entity is active
    /// </summary>
    public bool IsActive { get; init; }
    
    /// <summary>
    /// Gets a string representation of the entity status (Active/Inactive)
    /// </summary>
    public string Status { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the date and time when the dummy entity was created
    /// </summary>
    public DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Gets the date and time when the dummy entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Data transfer object for creating a Dummy entity
/// </summary>
public record CreateDummyDto
{
    /// <summary>
    /// Gets the name of the dummy entity
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets the description of the dummy entity
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Gets the priority level of the dummy entity
    /// </summary>
    public int Priority { get; init; } = 0;
}

/// <summary>
/// Data transfer object for updating a Dummy entity
/// </summary>
public record UpdateDummyDto
{
    /// <summary>
    /// Gets the unique identifier of the dummy entity to update
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Gets the name of the dummy entity
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// Gets the description of the dummy entity
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Gets the priority level of the dummy entity
    /// </summary>
    public int Priority { get; init; } = 0;
}
