using SoftwareEngineerSkills.Domain.Aggregates;
using SoftwareEngineerSkills.Domain.Common;
using SoftwareEngineerSkills.Domain.Common.Models;
using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Domain.Entities;

/// <summary>
/// Represents a Dummy entity for demonstration purposes
/// </summary>
public class Dummy : Entity, IAggregateRoot
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
    /// Default constructor for EF Core
    /// </summary>
    protected Dummy() 
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
    /// <returns>A Result containing a new Dummy entity if successful, or an error message if validation fails</returns>
    public static Result<Dummy> Create(string? name, string? description, int priority = 0)
    {
        // Validate inputs
        if (priority < 0 || priority > 5)
            return Result<Dummy>.Failure("Priority must be between 0 and 5");
            
        var dummy = new Dummy
        {
            Name = name,
            Description = description,
            Priority = priority,
            IsActive = true
        };
        
        // Add domain event
        dummy.AddDomainEvent(new DummyCreatedEvent(dummy));
        
        return Result<Dummy>.Success(dummy);
    }
    
    /// <summary>
    /// Creates a new Dummy entity with user context
    /// </summary>
    /// <param name="name">The name of the dummy entity (can be null)</param>
    /// <param name="description">The description of the dummy entity (can be null)</param>
    /// <param name="priority">The priority level of the dummy entity (default: 0)</param>
    /// <param name="userId">The ID of the user creating the entity</param>
    /// <returns>A Result containing a new Dummy entity if successful, or an error message if validation fails</returns>
    public static Result<Dummy> Create(string? name, string? description, string userId, int priority = 0)
    {
        // Validate inputs
        if (priority < 0 || priority > 5)
            return Result<Dummy>.Failure("Priority must be between 0 and 5");
            
        var dummy = new Dummy
        {
            Name = name,
            Description = description,
            Priority = priority,
            IsActive = true
        };
        
        // Add domain event
        dummy.AddDomainEvent(new DummyCreatedEvent(dummy));
        
        return Result<Dummy>.Success(dummy);
    }
    
    /// <summary>
    /// Updates the dummy entity with new values
    /// </summary>
    /// <param name="name">The new name (can be null)</param>
    /// <param name="description">The new description (can be null)</param>
    /// <param name="priority">The new priority (default: 0)</param>
    /// <returns>A Result indicating success or containing an error message if validation fails</returns>
    public Result Update(string? name, string? description, int priority = 0)
    {
        // Validate inputs
        if (priority < 0 || priority > 5)
            return Result.Failure("Priority must be between 0 and 5");
            
        Name = name;
        Description = description;
        Priority = priority;
        
        // Add domain event
        AddDomainEvent(new DummyUpdatedEvent(this));
        
        return Result.Success();
    }
    
    /// <summary>
    /// Updates the dummy entity with new values and user context
    /// </summary>
    /// <param name="name">The new name (can be null)</param>
    /// <param name="description">The new description (can be null)</param>
    /// <param name="priority">The new priority (default: 0)</param>
    /// <param name="userId">The ID of the user updating the entity</param>
    /// <returns>A Result indicating success or containing an error message if validation fails</returns>
    public Result Update(string? name, string? description, string userId, int priority = 0)
    {
        // Validate inputs
        if (priority < 0 || priority > 5)
            return Result.Failure("Priority must be between 0 and 5");
            
        Name = name;
        Description = description;
        Priority = priority;
        
        // Add domain event
        AddDomainEvent(new DummyUpdatedEvent(this));
        
        return Result.Success();
    }
    
    /// <summary>
    /// Activates the dummy entity
    /// </summary>
    /// <returns>A Result indicating success or failure</returns>
    public Result Activate()
    {
        if (IsActive)
            return Result.Success(); // Already active, no change needed
            
        IsActive = true;
        
        // Add domain event
        AddDomainEvent(new DummyActivatedEvent(this));
        
        return Result.Success();
    }
    
    /// <summary>
    /// Deactivates the dummy entity
    /// </summary>
    /// <returns>A Result indicating success or failure</returns>
    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Success(); // Already inactive, no change needed
            
        IsActive = false;
        
        // Add domain event
        AddDomainEvent(new DummyDeactivatedEvent(this));
        
        return Result.Success();
    }
}
