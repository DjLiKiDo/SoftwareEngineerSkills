using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic base repository implementation for entities that inherit from BaseEntity
/// </summary>
/// <typeparam name="T">The entity type that inherits from BaseEntity</typeparam>
public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly List<T> _entities = [];

    /// <inheritdoc />
    public virtual Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_entities.AsEnumerable());
    }

    /// <inheritdoc />
    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.Add(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var index = _entities.FindIndex(e => e.Id == entity.Id);
        if (index >= 0)
        {
            _entities[index] = entity;
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        return Task.CompletedTask;
    }
}
