using Microsoft.EntityFrameworkCore;
using SoftwareEngineerSkills.Domain.Common.Interfaces;

namespace SoftwareEngineerSkills.Infrastructure.Persistence.Extensions;

/// <summary>
/// Extensions for working with EF Core entities and soft deletes
/// </summary>
public static class EfCoreExtensions
{
    /// <summary>
    /// Gets a queryable that can include soft deleted entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="dbSet">The DbSet to query</param>
    /// <param name="includeSoftDeleted">Whether to include soft deleted entities</param>
    /// <returns>A queryable that conditionally includes soft deleted entities</returns>
    public static IQueryable<TEntity> WithSoftDeleted<TEntity>(
        this DbSet<TEntity> dbSet, 
        bool includeSoftDeleted = false) 
        where TEntity : class, ISoftDelete
    {
        if (includeSoftDeleted)
        {
            return dbSet.IgnoreQueryFilters();
        }
        
        return dbSet;
    }
    
    /// <summary>
    /// Gets only the soft deleted entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="dbSet">The DbSet to query</param>
    /// <returns>A queryable that only includes soft deleted entities</returns>
    public static IQueryable<TEntity> OnlySoftDeleted<TEntity>(
        this DbSet<TEntity> dbSet) 
        where TEntity : class, ISoftDelete
    {
        return dbSet.IgnoreQueryFilters().Where(e => e.IsDeleted);
    }
    
    /// <summary>
    /// Soft deletes an entity
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="dbContext">The DbContext</param>
    /// <param name="entity">The entity to delete</param>
    /// <param name="deletedBy">The user who deleted the entity</param>
    public static void SoftDelete<TEntity>(
        this DbContext dbContext, 
        TEntity entity, 
        string? deletedBy = null) 
        where TEntity : class, ISoftDelete
    {
        if (entity == null) return;
        
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;
        
        dbContext.Update(entity);
    }
    
    /// <summary>
    /// Soft deletes multiple entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="dbContext">The DbContext</param>
    /// <param name="entities">The entities to delete</param>
    /// <param name="deletedBy">The user who deleted the entities</param>
    public static void SoftDeleteRange<TEntity>(
        this DbContext dbContext, 
        IEnumerable<TEntity> entities, 
        string? deletedBy = null) 
        where TEntity : class, ISoftDelete
    {
        if (entities == null) return;
        
        foreach (var entity in entities)
        {
            dbContext.SoftDelete(entity, deletedBy);
        }
    }
    
    /// <summary>
    /// Restores a soft deleted entity
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="dbContext">The DbContext</param>
    /// <param name="entity">The entity to restore</param>
    public static void Restore<TEntity>(
        this DbContext dbContext, 
        TEntity entity) 
        where TEntity : class, ISoftDelete
    {
        if (entity == null) return;
        
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.DeletedBy = null;
        
        dbContext.Update(entity);
    }
}
