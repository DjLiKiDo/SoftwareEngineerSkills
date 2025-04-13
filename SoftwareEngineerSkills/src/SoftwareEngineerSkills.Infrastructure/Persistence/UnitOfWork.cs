using Microsoft.Extensions.DependencyInjection;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Infrastructure.Persistence;

/// <summary>
/// Implementation of the Unit of Work pattern that coordinates the work of multiple repositories
/// using Entity Framework Core and an in-memory database
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _dbContext;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="dbContext">The database context</param>
    /// <param name="dummyRepository">The dummy repository</param>
    public UnitOfWork(
        IServiceProvider serviceProvider,
        ApplicationDbContext dbContext,
        IDummyRepository dummyRepository)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
    public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
    {
        return _serviceProvider.GetRequiredService<IRepository<TEntity>>();
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.CommitTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.RollbackTransactionAsync(cancellationToken);
    }
}
