namespace SoftwareEngineerSkills.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity cannot be found.
/// Used when attempting to retrieve or operate on an entity that does not exist.
/// </summary>
public class EntityNotFoundException : DomainException
{
    /// <summary>
    /// Gets the ID of the entity that was not found.
    /// </summary>
    public Guid EntityId { get; }
    
    /// <summary>
    /// Gets the type of the entity that was not found.
    /// </summary>
    public Type EntityType { get; }
    
    /// <summary>
    /// Gets the name of the entity type that was not found.
    /// </summary>
    public string EntityTypeName { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
    /// with a specified entity ID and entity type.
    /// </summary>
    /// <param name="id">The ID of the entity that was not found.</param>
    /// <param name="entityType">The type of the entity that was not found.</param>
    public EntityNotFoundException(Guid id, Type entityType)
        : base($"Entity of type {entityType.Name} with ID {id} was not found")
    {
        EntityId = id;
        EntityType = entityType;
        EntityTypeName = entityType.Name;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
    /// with a specified entity ID and entity type name.
    /// </summary>
    /// <param name="id">The ID of the entity that was not found.</param>
    /// <param name="entityTypeName">The name of the type of the entity that was not found.</param>
    public EntityNotFoundException(Guid id, string entityTypeName)
        : base($"Entity of type {entityTypeName} with ID {id} was not found")
    {
        EntityId = id;
        EntityType = typeof(object); // Default if exact type not known
        EntityTypeName = entityTypeName;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
    /// with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public EntityNotFoundException(string message)
        : base(message)
    {
        EntityId = Guid.Empty;
        EntityType = typeof(object);
        EntityTypeName = "Unknown";
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class
    /// with a custom message and inner exception.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
        EntityId = Guid.Empty;
        EntityType = typeof(object);
        EntityTypeName = "Unknown";
    }
}
