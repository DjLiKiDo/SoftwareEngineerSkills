using SoftwareEngineerSkills.Domain.Events;

namespace SoftwareEngineerSkills.Domain.Common;

/// <summary>
/// Base class for all domain entities providing common behavior and properties
/// </summary>
public abstract class Entity : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; protected init; } = Guid.NewGuid();
    
    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// ID of the user who created the entity
    /// </summary>
    public string? CreatedBy { get; protected init; }
    
    /// <summary>
    /// ID of the user who last modified the entity
    /// </summary>
    public string? LastModifiedBy { get; protected set; }
    
    /// <summary>
    /// Gets the collection of domain events that have been raised but not yet published
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    /// <summary>
    /// Adds a domain event to the entity's collection of domain events
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// Removes a domain event from the entity's collection of domain events
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    
    /// <summary>
    /// Clears all domain events from the entity's collection of domain events
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    /// <summary>
    /// Equality comparison based on the entity's identity
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
            
        if (ReferenceEquals(this, other))
            return true;
            
        if (GetType() != other.GetType())
            return false;
            
        return Id == other.Id;
    }
    
    /// <summary>
    /// GetHashCode implementation consistent with Equals
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;
            
        if (left is null || right is null)
            return false;
            
        return left.Equals(right);
    }
    
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
