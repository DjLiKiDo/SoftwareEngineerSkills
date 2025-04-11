using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Infrastructure.Persistence;

/// <summary>
/// Implementation of the Unit of Work pattern that coordinates the work of multiple repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private bool _isTransactionActive;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="dummyRepository">The dummy repository</param>
    public UnitOfWork(
        IServiceProvider serviceProvider,
        IDummyRepository dummyRepository)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        DummyRepository = dummyRepository ?? throw new ArgumentNullException(nameof(dummyRepository));
    }

    /// <inheritdoc />
    public IDummyRepository DummyRepository { get; }

    /// <inheritdoc />
    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        return _serviceProvider.GetRequiredService<TRepository>();
    }

    /// <inheritdoc />
    public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Since we're using an in-memory repository implementation,
        // there's no actual database context to save changes to.
        // In a real application with a database, this would call
        // _dbContext.SaveChangesAsync() or equivalent.

        // For now, we'll just return a completed task with value 1
        // to indicate success
        return Task.FromResult(1);
    }

    /// <inheritdoc />
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would start a database transaction
        // For this in-memory example, we'll just track the transaction state
        _isTransactionActive = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would commit the database transaction
        // For this in-memory example, we'll just reset the transaction state
        _isTransactionActive = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would roll back the database transaction
        // For this in-memory example, we'll just reset the transaction state
        _isTransactionActive = false;
        return Task.CompletedTask;
    }
}