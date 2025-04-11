using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Event raised when a dummy entity is deleted
/// </summary>
public class DummyDeletedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the deleted dummy entity
    /// </summary>
    public Guid DummyId { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Creates a new instance of the DummyDeletedEvent class
    /// </summary>
    /// <param name="dummyId">The ID of the deleted dummy entity</param>
    public DummyDeletedEvent(Guid dummyId)
    {
        DummyId = dummyId;
        OccurredOn = DateTime.UtcNow;
    }
}