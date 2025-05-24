namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Interface for the Unit of Work pattern
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the skill repository
    /// </summary>
    ISkillRepository Skills { get; }
    
    /// <summary>
    /// Commits all changes made in a transaction to the database
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back all changes made in this transaction
    /// </summary>
    void Rollback();
    
    /// <summary>
    /// Begins a new transaction
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits the transaction
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
