# Domain Layer Code Organization and Consistency Improvements

After analyzing the domain layer code, here are specific improvements to enhance readability, consistency, and organization:

## 1. Entity and Aggregate Event Co-location

### Current Issue:
Events are defined in separate files (e.g., `SkillEvents.cs`, `CustomerEvents.cs`) from their corresponding entities.

### Recommendation:
1. Either keep events in the same file as the entity for simple cases, or
2. For more complex aggregates, use a consistent subfolder structure:
```
Aggregates/
├── Customer/
│   ├── Customer.cs                <- Aggregate root
│   ├── Events/                    <- Folder for events
│   │   ├── CustomerCreatedEvent.cs
│   │   └── CustomerUpdatedEvent.cs
│   └── ValueObjects/              <- Customer-specific value objects
└── Skills/
    ├── Skill.cs
    ├── Events/
    └── ValueObjects/
```

## 2. Name Standardization for Consistent Event Properties

### Current Issue:
Inconsistent property naming in domain events. For example, `SkillUpdatedEvent` uses `SkillName` while other events use names like `OldName` and `NewName`.

### Recommendation:
Standardize property naming conventions across all domain events:
- Use `Id` consistently (not `SkillId`, `CustomerId`, etc.)
- For updated properties, use `Old{PropertyName}` and `New{PropertyName}` consistently
- Include the aggregate type in the event name (e.g., `SkillCreatedEvent`, not just `CreatedEvent`)

## 3. Apply Pattern for Event Handling

### Current Issue:
Not all aggregates consistently implement an Apply pattern for handling domain events.

### Recommendation:
Implement a consistent Apply pattern across all aggregates:

```csharp
public void UpdateName(string newName)
{
    if (string.IsNullOrWhiteSpace(newName))
        throw new ArgumentException("Name cannot be empty", nameof(newName));
    
    if (Name == newName)
        return;
    
    var oldName = Name;
    Name = newName;
    
    var @event = new SkillNameUpdatedEvent(Id, oldName, newName);
    AddDomainEvent(@event);
    ApplyDomainEvent(@event); // Apply pattern call
}

protected void ApplyDomainEvent(IDomainEvent @event)
{
    switch (@event)
    {
        case SkillNameUpdatedEvent e:
            Apply(e);
            break;
        // Other events
    }
}

private void Apply(SkillNameUpdatedEvent @event)
{
    // Additional logic related to name change, if needed
    Version++;
}
```

## 4. XML Documentation Improvements

### Current Issue:
Inconsistent level of XML documentation across different classes and components.

### Recommendation:
1. Add more detailed XML documentation to all public methods, especially in base classes
2. Document the "why" behind complex domain rules, not just the "what"
3. Include examples in XML comments for complex methods
4. Add `<remarks>` sections for implementation notes and usage guidance

Example enhancement:
```csharp
/// <summary>
/// Updates the difficulty level of the skill.
/// </summary>
/// <param name="newLevel">The new difficulty level to assign</param>
/// <remarks>
/// When changing difficulty level, entities or services that depend on this skill
/// may need to be notified. This is handled through the SkillDifficultyChangedEvent.
/// 
/// The event contains both the old and new difficulty levels to enable proper handling
/// by subscribers.
/// </remarks>
/// <exception cref="BusinessRuleException">
/// Thrown when attempting to set the skill level to the same value it already has
/// </exception>
public void UpdateDifficultyLevel(SkillLevel newLevel)
{
    // Implementation
}
```

## 5. Invariant Validation System Standardization

### Current Issue:
The `CheckInvariants` method is implemented inconsistently across different entities.

### Recommendation:
1. Create a standard pattern for implementing invariant checks:
```csharp
protected override IEnumerable<string> CheckInvariants()
{
    // Chain with base class invariants
    foreach (var error in base.CheckInvariants())
    {
        yield return error;
    }

    // Entity-specific invariants
    if (string.IsNullOrWhiteSpace(Name))
    {
        yield return "Skill name cannot be empty";
    }
    
    if (Description?.Length > 1000)
    {
        yield return "Skill description cannot exceed 1000 characters";
    }
}
```

2. Add XML documentation to explain invariant rules

## 6. Relationship Management within Aggregates

### Current Issue:
Some aggregate roots do not properly manage relationships with child entities.

### Recommendation:
Implement clear patterns for relationship management:

```csharp
// For collections
private readonly List<SubSkill> _subSkills = new();
public IReadOnlyCollection<SubSkill> SubSkills => _subSkills.AsReadOnly();

public void AddSubSkill(string name, string description)
{
    var subSkill = new SubSkill(name, description);
    _subSkills.Add(subSkill);
    AddDomainEvent(new SubSkillAddedEvent(Id, subSkill.Id, name));
}

public void RemoveSubSkill(Guid subSkillId)
{
    var subSkill = _subSkills.FirstOrDefault(s => s.Id == subSkillId);
    if (subSkill == null)
        return;
        
    _subSkills.Remove(subSkill);
    AddDomainEvent(new SubSkillRemovedEvent(Id, subSkillId));
}
```

## 7. Value Object Improvements

### Current Issue:
Value objects could be better utilized for domain concepts.

### Recommendation:
1. Create more Value Objects for domain concepts that represent values without identity:
   - `SkillName` for validation rules around skill names
   - `Duration` for time-based concepts
   - `Proficiency` to encapsulate proficiency levels with more business logic

2. Ensure consistent implementation:
```csharp
public class SkillName : ValueObject
{
    public string Value { get; }
    
    private SkillName() { } // For EF Core
    
    public SkillName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Skill name cannot be empty", nameof(value));
            
        if (value.Length > 100)
            throw new ArgumentException("Skill name cannot exceed 100 characters", nameof(value));
            
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator string(SkillName name) => name.Value;
    public static explicit operator SkillName(string name) => new(name);
}
```

## 8. Consistent Async Support

### Current Issue:
Inconsistent async support across the domain layer.

### Recommendation:
1. Implement `CheckInvariantsAsync` consistently across entities that might have async validations
2. Use Task-based Async Pattern for methods that might involve async operations

## 9. Error Handling and Exceptions

### Current Issue:
Inconsistent use of exception types for domain rule violations.

### Recommendation:
1. Create a hierarchy of domain-specific exceptions
2. Use `DomainValidationException` consistently for invariant violations
3. Use `BusinessRuleException` for business rule violations during operations
4. Include helpful context in exception messages

Example:
```csharp
// Exception hierarchy
public class DomainException : Exception { /* implementation */ }
public class BusinessRuleException : DomainException { /* implementation */ }  
public class DomainValidationException : DomainException { /* implementation */ }
public class EntityNotFoundException : DomainException { /* implementation */ }

// Usage
public void AssignToProject(Project project)
{
    if (project == null)
        throw new ArgumentNullException(nameof(project));
        
    if (IsAssigned)
        throw new BusinessRuleException($"Skill '{Name}' is already assigned to a project");
        
    // Implementation
}
```
