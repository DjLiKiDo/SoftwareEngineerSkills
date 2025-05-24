namespace SoftwareEngineerSkills.Domain.Common.Events;

/// <summary>
/// Interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The date and time when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// Gets a unique identifier for the event instance
    /// </summary>
    Guid Id { get; }
}
