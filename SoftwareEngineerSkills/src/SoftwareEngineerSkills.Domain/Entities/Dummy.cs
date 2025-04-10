using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Domain.Entities;

/// <summary>
/// Represents a Dummy entity for demonstration purposes
/// </summary>
public class Dummy : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the dummy entity
    /// </summary>
    public string? Name { get; private set; }
    
    /// <summary>
    /// Gets or sets the description of the dummy entity
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// Gets or sets the priority level of the dummy entity (default: 0)
    /// </summary>
    public int Priority { get; private set; } = 0;

    /// <summary>
    /// Gets or sets whether the dummy entity is active (default: true)
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets or sets the creation date of the dummy entity
    /// </summary>
    public DateTime? CreatedAt { get; private set; }
    
    /// <summary>
    /// Gets or sets the last update date of the dummy entity
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Default constructor for EF Core
    /// </summary>
    public Dummy() 
    { 
        IsActive = true;
        Priority = 0;
    }
    
    /// <summary>
    /// Creates a new Dummy entity
    /// </summary>
    /// <param name="name">The name of the dummy entity (can be null)</param>
    /// <param name="description">The description of the dummy entity (can be null)</param>
    /// <param name="priority">The priority level of the dummy entity (default: 0)</param>
    /// <returns>A new Dummy entity</returns>
    public static Dummy Create(string? name, string? description, int priority = 0)
    {
        if (priority < 0 || priority > 5)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 0 and 5");
            
        return new Dummy
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Priority = priority,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Updates the dummy entity with new values
    /// </summary>
    /// <param name="name">The new name (can be null)</param>
    /// <param name="description">The new description (can be null)</param>
    /// <param name="priority">The new priority (default: 0)</param>
    public void Update(string? name, string? description, int priority = 0)
    {
        if (priority < 0 || priority > 5)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 0 and 5");
            
        Name = name;
        Description = description;
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Activates the dummy entity
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;
            
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Deactivates the dummy entity
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;
            
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}