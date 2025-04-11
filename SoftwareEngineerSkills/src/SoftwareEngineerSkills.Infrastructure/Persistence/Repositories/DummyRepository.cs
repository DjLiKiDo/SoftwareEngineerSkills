using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// In-memory implementation of the IDummyRepository interface
/// </summary>
public class DummyRepository : BaseRepository<Dummy>, IDummyRepository
{
    /// <inheritdoc />
    public Task<IEnumerable<Dummy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        IEnumerable<Dummy> result = _entities;
        if (!includeInactive)
        {
            result = result.Where(d => d.IsActive);
        }
        return Task.FromResult(result);
    }
}
