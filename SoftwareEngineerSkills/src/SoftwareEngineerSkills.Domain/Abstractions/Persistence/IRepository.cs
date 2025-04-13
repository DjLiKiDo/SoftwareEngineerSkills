using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Generic repository interface for entities that inherit from BaseEntity
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
public interface IRepository<T> where T : Entity
{
    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all entities</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    /// <param name="id">The ID of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new entity
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
