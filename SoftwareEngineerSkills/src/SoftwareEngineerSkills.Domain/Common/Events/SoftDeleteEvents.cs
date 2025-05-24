namespace SoftwareEngineerSkills.Domain.Common.Events;

/// <summary>
/// Event raised when an entity is soft-deleted
/// </summary>
public class EntitySoftDeletedEvent : DomainEvent
{
    /// <summary>
    /// The ID of the entity that was deleted
    /// </summary>
    public Guid EntityId { get; }
    
    /// <summary>
    /// The type name of the entity that was deleted
    /// </summary>
    public string EntityType { get; }
    
    /// <summary>
    /// The user or system who deleted the entity
    /// </summary>
    public string DeletedBy { get; }
    
    /// <summary>
    /// Creates a new instance of the EntitySoftDeletedEvent class
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="entityType">The type name of the entity</param>
    /// <param name="deletedBy">The user or system who deleted the entity</param>
    public EntitySoftDeletedEvent(Guid entityId, string entityType, string deletedBy)
    {
        EntityId = entityId;
        EntityType = entityType;
        DeletedBy = deletedBy;
    }
}

/// <summary>
/// Event raised when a previously soft-deleted entity is restored
/// </summary>
public class EntityRestoredEvent : DomainEvent
{
    /// <summary>
    /// The ID of the entity that was restored
    /// </summary>
    public Guid EntityId { get; }
    
    /// <summary>
    /// The type name of the entity that was restored
    /// </summary>
    public string EntityType { get; }
    
    /// <summary>
    /// The user or system who previously deleted the entity
    /// </summary>
    public string PreviouslyDeletedBy { get; }
    
    /// <summary>
    /// Creates a new instance of the EntityRestoredEvent class
    /// </summary>
    /// <param name="entityId">The ID of the entity</param>
    /// <param name="entityType">The type name of the entity</param>
    /// <param name="previouslyDeletedBy">The user or system who previously deleted the entity</param>
    public EntityRestoredEvent(Guid entityId, string entityType, string previouslyDeletedBy)
    {
        EntityId = entityId;
        EntityType = entityType;
        PreviouslyDeletedBy = previouslyDeletedBy;
    }
}
