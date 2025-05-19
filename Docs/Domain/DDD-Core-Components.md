# Domain-Driven Design Core Components

This document provides an overview of the core Domain-Driven Design components implemented in the SoftwareEngineerSkills project.

## Overview

The domain model is composed of several foundational components that help enforce domain rules and maintain the integrity of the domain:

- **BaseEntity**: Abstract base class for all domain entities
- **ValueObject**: Abstract base class for immutable value types
- **AggregateRoot**: Base class for aggregate roots that coordinate domain events and enforce boundaries
- **Domain Events**: System for signaling important occurrences within the domain

## Core Components

### BaseEntity

The `BaseEntity` class provides common functionality for all domain entities:

- Unique identifier
- Auditing properties (creation/modification timestamps and users)
- Version control for optimistic concurrency
- Invariant validation

```csharp
public abstract class BaseEntity : IAuditableEntity
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; } = 1;
    
    protected virtual IEnumerable<string> CheckInvariants()
    {
        yield break;
    }
    
    public void EnforceInvariants()
    {
        var errors = CheckInvariants().ToList();
        if (errors.Any())
        {
            throw new DomainValidationException(errors);
        }
    }
    
    public async Task EnforceInvariantsAsync()
    {
        var errors = (await CheckInvariantsAsync()).ToList();
        if (errors.Any())
        {
            throw new DomainValidationException(errors);
        }
    }
}
```

### ValueObject

The `ValueObject` class implements value semantics for immutable objects that are identified by their properties:

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
}
```

### AggregateRoot

The `AggregateRoot` class extends `BaseEntity` and implements the `IAggregateRoot` interface:

```csharp
public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    private readonly object _domainEventLock = new();
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        lock (_domainEventLock)
        {
            base.AddDomainEvent(domainEvent);
        }
        
        // After adding a domain event, increment the version
        IncrementVersion();
    }
    
    protected virtual void Apply(IDomainEvent domainEvent)
    {
        // Default implementation does nothing
        // Derived classes should override this method to apply the event to the aggregate state
    }
    
    protected void AddAndApplyEvent(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AddDomainEvent(domainEvent);
        EnforceInvariants();
    }
}
```

### Domain Events

Domain events represent significant occurrences within the domain model:

```csharp
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }
    
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}
```

## Usage Example

Here's an example of a Customer aggregate root that uses these core components:

```csharp
public class Customer : AggregateRoot
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    
    private Customer() { }
    
    public Customer(string name, Email email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        
        AddDomainEvent(new CustomerCreatedEvent(Id, name, email.Value));
        EnforceInvariants();
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
            
        var oldName = Name;
        Name = name;
        
        AddDomainEvent(new CustomerNameUpdatedEvent(Id, oldName, name));
        EnforceInvariants();
    }
    
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return "Customer name cannot be empty";
        }
        
        if (Email == null)
        {
            yield return "Customer email cannot be null";
        }
    }
}
```

## Invariant Validation

Invariant validation ensures that domain entities always maintain a valid state:

1. The `CheckInvariants` method returns a collection of error messages if any invariants are violated.
2. The `EnforceInvariants` method throws a `DomainValidationException` if any invariants are violated.
3. Invariants should be enforced after any state change to an entity.

```csharp
public void UpdateEmail(Email email)
{
    var oldEmail = Email;
    Email = email ?? throw new ArgumentNullException(nameof(email));
    
    AddDomainEvent(new CustomerEmailUpdatedEvent(Id, oldEmail.Value, email.Value));
    EnforceInvariants(); // Validate invariants after state change
}
```

## Thread Safety

The domain components are designed to be thread-safe:

1. Domain events are added to the collection under a lock to prevent race conditions
2. Version control enables optimistic concurrency
3. Immutable value objects ensure thread safety for complex values

## Best Practices

- Always enforce invariants after state changes
- Use private setters for properties that should only be changed through methods
- Include domain events for significant state changes
- Keep entities focused on behavior rather than just data storage
- Make value objects immutable
