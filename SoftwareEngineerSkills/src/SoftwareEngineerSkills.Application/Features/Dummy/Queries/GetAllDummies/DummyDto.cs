namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;

/// <summary>
/// Data transfer object for the Dummy entity
/// </summary>
public record DummyDto
{
    /// <summary>
    /// Gets the ID of the dummy entity
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
    /// Gets the creation date of the dummy entity
    /// </summary>
    public DateTime? CreatedAt { get; init; }
    
    /// <summary>
    /// Gets the last update date of the dummy entity
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}