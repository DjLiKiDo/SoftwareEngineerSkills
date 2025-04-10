using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Repository interface for Dummy entities
/// </summary>
public interface IDummyRepository
{
    /// <summary>
    /// Gets all dummy entities
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all dummy entities</returns>
    Task<IEnumerable<Dummy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a dummy entity by its ID
    /// </summary>
    /// <param name="id">The ID of the dummy entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dummy entity if found, null otherwise</returns>
    Task<Dummy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new dummy entity
    /// </summary>
    /// <param name="dummy">The dummy entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(Dummy dummy, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing dummy entity
    /// </summary>
    /// <param name="dummy">The dummy entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateAsync(Dummy dummy, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a dummy entity
    /// </summary>
    /// <param name="dummy">The dummy entity to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteAsync(Dummy dummy, CancellationToken cancellationToken = default);
}