# Domain Layer Reference Guide

## Overview

The Domain Layer is the core of the SoftwareEngineerSkills application, implementing Domain-Driven Design (DDD) principles within Clean Architecture. It contains all business entities, value objects, domain logic, and business rules that represent the business model.

**Key Principles:**
- **Rich Domain Model**: Entities encapsulate both data and behavior
- **Dependency Rule**: No external dependencies - domain is technology-independent
- **Event-Driven Design**: Domain events for cross-aggregate communication
- **Invariant Validation**: Business rules enforced through comprehensive validation
- **Thread Safety**: Aggregate roots provide thread-safe domain event handling

## Core Components

### BaseEntity - Foundation Class

Provides identity, auditing, domain events, and validation for all domain entities:

```csharp
public abstract class BaseEntity : IAuditableEntity
{
    // Identity & Versioning
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public int Version { get; private set; }
    
    // Audit Properties
    public DateTime Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Domain Events (basic implementation)
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    // Invariant Validation System
    protected virtual IEnumerable<string> CheckInvariants()
    protected virtual Task<IEnumerable<string>> CheckInvariantsAsync(CancellationToken cancellationToken = default)
    public void EnforceInvariants()
    public async Task EnforceInvariantsAsync(CancellationToken cancellationToken = default)
    
    // Event Management
    public void AddDomainEvent(IDomainEvent domainEvent)
    public bool RemoveDomainEvent(IDomainEvent domainEvent)
    public void ClearDomainEvents()
    
    // Version Control
    public void IncrementVersion()
}
```

**Features:**
- **Unique Identity**: GUID-based identification
- **Audit Support**: Full `IAuditableEntity` implementation
- **Version Control**: Optimistic concurrency through version incrementing
- **Domain Events**: Built-in event handling for cross-aggregate communication
- **Business Rule Validation**: Sync/async invariant validation with `DomainValidationException`

### AggregateRoot - Enhanced Aggregate Boundary

Extends BaseEntity with thread-safe domain event handling and advanced event patterns:

```csharp
public abstract class AggregateRoot : BaseEntity, IAggregateRoot
{
    // Thread-safe event handling
    protected new void AddDomainEvent(IDomainEvent domainEvent) // Thread-safe + auto version increment
    public new void RemoveDomainEvent(IDomainEvent domainEvent) // Thread-safe
    public new void ClearDomainEvents() // Thread-safe
    
    // Event Sourcing Patterns
    protected virtual void Apply(IDomainEvent domainEvent)
    protected void AddAndApplyEvent(IDomainEvent domainEvent)
    protected async Task AddAndApplyEventAsync(IDomainEvent domainEvent)
    
    // Async invariant validation
    public Task EnforceInvariantsAsync()
}
```

**Key Enhancements:**
- **Thread Safety**: All domain event operations are thread-safe using internal locking
- **Automatic Versioning**: Version incremented automatically when events are added
- **Event Sourcing Support**: `Apply` pattern for event-driven state changes
- **Composite Operations**: `AddAndApplyEvent` methods combine event application with collection management

### ValueObject - Immutable Domain Concepts

Abstract base for immutable value types identified by their properties:

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj) { /* ... */ }
    public override int GetHashCode() { /* ... */ }
    public static bool operator ==(ValueObject? left, ValueObject? right) { /* ... */ }
}

// Example Implementation
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    // ... other properties
    
    public Address(string street, string city, /* ... */)
    {
        Street = Guard.Against.NullOrWhiteSpace(street, nameof(street));
        // ... validation and assignment
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        // ... other components
    }
}
```

### SoftDeleteEntity - Soft Deletion Support

For entities requiring soft deletion capabilities:

```csharp
public abstract class SoftDeleteEntity : BaseEntity, ISoftDelete
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    
    public virtual void SoftDelete(string deletedBy)
    {
        if (IsDeleted) return;
        
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = Guard.Against.NullOrWhiteSpace(deletedBy, nameof(deletedBy));
        
        AddDomainEvent(new EntitySoftDeletedEvent(Id, GetType().Name));
        EnforceInvariants();
    }
}
```

## Implementation Patterns

### Domain Entity Pattern

```csharp
public class Customer : AggregateRoot
{
    // Properties with private setters
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    
    // Collections with backing fields
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    
    // EF Core constructor
    private Customer() { }
    
    // Public constructor with validation
    public Customer(string name, Email email)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Email = Guard.Against.Null(email, nameof(email));
        
        AddDomainEvent(new CustomerCreatedEvent(Id, name, email.Value));
        EnforceInvariants();
    }
    
    // Behavior methods using enhanced event patterns
    public void UpdateName(string newName)
    {
        Guard.Against.NullOrWhiteSpace(newName, nameof(newName));
        
        if (Name == newName) return;
            
        var oldName = Name;
        Name = newName;
        
        // Using AddAndApplyEvent for atomic operation
        AddAndApplyEvent(new CustomerNameChangedEvent(Id, oldName, newName));
    }
    
    // Event sourcing pattern
    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CustomerNameChangedEvent nameChanged:
                // Additional state changes beyond direct property assignment
                LastNameChangeDate = DateTime.UtcNow;
                break;
        }
    }
    
    // Business rule validation
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return "Customer name cannot be empty";
            
        if (Name?.Length > 100)
            yield return "Customer name cannot exceed 100 characters";
    }
}
```

### Domain Events

Events signal important domain occurrences using past tense naming:

```csharp
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid Id { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid Id { get; }
    
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}

// Example Event
public class CustomerCreatedEvent : DomainEvent
{
    public Guid CustomerId { get; }
    public string CustomerName { get; }
    public string Email { get; }
    
    public CustomerCreatedEvent(Guid customerId, string customerName, string email)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        Email = email;
    }
}
```

## Advanced Event Handling Patterns

### Thread-Safe Event Management (AggregateRoot Only)

```csharp
public class Order : AggregateRoot
{
    public void ProcessConcurrentUpdates()
    {
        // Multiple threads can safely add events
        Parallel.For(0, 10, i =>
        {
            AddDomainEvent(new OrderItemAddedEvent(Id, $"Item{i}"));
        });
        
        // Version is automatically incremented for each event
        // Thread-safe operations ensure consistency
    }
}
```

### Event Sourcing Support

```csharp
public class Account : AggregateRoot
{
    public decimal Balance { get; private set; }
    
    public void Deposit(decimal amount)
    {
        // Apply event with automatic validation and event recording
        AddAndApplyEvent(new MoneyDepositedEvent(Id, amount));
    }
    
    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case MoneyDepositedEvent deposited:
                Balance += deposited.Amount;
                break;
            case MoneyWithdrawnEvent withdrawn:
                Balance -= withdrawn.Amount;
                break;
        }
    }
}
```

### Asynchronous Event Processing

```csharp
public class Customer : AggregateRoot
{
    public async Task UpdateEmailAsync(Email newEmail, CancellationToken cancellationToken = default)
    {
        var oldEmail = Email;
        Email = newEmail;
        
        // Async event application with validation
        await AddAndApplyEventAsync(new CustomerEmailChangedEvent(Id, oldEmail, newEmail));
    }
    
    protected override async Task<IEnumerable<string>> CheckInvariantsAsync(CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        errors.AddRange(CheckInvariants());
        
        // Async validation requiring I/O
        if (Email != null)
        {
            var emailExists = await emailValidationService.IsEmailInUseAsync(Email.Value, cancellationToken);
            if (emailExists)
                errors.Add("Email address is already in use");
        }
        
        return errors;
    }
}
```

## Domain Modeling Workflow

### 1. Identify Domain Concepts
- Extract key business concepts from requirements
- Distinguish between entities (have identity) and value objects (defined by properties)
- Identify aggregate boundaries (consistency boundaries)

### 2. Define Aggregates
- Determine aggregate roots and their boundaries
- Ensure aggregates maintain consistency internally
- Reference other aggregates by ID only

### 3. Model Entities and Value Objects
- Extend `AggregateRoot` for aggregate roots (recommended for main entities)
- Extend `BaseEntity` for simple entities within aggregates
- Inherit from `ValueObject` for immutable concepts
- Use `SoftDeleteEntity` when soft deletion is required

### 4. Implement Business Behavior
- Add methods that encapsulate domain operations
- Generate domain events for important state changes
- Use `AddAndApplyEvent` for event sourcing patterns
- Implement invariant validation in `CheckInvariants()` and `CheckInvariantsAsync()`

### 5. Define Interfaces
- `IAuditableEntity`: For entities requiring audit trails
- `ISoftDelete`: For entities supporting soft deletion
- `IAggregateRoot`: Marker interface for aggregate roots

## Exception Handling

### Domain Exception Hierarchy

```
DomainException (abstract)
├── BusinessRuleException          # Business rule violations
│   └── DomainValidationException  # Invariant validation failures
└── EntityNotFoundException        # Entity not found scenarios
```

### Usage Examples

```csharp
// Business rule violation
if (orderAmount < minimumOrderAmount)
    throw new BusinessRuleException(
        $"Order amount ${orderAmount} below minimum ${minimumOrderAmount}");

// Invariant validation (automatic via EnforceInvariants)
protected override IEnumerable<string> CheckInvariants()
{
    if (string.IsNullOrWhiteSpace(Name))
        yield return "Customer name cannot be empty";
}

// DomainValidationException thrown automatically with all validation errors
```

## Project Structure

```
SoftwareEngineerSkills.Domain/
├── Common/                     # Shared domain infrastructure
│   ├── Base/                   # Base classes for domain entities
│   │   ├── BaseEntity.cs       # Foundation entity with identity, auditing, and validation
│   │   ├── AggregateRoot.cs    # Thread-safe aggregate root with advanced event handling
│   │   └── SoftDeleteEntity.cs # Base class for entities supporting soft deletion
│   └── Events/                 # Domain events infrastructure
│       ├── IDomainEvent.cs     # Domain event interface
│       ├── DomainEvent.cs      # Base domain event implementation
│       └── SoftDeleteEvents.cs # Soft deletion related events
├── Aggregates/                 # Domain aggregates (main business entities)
│   └── Customer/               # Customer aggregate
│       ├── Customer.cs         # Customer aggregate root
│       └── CustomerEvents.cs   # Customer-specific domain events
├── Enums/                      # Domain enumerations
│   ├── SkillCategory.cs        # Skill categorization enumeration
│   └── SkillLevel.cs           # Skill proficiency level enumeration
├── Exceptions/                 # Domain-specific exception hierarchy
│   ├── DomainException.cs      # Base domain exception
│   ├── BusinessRuleException.cs # Business rule violations
│   ├── DomainValidationException.cs # Invariant validation failures
│   └── EntityNotFoundException.cs # Entity not found scenarios
├── Abstractions/               # Domain interfaces and abstractions
├── DomainServices/             # Domain services for cross-aggregate operations
├── Entities/                   # Additional domain entities
├── Rules/                      # Business rules and specifications
├── Shared/                     # Shared domain utilities
├── ValueObjects/               # Domain value objects
└── README.md                   # This documentation
```

## Best Practices

### Entity Design
✅ **DO:**
- Use `AggregateRoot` for main business entities requiring thread-safe event handling
- Use private setters for properties
- Expose behavior through methods, not property setters
- Validate inputs in constructors and methods
- Always call `EnforceInvariants()` or `EnforceInvariantsAsync()` after state changes
- Use `AddAndApplyEvent` for event sourcing patterns
- Use meaningful domain events with past tense naming

❌ **DON'T:**
- Expose public setters for domain properties
- Allow entities to be in invalid states
- Use domain entities as data transfer objects
- Reference other aggregates directly (use IDs)
- Mix thread-unsafe operations in aggregate roots

### Value Object Design
✅ **DO:**
- Make value objects immutable after creation
- Implement proper equality comparison via `GetEqualityComponents()`
- Validate all inputs in constructors
- Use factory methods for complex creation logic

### Domain Events
✅ **DO:**
- Use past tense naming (CustomerCreated, OrderShipped)
- Include sufficient context information
- Generate events before enforcing invariants when using basic `AddDomainEvent`
- Use `AddAndApplyEvent` for event sourcing patterns
- Keep events focused on single concerns

### Thread Safety (AggregateRoot)
✅ **DO:**
- Use AggregateRoot for entities requiring concurrent access
- Leverage built-in thread-safe event operations
- Use `AddAndApplyEvent` for atomic event processing

❌ **DON'T:**
- Assume BaseEntity is thread-safe (it's not)
- Manually implement locking when AggregateRoot provides it

### Invariant Validation
✅ **DO:**
- Express all business rules as invariants
- Provide clear, actionable error messages
- Support both sync and async validation
- Validate after every state change
- Handle `DomainValidationException` with multiple error messages

## Testing Guidelines

### Unit Testing Entities

```csharp
[Fact]
public void Customer_UpdateName_ShouldRaiseNameChangedEvent()
{
    // Arrange
    var customer = new Customer("John Doe", new Email("john@example.com"));
    customer.ClearDomainEvents();
    
    // Act
    customer.UpdateName("Jane Doe");
    
    // Assert
    customer.DomainEvents.Should().HaveCount(1);
    customer.DomainEvents.First().Should().BeOfType<CustomerNameChangedEvent>();
    
    // Verify version increment (AggregateRoot feature)
    customer.Version.Should().BeGreaterThan(0);
}

[Fact]
public void Customer_UpdateName_WithEmptyName_ShouldThrowDomainValidationException()
{
    // Arrange
    var customer = new Customer("John Doe", new Email("john@example.com"));
    
    // Act & Assert
    customer.Invoking(c => c.UpdateName(""))
        .Should().Throw<DomainValidationException>()
        .Which.Errors.Should().Contain("Customer name cannot be empty");
}

[Fact]
public void AggregateRoot_ConcurrentEventAddition_ShouldBeThreadSafe()
{
    // Arrange
    var customer = new Customer("John Doe", new Email("john@example.com"));
    customer.ClearDomainEvents();
    
    // Act - Add events concurrently
    Parallel.For(0, 100, i =>
    {
        customer.AddDomainEvent(new CustomerNameChangedEvent(customer.Id, "Old", $"New{i}"));
    });
    
    // Assert
    customer.DomainEvents.Should().HaveCount(100);
    customer.Version.Should().Be(100); // Version incremented for each event
}
```

### Testing Value Objects

```csharp
[Fact]
public void Address_WithSameValues_ShouldBeEqual()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "Country");
    var address2 = new Address("123 Main St", "City", "State", "12345", "Country");
    
    // Act & Assert
    address1.Should().Be(address2);
    address1.GetHashCode().Should().Be(address2.GetHashCode());
}
```

## Interfaces Reference

### Core Interfaces

```csharp
// Audit support
public interface IAuditableEntity
{
    DateTime Created { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}

// Soft deletion support
public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
}

// Aggregate root marker
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    Task EnforceInvariantsAsync();
}
```

## Key Implementation Notes

1. **Thread Safety**: Only AggregateRoot provides thread-safe domain event handling
2. **Automatic Versioning**: AggregateRoot increments version automatically on event addition
3. **Audit Properties**: Use `Created`, `CreatedBy`, `LastModified`, `LastModifiedBy`
4. **Domain Events**: Available in BaseEntity, enhanced in AggregateRoot
5. **Validation**: Both synchronous and asynchronous invariant validation with comprehensive error reporting
6. **Event Sourcing**: `Apply` pattern supported through `AddAndApplyEvent` methods

## Quick Start Checklist

For new domain entities:
- [ ] Choose appropriate base class:
  - `AggregateRoot` for main business entities (recommended)
  - `BaseEntity` for simple entities within aggregates
  - `SoftDeleteEntity` for entities requiring soft deletion
- [ ] Add private parameterless constructor for EF Core
- [ ] Implement public constructor with validation
- [ ] Use private setters with behavioral methods
- [ ] Implement `CheckInvariants()` for business rules
- [ ] Implement `CheckInvariantsAsync()` for async business rules if needed
- [ ] Generate domain events for state changes using appropriate methods:
  - `AddDomainEvent()` for basic event recording
  - `AddAndApplyEvent()` for event sourcing patterns
- [ ] Call `EnforceInvariants()` or `EnforceInvariantsAsync()` after mutations
- [ ] Override `Apply()` method if using event sourcing patterns
- [ ] Write comprehensive unit tests including thread safety for aggregates

---

**Additional Resources:**
- [CHANGELOG.md](../../../CHANGELOG.md) - Recent domain layer enhancements
- Code Examples: Study existing aggregates for implementation patterns
