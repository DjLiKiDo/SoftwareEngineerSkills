---
mode: 'agent'
tools: ['codebase']
description: 'Create new domain entities following DDD patterns'
applyTo: '**/Domain/**/*.cs'
---

# Create Domain Entity

Create a new domain entity following Domain-Driven Design patterns and Clean Architecture principles for the SoftwareEngineerSkills task board system.

## Context

You are creating domain entities for a **Development Team Task Board** that models:
- Tasks requiring specific technical skills
- Developers with varying skill proficiency levels
- Projects organizing tasks into business initiatives
- Smart assignment matching capabilities

## Instructions

### 1. Entity Analysis
First, analyze the business requirements to determine:
- **Entity Type**: Is this an aggregate root, entity, or value object?
- **Base Class**: Choose from `AggregateRoot`, `BaseEntity`, or `SoftDeleteEntity`
- **Business Boundaries**: What operations belong to this entity?
- **Relationships**: How does it relate to other entities?

### 2. Implementation Requirements

Follow patterns from [Code Generation Standards](../instructions/code-generation-standards.instructions.md):

#### Entity Structure
```csharp
public class [EntityName] : AggregateRoot
{
    // Properties with private setters
    public string Title { get; private set; }
    
    // Collections with backing fields
    private readonly List<[ChildEntity]> _children = new();
    public IReadOnlyCollection<[ChildEntity]> Children => _children.AsReadOnly();
    
    // EF Core constructor
    private [EntityName]() { }
    
    // Public constructor with validation
    public [EntityName](string title, [parameters])
    {
        Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
        
        AddDomainEvent(new [EntityName]CreatedEvent(Id, title));
        EnforceInvariants();
    }
}
```

#### Business Behavior
- Implement behavior through methods, not property setters
- Add domain events for important state changes
- Use `AddDomainEvent()` for basic events or `AddAndApplyEvent()` for event sourcing
- Always call `EnforceInvariants()` after state changes

#### Validation
```csharp
protected override IEnumerable<string> CheckInvariants()
{
    if (string.IsNullOrWhiteSpace(Title))
        yield return "[Entity] title cannot be empty";
        
    if (Title?.Length > 200)
        yield return "[Entity] title cannot exceed 200 characters";
}
```

### 3. Domain Events
Generate appropriate domain events:
```csharp
public class [EntityName]CreatedEvent : DomainEvent
{
    public Guid [EntityName]Id { get; }
    public string Title { get; }
    
    public [EntityName]CreatedEvent(Guid entityId, string title)
    {
        [EntityName]Id = entityId;
        Title = title;
    }
}
```

### 4. Value Objects (if applicable)
For immutable concepts without identity:
```csharp
public class [ValueObjectName] : ValueObject
{
    public string Property1 { get; private set; }
    public int Property2 { get; private set; }
    
    public [ValueObjectName](string property1, int property2)
    {
        Property1 = Guard.Against.NullOrWhiteSpace(property1, nameof(property1));
        Property2 = Guard.Against.Negative(property2, nameof(property2));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Property1.ToLowerInvariant();
        yield return Property2;
    }
}
```

## What to Ask Me

To create the perfect entity, ask me for:

1. **Entity Details**
   - Entity name and primary purpose
   - Key properties and their types
   - Business rules and constraints

2. **Behavior Requirements**
   - What business operations does this entity perform?
   - What state changes generate domain events?
   - What validation rules must be enforced?

3. **Relationships**
   - Does this entity reference other aggregates?
   - What child entities or value objects does it contain?
   - Is this part of an existing aggregate or a new one?

4. **Lifecycle Management**
   - Does this entity need soft deletion?
   - What audit requirements exist?
   - Are there any special creation or update rules?

## Examples

### Task Aggregate Root
```csharp
public class Task : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Guid? AssignedDeveloperId { get; private set; }
    
    // Skill requirements collection
    private readonly List<TaskSkillRequirement> _skillRequirements = new();
    public IReadOnlyCollection<TaskSkillRequirement> SkillRequirements => _skillRequirements.AsReadOnly();
    
    private Task() { } // EF Core
    
    public Task(string title, string description, TaskPriority priority)
    {
        Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        Priority = priority;
        Status = TaskStatus.Todo;
        
        AddDomainEvent(new TaskCreatedEvent(Id, title, priority));
        EnforceInvariants();
    }
    
    public void AssignToDeveloper(Guid developerId, IEnumerable<DeveloperSkill> developerSkills)
    {
        Guard.Against.Default(developerId, nameof(developerId));
        
        if (!CanBeAssignedTo(developerSkills))
            throw new BusinessRuleException("Developer does not have required skills");
            
        var previousDeveloperId = AssignedDeveloperId;
        AssignedDeveloperId = developerId;
        
        AddDomainEvent(new TaskAssignedEvent(Id, developerId, previousDeveloperId));
        EnforceInvariants();
    }
    
    public void AddSkillRequirement(SkillCategory category, SkillLevel minimumLevel)
    {
        var requirement = new TaskSkillRequirement(category, minimumLevel, true);
        _skillRequirements.Add(requirement);
        
        AddDomainEvent(new TaskSkillRequirementAddedEvent(Id, category, minimumLevel));
        EnforceInvariants();
    }
    
    public bool CanBeAssignedTo(IEnumerable<DeveloperSkill> developerSkills)
    {
        return _skillRequirements.All(requirement => 
            developerSkills.Any(skill => skill.MeetsRequirement(requirement)));
    }
    
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Title))
            yield return "Task title cannot be empty";
            
        if (Title?.Length > 200)
            yield return "Task title cannot exceed 200 characters";
            
        if (AssignedDeveloperId.HasValue && Status == TaskStatus.Todo)
            yield return "Assigned tasks cannot have Todo status";
    }
}
```

### Developer Skill Value Object
```csharp
public class DeveloperSkill : ValueObject
{
    public Skill Skill { get; private set; }
    public SkillLevel Level { get; private set; }
    public DateTime AcquiredDate { get; private set; }
    
    private DeveloperSkill() { } // EF Core
    
    public DeveloperSkill(Skill skill, SkillLevel level, DateTime acquiredDate)
    {
        Skill = skill ?? throw new ArgumentNullException(nameof(skill));
        Level = level;
        AcquiredDate = acquiredDate;
        
        if (acquiredDate > DateTime.UtcNow)
            throw new BusinessRuleException("Skill acquisition date cannot be in the future");
    }
    
    public bool MeetsRequirement(TaskSkillRequirement requirement)
    {
        return Skill.Category == requirement.Category && 
               Level >= requirement.MinimumLevel;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Skill;
        yield return Level;
        yield return AcquiredDate.Date;
    }
}
```

## Quality Checklist

Before completing the entity creation, ensure:

✅ **Proper Base Class**: Correct choice of `AggregateRoot`, `BaseEntity`, or `SoftDeleteEntity`  
✅ **Encapsulation**: Private setters with behavior methods  
✅ **Validation**: Input validation and invariant checking  
✅ **Domain Events**: Appropriate events for state changes  
✅ **Business Logic**: Rich behavior, not anemic data model  
✅ **Documentation**: XML comments for public API  
✅ **Testing**: Consider what unit tests will be needed  

## Ready to Create

**What domain entity would you like me to create?**

Provide details about:
- Entity name and purpose
- Key properties and business rules
- Relationships with other entities
- Special requirements or constraints
