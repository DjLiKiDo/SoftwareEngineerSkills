using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Generic repository interface for entities that support soft deletion
/// </summary>
/// <typeparam name="TEntity">The type of entity this repository works with</typeparam>
public interface ISoftDeleteRepository<TEntity> : IRepository<TEntity> 
    where TEntity : class, ISoftDelete
{
    /// <summary>
    /// Soft deletes an entity instead of removing it from the database
    /// </summary>
    /// <param name="entity">The entity to soft delete</param>
    /// <param name="deletedBy">The user who performed the delete operation</param>
    void SoftDelete(TEntity entity, string? deletedBy = null);
    
    /// <summary>
    /// Soft deletes multiple entities
    /// </summary>
    /// <param name="entities">The entities to soft delete</param>
    /// <param name="deletedBy">The user who performed the delete operation</param>
    void SoftDeleteRange(IEnumerable<TEntity> entities, string? deletedBy = null);
    
    /// <summary>
    /// Restores a soft deleted entity
    /// </summary>
    /// <param name="entity">The entity to restore</param>
    void Restore(TEntity entity);
    
    /// <summary>
    /// Retrieves soft deleted entities
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>An enumerable collection of soft deleted entities</returns>
    Task<IEnumerable<TEntity>> GetSoftDeletedAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an entity by its ID, optionally including soft deleted items
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve</param>
    /// <param name="includeSoftDeleted">Whether to include soft deleted entities in the search</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The entity if found, otherwise null</returns>
    Task<TEntity?> GetByIdAsync(Guid id, bool includeSoftDeleted, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an entity by its ID, optionally including soft deleted items, or throws an EntityNotFoundException if not found
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve</param>
    /// <param name="includeSoftDeleted">Whether to include soft deleted entities in the search</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The entity if found</returns>
    /// <exception cref="Exceptions.EntityNotFoundException">Thrown when the entity with the specified ID does not exist</exception>
    Task<TEntity> GetByIdOrThrowAsync(Guid id, bool includeSoftDeleted, CancellationToken cancellationToken = default);
}
