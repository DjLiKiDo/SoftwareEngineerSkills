using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.Abstractions.Persistence;
using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Infrastructure.Persistence.Extensions;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of the repository pattern with soft delete support using Entity Framework Core
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
internal class EfSoftDeleteRepository<TEntity> : EfRepository<TEntity>, ISoftDeleteRepository<TEntity> 
    where TEntity : BaseEntity, ISoftDelete
{
    /// <summary>
    /// Creates a new instance of the EfSoftDeleteRepository class
    /// </summary>
    /// <param name="context">The database context</param>
    public EfSoftDeleteRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public void SoftDelete(TEntity entity, string? deletedBy = null)
    {
        _context.SoftDelete(entity, deletedBy);
    }

    /// <inheritdoc />
    public void SoftDeleteRange(IEnumerable<TEntity> entities, string? deletedBy = null)
    {
        _context.SoftDeleteRange(entities, deletedBy);
    }

    /// <inheritdoc />
    public void Restore(TEntity entity)
    {
        _context.Restore(entity);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetSoftDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.OnlySoftDeleted().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetByIdAsync(Guid id, bool includeSoftDeleted, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .WithSoftDeleted(includeSoftDeleted)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    /// <inheritdoc />
    public override async Task<TEntity> GetByIdOrThrowAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // By default, only include non-soft-deleted entities
        var entity = await GetByIdAsync(id, false, cancellationToken);
        
        if (entity == null)
        {
            throw new Domain.Exceptions.EntityNotFoundException(id, typeof(TEntity));
        }
        
        return entity;
    }
    
    /// <inheritdoc />
    public async Task<TEntity> GetByIdOrThrowAsync(Guid id, bool includeSoftDeleted, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, includeSoftDeleted, cancellationToken);
        
        if (entity == null)
        {
            throw new Domain.Exceptions.EntityNotFoundException(id, typeof(TEntity));
        }
        
        return entity;
    }
    
    /// <inheritdoc />
    public override void Remove(TEntity entity)
    {
        // Override the Remove method to use soft delete instead
        SoftDelete(entity);
    }

    /// <inheritdoc />
    public override void RemoveRange(IEnumerable<TEntity> entities)
    {
        // Override the RemoveRange method to use soft delete instead
        SoftDeleteRange(entities);
    }
}
