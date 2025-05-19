namespace SoftwareEngineerSkills.Domain.Common.Events;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets a unique identifier for the event instance
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// Creates a new instance of a domain event
    /// </summary>
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Creates a new instance of a domain event with a specified occurrence time
    /// </summary>
    /// <param name="occurredOn">The date and time when the event occurred</param>
    protected DomainEvent(DateTime occurredOn)
    {
        Id = Guid.NewGuid();
        OccurredOn = occurredOn;
    }
}
