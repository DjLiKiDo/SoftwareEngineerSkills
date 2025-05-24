using SoftwareEngineerSkills.Domain.Abstractions.Persistence;

namespace SoftwareEngineerSkills.Infrastructure.Persistence;

/// <summary>
/// Implementation of the unit of work pattern
/// </summary>
internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ISkillRepository? _skillRepository;
    private bool _disposed = false;

    /// <summary>
    /// Creates a new instance of the UnitOfWork class
    /// </summary>
    /// <param name="context">The database context</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public ISkillRepository Skills => _skillRepository ??= new Repositories.SkillRepository(_context);

    /// <inheritdoc />
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Rollback()
    {
        _context.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.CommitTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _context.RollbackTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes the unit of work
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the unit of work
    /// </summary>
    /// <param name="disposing">Whether the method is being called from Dispose()</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}
