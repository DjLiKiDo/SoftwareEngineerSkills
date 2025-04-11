using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.Abstractions.Persistence;

/// <summary>
/// Repository interface for Dummy entities
/// </summary>
public interface IDummyRepository : IRepository<Dummy>
{
    /// <summary>
    /// Gets all dummy entities
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all dummy entities</returns>
    Task<IEnumerable<Dummy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
}
