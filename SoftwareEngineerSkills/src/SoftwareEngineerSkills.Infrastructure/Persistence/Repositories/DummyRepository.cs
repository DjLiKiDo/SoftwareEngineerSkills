using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// In-memory implementation of the IDummyRepository interface
/// </summary>
public class DummyRepository : IDummyRepository
{
    private readonly List<Dummy> _dummies = new();

    /// <inheritdoc />
    public Task<IEnumerable<Dummy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        IEnumerable<Dummy> result = _dummies;
        if (!includeInactive)
        {
            result = result.Where(d => d.IsActive);
        }
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<Dummy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dummy = _dummies.FirstOrDefault(d => d.Id == id);
        return Task.FromResult(dummy);
    }

    /// <inheritdoc />
    public Task AddAsync(Dummy dummy, CancellationToken cancellationToken = default)
    {
        _dummies.Add(dummy);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateAsync(Dummy dummy, CancellationToken cancellationToken = default)
    {
        var index = _dummies.FindIndex(d => d.Id == dummy.Id);
        if (index >= 0)
        {
            _dummies[index] = dummy;
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Dummy dummy, CancellationToken cancellationToken = default)
    {
        _dummies.Remove(dummy);
        return Task.CompletedTask;
    }
}