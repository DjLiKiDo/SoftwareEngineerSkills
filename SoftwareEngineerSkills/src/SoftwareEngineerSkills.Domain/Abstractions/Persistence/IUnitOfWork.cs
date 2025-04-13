namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Unit of Work interface that coordinates the work of multiple repositories by ensuring
/// that all operations within the scope are completed successfully, or none of them are.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the repository for the specified entity type
    /// </summary>
    /// <typeparam name="TRepository">The type of repository to get</typeparam>
    /// <returns>The repository</returns>
    TRepository GetRepository<TRepository>() where TRepository : class;

    /// <summary>
    /// Gets a specific repository by type
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <returns>The repository for the entity type</returns>
    IRepository<TEntity> Repository<TEntity>() where TEntity : Common.Entity;

    /// <summary>
    /// Gets the dummy repository
    /// </summary>
    IDummyRepository DummyRepository { get; }
    
    /// <summary>
    /// Saves all changes made in this unit of work to the database
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begins a transaction on the current unit of work
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
