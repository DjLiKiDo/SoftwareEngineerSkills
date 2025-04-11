using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Event raised when a dummy entity is activated
/// </summary>
public class DummyActivatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the activated dummy entity
    /// </summary>
    public Guid DummyId { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Creates a new instance of the DummyActivatedEvent class
    /// </summary>
    /// <param name="dummy">The activated dummy entity</param>
    public DummyActivatedEvent(Dummy dummy)
    {
        DummyId = dummy.Id;
        OccurredOn = DateTime.UtcNow;
    }
}