namespace SoftwareEngineerSkills.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested resource cannot be found
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Creates a new instance of the NotFoundException class
    /// </summary>
    public NotFoundException() : base("The requested resource was not found.")
    {
    }
    
    /// <summary>
    /// Creates a new instance of the NotFoundException class with a specified error message
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    public NotFoundException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Creates a new instance of the NotFoundException class for an entity with the specified ID
    /// </summary>
    /// <param name="name">The name of the entity type</param>
    /// <param name="id">The ID of the entity that was not found</param>
    public NotFoundException(string name, object id)
        : base($"Entity \"{name}\" ({id}) was not found.")
    {
    }
}