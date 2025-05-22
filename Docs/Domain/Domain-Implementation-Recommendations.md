# Domain Layer Implementation Recommendations

This document provides specific implementation recommendations to improve your domain layer based on the analysis performed.

## 1. Enhance Domain Event System

### Standardize Domain Event Base Class

```csharp
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public Guid Id { get; }
    
    /// <summary>
    /// When the event occurred
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// The version of the aggregate when this event was created
    /// </summary>
    public int AggregateVersion { get; }
    
    /// <summary>
    /// Initializes a new instance of the domain event
    /// </summary>
    /// <param name="aggregateVersion">The version of the aggregate when this event was created</param>
    protected DomainEvent(int aggregateVersion = 1)
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        AggregateVersion = aggregateVersion;
    }
}
```

### Implement Standardized Entity Update Events

```csharp
/// <summary>
/// Base class for events that indicate a property has been changed
/// </summary>
/// <typeparam name="T">Type of the property that changed</typeparam>
public abstract class PropertyChangedEvent<T> : DomainEvent
{
    /// <summary>
    /// The ID of the entity that was updated
    /// </summary>
    public Guid EntityId { get; }
    
    /// <summary>
    /// The name of the property that was changed
    /// </summary>
    public string PropertyName { get; }
    
    /// <summary>
    /// The previous value
    /// </summary>
    public T? OldValue { get; }
    
    /// <summary>
    /// The new value
    /// </summary>
    public T NewValue { get; }
    
    /// <summary>
    /// Initializes a new instance of the property changed event
    /// </summary>
    protected PropertyChangedEvent(Guid entityId, string propertyName, T? oldValue, T newValue, int version)
        : base(version)
    {
        EntityId = entityId;
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

/// <summary>
/// Event raised when a skill's name is updated
/// </summary>
public class SkillNameChangedEvent : PropertyChangedEvent<string>
{
    public SkillNameChangedEvent(Guid skillId, string oldName, string newName, int version)
        : base(skillId, nameof(Skill.Name), oldName, newName, version)
    {
    }
}
```

## 2. Implement Improved Value Objects

### Address Value Object with Validation

```csharp
public class Address : ValueObject
{
    public string Street { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    
    // Required for EF Core
    private Address() { }
    
    public Address(string street, string city, string state, string postalCode, string country)
    {
        // Validation with helpful error messages
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
            
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
            
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be empty", nameof(state));
            
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));
            
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));
        
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
    
    /// <summary>
    /// Creates a new address with an updated street
    /// </summary>
    public Address WithStreet(string street)
    {
        return new Address(street, City, State, PostalCode, Country);
    }
    
    /// <summary>
    /// Creates a new address with an updated city
    /// </summary>
    public Address WithCity(string city)
    {
        return new Address(Street, city, State, PostalCode, Country);
    }
    
    // Add similar methods for other properties
    
    /// <summary>
    /// Returns a formatted single-line address
    /// </summary>
    public string GetFormattedAddress()
    {
        return $"{Street}, {City}, {State} {PostalCode}, {Country}";
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }
}
```

## 3. Enhanced AggregateRoot with Apply Pattern

```csharp
public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    private readonly object _domainEventLock = new();
    private readonly List<IDomainEvent> _domainEvents = new();
    
    /// <summary>
    /// Gets the domain events raised by this aggregate root
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    /// <summary>
    /// Adds a domain event to this aggregate root
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            _domainEvents.Add(domainEvent);
        }
    }
    
    /// <summary>
    /// Removes a domain event from this aggregate root
    /// </summary>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            _domainEvents.Remove(domainEvent);
        }
    }
    
    /// <summary>
    /// Clears all domain events from this aggregate root
    /// </summary>
    public void ClearDomainEvents()
    {
        lock (_domainEventLock)
        {
            _domainEvents.Clear();
        }
    }
    
    /// <summary>
    /// Applies a domain event to update the aggregate state
    /// </summary>
    protected virtual void ApplyDomainEvent(IDomainEvent @event)
    {
        // Override in derived classes to handle specific events
    }
    
    /// <summary>
    /// Increments the version of this aggregate root
    /// </summary>
    protected void IncrementVersion()
    {
        Version++;
    }
}
```

## 4. Consistent Skill Aggregate Implementation

```csharp
public class Skill : AggregateRoot
{
    public string Name { get; private set; }
    public SkillCategory Category { get; private set; }
    public string Description { get; private set; }
    public SkillLevel DifficultyLevel { get; private set; }
    public bool IsActive { get; private set; }
    
    // For EF Core
    private Skill() { }
    
    /// <summary>
    /// Creates a new skill
    /// </summary>
    public Skill(string name, SkillCategory category, string description, SkillLevel difficultyLevel)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Skill description cannot be empty", nameof(description));
            
        Name = name;
        Category = category;
        Description = description;
        DifficultyLevel = difficultyLevel;
        IsActive = true;
        
        AddDomainEvent(new SkillCreatedEvent(
            Id,
            Name,
            Category,
            Description,
            DifficultyLevel,
            Version
        ));
    }
    
    /// <summary>
    /// Updates the name of the skill
    /// </summary>
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Skill name cannot be empty", nameof(newName));
            
        if (Name == newName)
            return;
            
        var oldName = Name;
        Name = newName;
        
        var @event = new SkillNameChangedEvent(Id, oldName, newName, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    /// <summary>
    /// Updates the difficulty level of the skill
    /// </summary>
    public void UpdateDifficultyLevel(SkillLevel newLevel)
    {
        if (DifficultyLevel == newLevel)
            return;
            
        var oldLevel = DifficultyLevel;
        DifficultyLevel = newLevel;
        
        var @event = new SkillDifficultyChangedEvent(Id, oldLevel, newLevel, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    /// <summary>
    /// Updates the category of the skill
    /// </summary>
    public void UpdateCategory(SkillCategory newCategory)
    {
        if (Category == newCategory)
            return;
            
        var oldCategory = Category;
        Category = newCategory;
        
        var @event = new SkillCategoryChangedEvent(Id, oldCategory, newCategory, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    /// <summary>
    /// Deactivates the skill
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;
            
        IsActive = false;
        
        var @event = new SkillDeactivatedEvent(Id, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    /// <summary>
    /// Activates the skill
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;
            
        IsActive = true;
        
        var @event = new SkillActivatedEvent(Id, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    /// <summary>
    /// Applies domain events to update the aggregate state
    /// </summary>
    protected override void ApplyDomainEvent(IDomainEvent @event)
    {
        switch (@event)
        {
            case SkillNameChangedEvent e:
                Apply(e);
                break;
            case SkillDifficultyChangedEvent e:
                Apply(e);
                break;
            case SkillCategoryChangedEvent e:
                Apply(e);
                break;
            case SkillActivatedEvent e:
                Apply(e);
                break;
            case SkillDeactivatedEvent e:
                Apply(e);
                break;
        }
    }
    
    private void Apply(SkillNameChangedEvent @event)
    {
        // Additional logic if needed
        IncrementVersion();
    }
    
    private void Apply(SkillDifficultyChangedEvent @event)
    {
        // Additional logic if needed
        IncrementVersion();
    }
    
    private void Apply(SkillCategoryChangedEvent @event)
    {
        // Additional logic if needed
        IncrementVersion();
    }
    
    private void Apply(SkillActivatedEvent @event)
    {
        // Additional logic if needed
        IncrementVersion();
    }
    
    private void Apply(SkillDeactivatedEvent @event)
    {
        // Additional logic if needed
        IncrementVersion();
    }
    
    /// <summary>
    /// Validates that the entity satisfies all invariants
    /// </summary>
    protected override IEnumerable<string> CheckInvariants()
    {
        // Chain with base class invariants
        foreach (var error in base.CheckInvariants())
        {
            yield return error;
        }
        
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return "Skill name cannot be empty";
        }
        
        if (string.IsNullOrWhiteSpace(Description))
        {
            yield return "Skill description cannot be empty";
        }
        
        if (Description?.Length > 1000)
        {
            yield return "Skill description cannot exceed 1000 characters";
        }
    }
}
```

## 5. Improved SkillEvents Implementation

```csharp
// Base event for Skill aggregate events
public abstract class SkillEvent : DomainEvent
{
    public Guid SkillId { get; }
    
    protected SkillEvent(Guid skillId, int version) : base(version)
    {
        SkillId = skillId;
    }
}

public class SkillCreatedEvent : SkillEvent
{
    public string Name { get; }
    public SkillCategory Category { get; }
    public string Description { get; }
    public SkillLevel DifficultyLevel { get; }
    
    public SkillCreatedEvent(
        Guid skillId,
        string name,
        SkillCategory category,
        string description,
        SkillLevel difficultyLevel,
        int version)
        : base(skillId, version)
    {
        Name = name;
        Category = category;
        Description = description;
        DifficultyLevel = difficultyLevel;
    }
}

public class SkillNameChangedEvent : SkillEvent
{
    public string OldName { get; }
    public string NewName { get; }
    
    public SkillNameChangedEvent(Guid skillId, string oldName, string newName, int version)
        : base(skillId, version)
    {
        OldName = oldName;
        NewName = newName;
    }
}

public class SkillDifficultyChangedEvent : SkillEvent
{
    public SkillLevel OldLevel { get; }
    public SkillLevel NewLevel { get; }
    
    public SkillDifficultyChangedEvent(Guid skillId, SkillLevel oldLevel, SkillLevel newLevel, int version)
        : base(skillId, version)
    {
        OldLevel = oldLevel;
        NewLevel = newLevel;
    }
}

public class SkillCategoryChangedEvent : SkillEvent
{
    public SkillCategory OldCategory { get; }
    public SkillCategory NewCategory { get; }
    
    public SkillCategoryChangedEvent(Guid skillId, SkillCategory oldCategory, SkillCategory newCategory, int version)
        : base(skillId, version)
    {
        OldCategory = oldCategory;
        NewCategory = newCategory;
    }
}

public class SkillActivatedEvent : SkillEvent
{
    public SkillActivatedEvent(Guid skillId, int version) 
        : base(skillId, version)
    {
    }
}

public class SkillDeactivatedEvent : SkillEvent
{
    public SkillDeactivatedEvent(Guid skillId, int version) 
        : base(skillId, version)
    {
    }
}
```

## 6. Enhanced Soft Delete Implementation

```csharp
/// <summary>
/// Interface for entities that support soft deletion
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted
    /// </summary>
    bool IsDeleted { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the entity was deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets who deleted the entity
    /// </summary>
    string? DeletedBy { get; set; }
    
    /// <summary>
    /// Marks the entity as deleted
    /// </summary>
    void Delete();
    
    /// <summary>
    /// Restores a previously deleted entity
    /// </summary>
    void Restore();
}

/// <summary>
/// Base class for entities that support soft deletion
/// </summary>
public abstract class SoftDeleteEntity : BaseEntity, ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the entity was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets who deleted the entity
    /// </summary>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// Marks the entity as deleted
    /// </summary>
    public virtual void Delete()
    {
        if (IsDeleted)
            return;
            
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        // DeletedBy will be set by DbContext
    }
    
    /// <summary>
    /// Restores a previously deleted entity
    /// </summary>
    public virtual void Restore()
    {
        if (!IsDeleted)
            return;
            
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}

/// <summary>
/// Base class for aggregate roots that also support soft deletion
/// </summary>
public abstract class SoftDeleteAggregateRoot : AggregateRoot, ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Gets or sets the date when the entity was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets who deleted the entity
    /// </summary>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// Marks the entity as deleted and raises a domain event
    /// </summary>
    public virtual void Delete()
    {
        if (IsDeleted)
            return;
            
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        // DeletedBy will be set by DbContext
        
        var @event = new EntitySoftDeletedEvent(Id, GetType().Name, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    /// <summary>
    /// Restores a previously deleted entity and raises a domain event
    /// </summary>
    public virtual void Restore()
    {
        if (!IsDeleted)
            return;
            
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        
        var @event = new EntityRestoredEvent(Id, GetType().Name, Version);
        AddDomainEvent(@event);
        ApplyDomainEvent(@event);
    }
    
    protected override void ApplyDomainEvent(IDomainEvent @event)
    {
        switch (@event)
        {
            case EntitySoftDeletedEvent e:
                Apply(e);
                break;
            case EntityRestoredEvent e:
                Apply(e);
                break;
            default:
                base.ApplyDomainEvent(@event);
                break;
        }
    }
    
    private void Apply(EntitySoftDeletedEvent @event)
    {
        IncrementVersion();
    }
    
    private void Apply(EntityRestoredEvent @event)
    {
        IncrementVersion();
    }
}
```

## 7. Domain Exception Hierarchy

```csharp
/// <summary>
/// Base exception for all domain-specific exceptions
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when domain validation fails
/// </summary>
public class DomainValidationException : DomainException
{
    public IReadOnlyCollection<string> Errors { get; }
    
    public DomainValidationException(IEnumerable<string> errors)
        : base($"Domain validation failed: {string.Join("; ", errors)}")
    {
        Errors = errors.ToList().AsReadOnly();
    }
    
    public DomainValidationException(string error)
        : base($"Domain validation failed: {error}")
    {
        Errors = new[] { error }.ToList().AsReadOnly();
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public Guid EntityId { get; }
    public Type EntityType { get; }
    
    public EntityNotFoundException(Guid id, Type entityType)
        : base($"Entity of type {entityType.Name} with ID {id} was not found")
    {
        EntityId = id;
        EntityType = entityType;
    }
    
    public EntityNotFoundException(Guid id, string entityTypeName)
        : base($"Entity of type {entityTypeName} with ID {id} was not found")
    {
        EntityId = id;
        EntityType = typeof(object); // Default if exact type not known
    }
}
```
