using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of the dummy repository
/// </summary>
public class DummyRepository : Repository<Dummy>, IDummyRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DummyRepository"/> class
    /// </summary>
    /// <param name="dbContext">The database context</param>
    public DummyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Dummy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(d => includeInactive || d.IsActive)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Dummy>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(d => d.IsActive && d.Priority == priority)
            .ToListAsync(cancellationToken);
    }
}