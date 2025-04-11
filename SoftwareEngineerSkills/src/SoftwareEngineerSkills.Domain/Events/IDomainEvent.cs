namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }
}
