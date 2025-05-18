# Entity Auditing in SoftwareEngineerSkills

This document outlines the entity auditing strategy implemented in the SoftwareEngineerSkills application.

## Overview

Entity auditing is critical for tracking changes to data over time, understanding who made changes, and when those changes occurred. Our auditing system includes:

1. **Basic auditing fields** for all entities (creation/modification)
2. **Soft delete functionality** for entities that shouldn't be permanently removed
3. **Integration with the authentication system** to record the actual user making changes

## Interfaces and Base Classes

### IAuditableEntity

All entities that require auditing implement the `IAuditableEntity` interface:

```csharp
public interface IAuditableEntity
{
    DateTime Created { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}
```

### ISoftDelete

Entities that support soft deletion implement the `ISoftDelete` interface:

```csharp
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
```

### BaseEntity

The `BaseEntity` class serves as the foundation for all domain entities and implements `IAuditableEntity`:

```csharp
public abstract class BaseEntity : IAuditableEntity
{
    public Guid Id { get; protected set; }
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Domain event functionality
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    // Methods for managing domain events...
}
```

### SoftDeleteEntity

The `SoftDeleteEntity` class extends `BaseEntity` and implements `ISoftDelete` for entities that require soft deletion:

```csharp
public abstract class SoftDeleteEntity : BaseEntity, ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    
    public void MarkAsDeleted(string? deletedBy)
    {
        // Implementation...
    }
    
    public void Restore()
    {
        // Implementation...
    }
}
```

## Automatic Auditing

The system automatically handles auditing through the `ApplicationDbContext`:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var utcNow = DateTime.UtcNow;
    var userName = _currentUserService.IsAuthenticated ? _currentUserService.UserName : "system";
    
    // Update audit properties
    foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entry.Entity.Created = utcNow;
                entry.Entity.CreatedBy = userName;
                break;
            case EntityState.Modified:
                entry.Entity.LastModified = utcNow;
                entry.Entity.LastModifiedBy = userName;
                break;
        }
    }
    
    // Handle soft delete
    foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
    {
        if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDeleteEntity)
        {
            entry.State = EntityState.Modified;
            softDeleteEntity.IsDeleted = true;
            softDeleteEntity.DeletedAt = utcNow;
            softDeleteEntity.DeletedBy = userName;
        }
    }
    
    // Save changes and handle domain events...
}
```

## Global Query Filters for Soft Delete

EF Core's global query filters are used to automatically filter out soft deleted entities:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ... other configurations
    
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var falseConstant = Expression.Constant(false);
            var expression = Expression.Equal(property, falseConstant);
            var lambda = Expression.Lambda(expression, parameter);
            
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
```

## Repositories for Soft Delete Entities

A specialized repository interface and implementation are provided for entities that support soft deletion:

```csharp
public interface ISoftDeleteRepository<TEntity> : IRepository<TEntity> 
    where TEntity : class, ISoftDelete
{
    void SoftDelete(TEntity entity, string? deletedBy = null);
    void SoftDeleteRange(IEnumerable<TEntity> entities, string? deletedBy = null);
    void Restore(TEntity entity);
    Task<IEnumerable<TEntity>> GetSoftDeletedAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(Guid id, bool includeSoftDeleted, CancellationToken cancellationToken = default);
}
```

## Best Practices

1. **Always use repositories** for data access to ensure audit trails are consistently maintained
2. **Use soft delete** for entities that need historical record retention
3. **Don't bypass the DbContext** to ensure auditing is consistently applied
4. **Consider adding database triggers** for additional auditing protection
5. **Use domain events** to react to significant changes in entity state

## Extension Points

1. **Audit tables**: Consider implementing separate audit tables for detailed change tracking
2. **Temporal tables**: For SQL Server, consider using temporal tables for point-in-time queries
3. **Version numbers**: Add concurrency tokens/version numbers for optimistic concurrency

## Conclusion

This robust auditing implementation provides:

- Complete tracking of entity creation, modification, and deletion
- Integration with the authentication system
- Soft deletion capability for entities that shouldn't be permanently removed
- Automatic data capture with minimal developer effort
