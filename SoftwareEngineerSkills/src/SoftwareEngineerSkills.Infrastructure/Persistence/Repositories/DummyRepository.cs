using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework Core implementation of the IDummyRepository interface
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
    public async Task<IEnumerable<Dummy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Dummy> query = _dbSet.AsNoTracking();
        if (!includeInactive)
        {
            query = query.Where(d => d.IsActive);
        }
        return await query.ToListAsync(cancellationToken);
    }
}