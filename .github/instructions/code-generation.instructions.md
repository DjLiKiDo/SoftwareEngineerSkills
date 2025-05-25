---
applyTo: "**/*.cs"
---

# Code Generation Standards and Rules for SoftwareEngineerSkills Project

This instruction file provides comprehensive guidelines for GitHub Copilot when generating C# code for the SoftwareEngineerSkills .NET 9 enterprise project following Clean Architecture and Domain-Driven Design principles.

## Project Context and Domain

**Primary Domain:** Development Team Task Board - Enterprise task management system
**Business Context:** Managing development tasks with skill requirements, developer assignments, and intelligent matching

### Core Entities and Relationships
- **Task**: Work items with skill requirements, status tracking, and hierarchical structure
- **Developer**: Team members with skill portfolios and assignment tracking  
- **Project**: Organizational unit containing related tasks
- **Skills**: Technical capabilities with proficiency levels
- **Assignments**: Task-to-developer mappings based on skill matching

## Architectural Standards

### Clean Architecture Layers
1. **Domain Layer** (`SoftwareEngineerSkills.Domain`): Core business logic, entities, value objects, domain events
2. **Application Layer** (`SoftwareEngineerSkills.Application`): Use cases, CQRS commands/queries, application services
3. **Infrastructure Layer** (`SoftwareEngineerSkills.Infrastructure`): Data access, external services, persistence
4. **API Layer** (`SoftwareEngineerSkills.API`): Controllers, middleware, configuration

### Dependency Flow
- **Inward Dependencies Only**: Domain → Application → Infrastructure → API
- **Dependency Injection**: Use built-in .NET DI container
- **Interface Segregation**: Define interfaces in Application layer, implement in Infrastructure

## Code Generation Rules

### C# 14+ Language Features
- **Primary Constructors**: Use for simple classes without complex initialization
- **Collection Expressions**: Use `[item1, item2]` syntax for collection initialization
- **Required Properties**: Use `required` keyword for mandatory properties
- **File-Scoped Namespaces**: Always use file-scoped namespace declarations
- **Global Using Statements**: Define common usings in GlobalUsings.cs
- **Record Types**: Use for DTOs and immutable data structures

### Naming Conventions
- **Classes/Interfaces/Methods**: PascalCase (`TaskService`, `IRepository`)
- **Properties/Public Fields**: PascalCase (`AssignedDeveloperId`, `CreatedDate`)
- **Private Fields**: _camelCase with underscore prefix (`_skillRequirements`, `_unitOfWork`)
- **Local Variables/Parameters**: camelCase (`taskId`, `developerSkills`)
- **Constants**: PascalCase (`MaxConcurrentTasks`)
- **Interfaces**: Prefix with 'I' (`ITaskRepository`, `IAggregateRoot`)

### Domain Layer Code Generation

#### Entity Pattern
```csharp
public class Task : AggregateRoot
{
    public string Title { get; private set; } = null!;
    public TaskStatus Status { get; private set; }
    
    // Collections with backing fields
    private readonly List<TaskSkillRequirement> _skillRequirements = new();
    public IReadOnlyCollection<TaskSkillRequirement> SkillRequirements => _skillRequirements.AsReadOnly();
    
    // EF Core constructor
    private Task() { }
    
    // Business constructor with validation
    public Task(string title, string description, TaskPriority priority)
    {
        Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
        Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
        Priority = priority;
        Status = TaskStatus.Todo;
        
        AddDomainEvent(new TaskCreatedEvent(Id, title, priority));
        EnforceInvariants();
    }
    
    // Business behavior methods
    public void AssignToDeveloper(Guid developerId, IEnumerable<DeveloperSkill> developerSkills)
    {
        Guard.Against.Default(developerId, nameof(developerId));
        
        if (!CanBeAssignedTo(developerSkills))
            throw new BusinessRuleException("Developer does not have required skills for this task");
            
        AssignedDeveloperId = developerId;
        AddDomainEvent(new TaskAssignedEvent(Id, developerId));
        EnforceInvariants();
    }
    
    // Invariant validation
    protected override IEnumerable<string> CheckInvariants()
    {
        if (string.IsNullOrWhiteSpace(Title))
            yield return "Task title cannot be empty";
        if (Title?.Length > 200)
            yield return "Task title cannot exceed 200 characters";
    }
}
```

#### Value Object Pattern
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
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Skill;
        yield return Level;
        yield return AcquiredDate.Date;
    }
}
```

### Application Layer Code Generation

#### CQRS Command Pattern
```csharp
public record CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public TaskPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public List<TaskSkillRequirementDto> SkillRequirements { get; init; } = [];
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public CreateTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var task = new Task(request.Title, request.Description, request.Priority);
            
            if (request.DueDate.HasValue)
                task.SetDueDate(request.DueDate.Value);
            
            foreach (var skillReq in request.SkillRequirements)
            {
                task.AddSkillRequirement(skillReq.Category, skillReq.MinimumLevel);
            }
            
            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(_mapper.Map<TaskDto>(task));
        }
        catch (BusinessRuleException ex)
        {
            return Result.Failure<TaskDto>(ex.Message);
        }
    }
}
```

#### Query Pattern
```csharp
public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskDto>>;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IReadRepository<Task> _repository;
    private readonly IMapper _mapper;
    
    public GetTaskByIdQueryHandler(IReadRepository<Task> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        return task == null
            ? Result.Failure<TaskDto>($"Task with ID {request.Id} not found.")
            : Result.Success(_mapper.Map<TaskDto>(task));
    }
}
```

### API Layer Code Generation

#### Controller Pattern
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id));
        
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask(CreateTaskCommand command)
    {
        var result = await _mediator.Send(command);
        
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTaskById), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }
}
```

## Error Handling Standards

### Result Pattern Usage
- **Return Results**: Use `Result<T>` for operations that can fail
- **Exception Handling**: Reserve exceptions for unexpected errors
- **Validation**: Use FluentValidation for input validation
- **Business Rules**: Throw `BusinessRuleException` for domain rule violations

### Exception Hierarchy
- `DomainException` (abstract base)
- `BusinessRuleException` (business rule violations)
- `DomainValidationException` (invariant validation failures)
- `EntityNotFoundException` (entity not found scenarios)

## Testing Standards

### Unit Test Pattern
```csharp
[Fact]
public async Task Handle_ValidCommand_ShouldCreateTask()
{
    // Arrange
    var command = new CreateTaskCommand
    {
        Title = "Implement user authentication",
        Description = "Add JWT authentication to the API",
        Priority = TaskPriority.High
    };
    
    var taskDto = new TaskDto { Id = Guid.NewGuid(), Title = command.Title };
    _mapperMock.Setup(m => m.Map<TaskDto>(It.IsAny<Task>())).Returns(taskDto);
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeEquivalentTo(taskDto);
    
    _taskRepositoryMock.Verify(
        r => r.AddAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()),
        Times.Once);
}
```

## Documentation Standards

### XML Documentation
- **Public APIs**: Always include XML comments for public methods, classes, and properties
- **Business Logic**: Document complex business rules and domain concepts
- **Parameters**: Document all parameters with `<param>` tags
- **Returns**: Document return values with `<returns>` tags
- **Examples**: Include `<example>` tags for complex usage scenarios

### Inline Comments
- **Business Logic**: Explain complex business rules and decisions
- **Algorithms**: Document non-obvious algorithmic choices
- **Workarounds**: Clearly mark and explain any workarounds
- **TODOs**: Use TODO comments for future improvements

## Performance and Security

### Async/Await Patterns
- **I/O Operations**: Always use async/await for database and external service calls
- **ConfigureAwait**: Use `ConfigureAwait(false)` in library code
- **Cancellation Tokens**: Always accept and honor cancellation tokens

### Security Considerations
- **Input Validation**: Validate all external inputs
- **SQL Injection**: Use parameterized queries via EF Core
- **Authentication**: Implement JWT-based authentication
- **Authorization**: Use policy-based authorization
- **Secrets Management**: Use configuration providers for sensitive data

## Dependencies and Libraries

### Preferred Libraries
- **ORM**: Entity Framework Core 9
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Testing**: xUnit, Moq, FluentAssertions
- **Logging**: Serilog
- **HTTP Clients**: HttpClientFactory
- **JSON**: System.Text.Json

### Avoid These Patterns
- **Static Classes**: Avoid static dependencies (except for pure functions)
- **Service Locator**: Use dependency injection instead
- **God Objects**: Keep classes focused and cohesive
- **Anemic Models**: Domain entities should contain behavior, not just data
- **Direct Database Access**: Always use repositories and Unit of Work

## Code Quality Requirements

### SOLID Principles
- **Single Responsibility**: Each class should have one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes must be substitutable for base classes
- **Interface Segregation**: Clients shouldn't depend on interfaces they don't use
- **Dependency Inversion**: Depend on abstractions, not concretions

### Clean Code Practices
- **Meaningful Names**: Use intention-revealing names
- **Small Functions**: Functions should do one thing well
- **No Magic Numbers**: Use named constants for literal values
- **Consistent Formatting**: Use consistent indentation and spacing
- **Eliminate Dead Code**: Remove unused code regularly

Remember: Always prioritize code clarity, maintainability, and adherence to the established architectural patterns. When in doubt, favor explicit code over clever code.
