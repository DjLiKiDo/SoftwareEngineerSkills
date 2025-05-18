namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Generic repository interface for data access operations
/// </summary>
/// <typeparam name="TEntity">The type of entity this repository works with</typeparam>
public interface IRepository<TEntity> : IReadRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds multiple entities
    /// </summary>
    /// <param name="entities">The entities to add</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    void Update(TEntity entity);
    
    /// <summary>
    /// Removes an entity
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    void Remove(TEntity entity);
    
    /// <summary>
    /// Removes multiple entities
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    void RemoveRange(IEnumerable<TEntity> entities);
}
