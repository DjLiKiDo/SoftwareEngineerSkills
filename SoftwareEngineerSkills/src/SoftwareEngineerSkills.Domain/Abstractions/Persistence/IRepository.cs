using System.Linq.Expressions;
using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Generic repository interface for entities that inherit from Entity
/// </summary>
/// <typeparam name="T">The entity type that inherits from Entity</typeparam>
public interface IRepository<T> where T : Entity
{
    /// <summary>
    /// Gets all entities asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An immutable list of all entities</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets entities by specified criteria asynchronously
    /// </summary>
    /// <param name="predicate">Filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An immutable list of entities matching the criteria</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets an entity by its ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first entity matching the criteria asynchronously
    /// </summary>
    /// <param name="predicate">The filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first entity matching the criteria, or null if none found</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single entity matching the criteria asynchronously
    /// </summary>
    /// <param name="predicate">The filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single entity matching the criteria, or null if none found</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one entity matches the criteria</exception>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new entity asynchronously
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates multiple entities asynchronously
    /// </summary>
    /// <param name="entities">The entities to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
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
    
    /// <summary>
    /// Deletes multiple entities
    /// </summary>
    /// <param name="entities">The entities to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any entity matches the specified criteria asynchronously
    /// </summary>
    /// <param name="predicate">Filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any entity matches the criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the total number of entities asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total count of entities</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts the number of entities matching the criteria asynchronously
    /// </summary>
    /// <param name="predicate">The filter expression</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of entities matching the criteria</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
