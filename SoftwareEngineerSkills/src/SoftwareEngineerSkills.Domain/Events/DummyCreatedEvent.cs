using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Events;

/// <summary>
/// Event raised when a new dummy entity is created
/// </summary>
public record DummyCreatedEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the created dummy entity
    /// </summary>
    public Guid DummyId { get; }
    
    /// <summary>
    /// Gets the name of the created dummy entity
    /// </summary>
    public string? Name { get; }
    
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Creates a new instance of the DummyCreatedEvent class
    /// </summary>
    /// <param name="dummy">The created dummy entity</param>
    public DummyCreatedEvent(Dummy dummy)
    {
        DummyId = dummy.Id;
        Name = dummy.Name;
        OccurredOn = DateTime.UtcNow;
    }
}
