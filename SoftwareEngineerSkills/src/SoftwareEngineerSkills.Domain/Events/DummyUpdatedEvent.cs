using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Event raised when a dummy entity is updated
/// </summary>
public record DummyUpdatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the updated dummy entity
    /// </summary>
    public Guid DummyId { get; }
    
    /// <summary>
    /// Gets the name of the updated dummy entity
    /// </summary>
    public string? Name { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Creates a new instance of the DummyUpdatedEvent class
    /// </summary>
    /// <param name="dummy">The updated dummy entity</param>
    public DummyUpdatedEvent(Dummy dummy)
    {
        DummyId = dummy.Id;
        Name = dummy.Name;
        OccurredOn = DateTime.UtcNow;
    }
}
