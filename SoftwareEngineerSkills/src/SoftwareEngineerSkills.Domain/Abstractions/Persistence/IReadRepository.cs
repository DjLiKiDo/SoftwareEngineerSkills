using System.Linq.Expressions;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Generic repository interface for read-only operations
/// </summary>
/// <typeparam name="TEntity">The type of entity this repository works with</typeparam>
public interface IReadRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves an entity by its ID
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The entity if found, otherwise null</returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves an entity by its ID or throws an EntityNotFoundException if not found
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The entity if found</returns>
    /// <exception cref="Exceptions.EntityNotFoundException">Thrown when the entity with the specified ID does not exist</exception>
    Task<TEntity> GetByIdOrThrowAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all entities
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>An enumerable collection of entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Finds entities based on a predicate
    /// </summary>
    /// <param name="predicate">The predicate to filter entities</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>An enumerable collection of entities that match the predicate</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any entity matches the given predicate
    /// </summary>
    /// <param name="predicate">The predicate to test</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>True if any entity matches the predicate; otherwise, false</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts entities that match the given predicate
    /// </summary>
    /// <param name="predicate">The predicate to filter entities</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The count of matching entities</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
