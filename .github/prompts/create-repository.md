# Repository Pattern Implementation Prompt

**Metadata:**
```yaml
category: infrastructure
complexity: intermediate
applyTo:
  - "**/Infrastructure/**/*.cs"
  - "**/Repositories/**/*.cs"
instructions:
  - code-generation-standards.instructions.md
  - infrastructure-standards.instructions.md
  - performance-optimization.instructions.md
```

## Request

Generate a comprehensive repository implementation for the selected entity that follows the Repository pattern with Unit of Work, includes support for soft deletion, and optimizes for performance with proper Entity Framework Core patterns.

## Context

You are working with a .NET 9 enterprise application using Clean Architecture, Entity Framework Core 9, and advanced features like soft deletion, audit properties, and performance optimization. The repository should handle both simple CRUD operations and complex queries while maintaining separation of concerns.

## Requirements

### Repository Features
- Implement both `IRepository<T>` and `IReadRepository<T>` interfaces
- Support for soft deletion through `ISoftDeleteRepository<T>`
- Generic repository with entity-specific extensions
- Optimized query patterns with includes and projections
- Proper async/await implementation
- Support for specifications pattern for complex queries

### Performance Considerations
- Use `IQueryable<T>` for deferred execution
- Implement efficient includes and projections
- Support for pagination and filtering
- Query optimization with proper indexing guidance
- Bulk operations where appropriate

### Quality Requirements
- Proper error handling and logging
- Thread-safe operations
- Comprehensive unit tests
- XML documentation for public methods
- Integration with EF Core change tracking appropriately

## Example Output

```csharp
namespace SoftwareEngineerSkills.Infrastructure.Repositories;

public class TaskRepository : Repository<Task>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<Task?> GetByIdWithSkillRequirementsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .Include(t => t.SkillRequirements)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Task?> GetByIdWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .Include(t => t.SkillRequirements)
            .Include(t => t.AssignedDeveloper)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Task>> GetTasksByStatusAsync(
        TaskStatus status, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = Context.Tasks
            .Where(t => t.Status == status)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate);

        var totalCount = await query.CountAsync(cancellationToken);
        
        var tasks = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Task>(tasks, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<Task>> GetTasksByDeveloperAsync(
        Guid developerId, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .Where(t => t.AssignedDeveloperId == developerId)
            .Include(t => t.SkillRequirements)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Task>> GetUnassignedTasksWithRequiredSkillsAsync(
        IEnumerable<SkillCategory> developerSkillCategories,
        CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .Where(t => t.AssignedDeveloperId == null)
            .Where(t => t.Status == TaskStatus.Todo)
            .Include(t => t.SkillRequirements)
            .Where(t => t.SkillRequirements.All(sr => 
                developerSkillCategories.Contains(sr.Category)))
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTaskCountByStatusAsync(
        TaskStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .CountAsync(t => t.Status == status, cancellationToken);
    }

    public async Task<Dictionary<TaskStatus, int>> GetTaskCountsByStatusAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .GroupBy(t => t.Status)
            .ToDictionaryAsync(
                g => g.Key, 
                g => g.Count(), 
                cancellationToken);
    }

    public async Task<IEnumerable<Task>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        
        return await Context.Tasks
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date < today)
            .Where(t => t.Status != TaskStatus.Released && t.Status != TaskStatus.Cancelled)
            .Include(t => t.AssignedDeveloper)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task BulkUpdateStatusAsync(
        IEnumerable<Guid> taskIds, 
        TaskStatus newStatus, 
        CancellationToken cancellationToken = default)
    {
        await Context.Tasks
            .Where(t => taskIds.Contains(t.Id))
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(t => t.Status, newStatus),
                cancellationToken);
    }

    protected override IQueryable<Task> GetQueryWithIncludes()
    {
        return Context.Tasks
            .Include(t => t.SkillRequirements);
    }
}

// Base Repository Implementation
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly ILogger<Repository<T>> Logger;
    protected readonly DbSet<T> DbSet;

    public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        var query = DbSet.AsQueryable();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryWithIncludes().ToListAsync(cancellationToken);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = GetQueryWithIncludes();
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await GetQueryWithIncludes()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await GetQueryWithIncludes()
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, 
        CancellationToken cancellationToken = default)
    {
        return predicate == null 
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        await DbSet.AddAsync(entity, cancellationToken);
        Logger.LogDebug("Added entity {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));
        
        var entitiesList = entities.ToList();
        await DbSet.AddRangeAsync(entitiesList, cancellationToken);
        Logger.LogDebug("Added {Count} entities of type {EntityType}", entitiesList.Count, typeof(T).Name);
    }

    public virtual void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        DbSet.Update(entity);
        Logger.LogDebug("Updated entity {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));
        
        var entitiesList = entities.ToList();
        DbSet.UpdateRange(entitiesList);
        Logger.LogDebug("Updated {Count} entities of type {EntityType}", entitiesList.Count, typeof(T).Name);
    }

    public virtual void Remove(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        DbSet.Remove(entity);
        Logger.LogDebug("Removed entity {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));
        
        var entitiesList = entities.ToList();
        DbSet.RemoveRange(entitiesList);
        Logger.LogDebug("Removed {Count} entities of type {EntityType}", entitiesList.Count, typeof(T).Name);
    }

    public virtual async Task<IEnumerable<TResult>> ProjectToAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);
            
        return await query
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    protected virtual IQueryable<T> GetQueryWithIncludes()
    {
        return DbSet;
    }
}

// Soft Delete Repository
public class SoftDeleteRepository<T> : Repository<T>, ISoftDeleteRepository<T> 
    where T : SoftDeleteEntity
{
    public SoftDeleteRepository(ApplicationDbContext context, ILogger<Repository<T>> logger) 
        : base(context, logger)
    {
    }

    public async Task<IEnumerable<T>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>()
            .IgnoreQueryFilters()
            .Where(e => e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>()
            .IgnoreQueryFilters()
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public void SoftDelete(T entity, string deletedBy)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        entity.SoftDelete(deletedBy);
        Update(entity);
        Logger.LogDebug("Soft deleted entity {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
    }

    public async Task SoftDeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            SoftDelete(entity, deletedBy);
        }
    }

    public void Restore(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (!entity.IsDeleted) return;
        
        entity.Restore();
        Update(entity);
        Logger.LogDebug("Restored entity {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
    }

    public async Task RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdIncludingDeletedAsync(id, cancellationToken);
        if (entity != null && entity.IsDeleted)
        {
            Restore(entity);
        }
    }
}
```

## Checklist

- [ ] Repository implements proper interface inheritance
- [ ] Includes both sync and async methods where appropriate
- [ ] Supports Entity Framework Core optimizations (includes, projections)
- [ ] Implements proper error handling and logging
- [ ] Supports soft deletion patterns when applicable
- [ ] Includes pagination and filtering capabilities
- [ ] Uses proper cancellation token support
- [ ] Implements bulk operations for performance
- [ ] Follows repository pattern with Unit of Work
- [ ] Includes comprehensive documentation and examples
