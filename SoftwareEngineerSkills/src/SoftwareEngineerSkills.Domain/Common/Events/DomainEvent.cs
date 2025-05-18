namespace SoftwareEngineerSkills.Domain.Common.Events;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets a unique identifier for the event instance
    /// </summary>
    public Guid EventId { get; }
    
    /// <summary>
    /// Creates a new instance of a domain event
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
