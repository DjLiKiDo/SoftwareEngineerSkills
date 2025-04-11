using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Event raised when a dummy entity is deactivated
/// </summary>
public class DummyDeactivatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the deactivated dummy entity
    /// </summary>
    public Guid DummyId { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Creates a new instance of the DummyDeactivatedEvent class
    /// </summary>
    /// <param name="dummy">The deactivated dummy entity</param>
    public DummyDeactivatedEvent(Dummy dummy)
    {
        DummyId = dummy.Id;
        OccurredOn = DateTime.UtcNow;
    }
}