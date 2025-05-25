# GitHub Copilot Instructions for .NET 9 Enterprise Web API Template

This document provides instructions for GitHub Copilot to optimize AI assistance for this project.

## 1. Project Definition

### Primary Domain and Tech Stack
- **Primary Domain:** Development Team Task Board - Enterprise task management system using .NET 9
- **Tech Stack:**
  - **Backend Framework:** .NET 9 / ASP.NET Core
  - **Language:** C# 14+
  - **ORM:** Entity Framework Core 9
  - **API Documentation:** OpenAPI/Swagger (Swashbuckle)
  - **Testing:** xUnit, Moq, FluentAssertions, Testcontainers
  - **Observability:** Serilog, OpenTelemetry
  - **Validation:** FluentValidation
  - **Middleware:** CQRS with MediatR
  - **Containerization:** Docker
  - **CI/CD:** GitHub Actions

### Key Architectural Patterns
- **Clean Architecture:** Layered structure with Domain at the center
- **Domain-Driven Design (DDD):**
  - Rich domain model with entities, value objects, domain events
  - Enhanced `BaseEntity` with invariant validation system
  - Improved `AggregateRoot` for thread-safe domain event handling
  - Comprehensive `ValueObject` implementation 
- **CQRS Pattern:** Separation of command and query responsibilities
- **Repository Pattern with Unit of Work:** 
  - Abstraction over data access
  - Specialized repositories for soft-delete entities
  - EF Core extensions for working with soft-deleted entities
- **Result Pattern:** Exception-free error handling
- **Mediator Pattern:** For handling commands and queries
- **Options Pattern:** For configuration management
- **Screaming Architecture:** Structure reveals intent
- **Entity Auditing System:** 
  - Sophisticated tracking through `IAuditableEntity` interface
  - Soft delete capabilities with `ISoftDelete` interface
  - `SoftDeleteEntity` base class for easy implementation

### Coding Conventions and Standards
- **Naming:**
  - PascalCase for class names, method names, public properties
  - camelCase for local variables and private fields
  - _camelCase for private fields (underscore prefix)
  - Interfaces prefixed with 'I' (e.g., IRepository)
  - Abstract classes may be prefixed with 'Base'
  - Use meaningful, descriptive names

- **Organization:**
  - One class per file (except small related classes)
  - Organize files by feature within each architectural layer
  - Follow Clean Architecture dependency flow (dependencies point inward)

- **Comments:** Use XML comments for public APIs and complex logic
- **Async Pattern:** Always use async/await for I/O operations
- **Error Handling:** Use Result pattern over exceptions for expected error paths

### Common Terminology and Abbreviations
- **DDD:** Domain-Driven Design
- **CQRS:** Command Query Responsibility Segregation
- **EF:** Entity Framework
- **DTO:** Data Transfer Object
- **POCO:** Plain Old CLR Object
- **IoC:** Inversion of Control
- **DI:** Dependency Injection
- **UoW:** Unit of Work
- **JWT:** JSON Web Token

### Project Structure and Organization
```
├── SoftwareEngineerSkills.sln
├── src
│   ├── SoftwareEngineerSkills.API              # Web API Controllers, Configuration
│   ├── SoftwareEngineerSkills.Application      # Business Logic, Commands, Queries
│   ├── SoftwareEngineerSkills.Common           # Shared utilities and helpers
│   ├── SoftwareEngineerSkills.Domain           # Entities, Value Objects, Domain Events
│   └── SoftwareEngineerSkills.Infrastructure   # Data Access, External Services
└── tests
    ├── SoftwareEngineerSkills.API.UnitTests
    ├── SoftwareEngineerSkills.Application.UnitTests
    ├── SoftwareEngineerSkills.Domain.UnitTests
    ├── SoftwareEngineerSkills.Infrastructure.UnitTests
    └── SoftwareEngineerSkills.IntegrationTests
```

- **Domain Layer:**
  - **SoftwareEngineerSkills.Domain:** Contains business entities, value objects, and domain logic
  - **Common/Base:** Core DDD components (`BaseEntity`, `AggregateRoot`, `ValueObject`, `SoftDeleteEntity`)
  - **Common/Events:** Domain event handling (`IDomainEvent`, `DomainEvent`)
  - **Common/Interfaces:** Core interfaces (`IAggregateRoot`, `IAuditableEntity`, `ISoftDelete`)
  - **Exceptions:** Domain-specific exceptions (`BusinessRuleException`, `DomainValidationException`)

- **Application Layer:**
  - **SoftwareEngineerSkills.Application:** Contains application services, commands/queries, validators

- **Infrastructure Layer:**
  - **SoftwareEngineerSkills.Infrastructure:** Implements interfaces defined in the Domain layer
  - Contains data access, external service integrations, logging, etc.

- **API Layer:**
  - **SoftwareEngineerSkills.API:** Web API controllers, middleware, configuration 

- **Common Layer:**
  - **SoftwareEngineerSkills.Common:** Shared utilities and helpers used across projects

- **Tests:**
  - **Unit Tests:** Individual test projects for each layer (API, Application, Domain, Infrastructure)
  - **Integration Tests:** End-to-end tests covering multiple layers

## 2. Workspace-Specific Instructions

### Preferred Code Patterns and Practices

#### Domain Entities
```csharp
namespace SoftwareEngineerSkills.Domain.Aggregates.Task;

public class Task : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid? AssignedDeveloperId { get; private set; }
    public Guid? ParentTaskId { get; private set; }
    
    private readonly List<TaskSkillRequirement> _skillRequirements = new();
    public IReadOnlyCollection<TaskSkillRequirement> SkillRequirements => _skillRequirements.AsReadOnly();
    
    private readonly List<Guid> _subtaskIds = new();
    public IReadOnlyCollection<Guid> SubtaskIds => _subtaskIds.AsReadOnly();
    
    private Task() { } // For EF Core
    
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
            throw new BusinessRuleException("Developer does not have required skills for this task");
            
        if (AssignedDeveloperId == developerId) return;
            
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
}
```

#### Value Objects
```csharp
namespace SoftwareEngineerSkills.Domain.ValueObjects;

public class DeveloperSkill : ValueObject
{
    public Skill Skill { get; private set; }
    public SkillLevel Level { get; private set; }
    public DateTime AcquiredDate { get; private set; }
    public DateTime? LastUsedDate { get; private set; }
    
    private DeveloperSkill() { } // For EF Core
    
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
    
    public static implicit operator SkillLevel(DeveloperSkill developerSkill) => developerSkill.Level;
}
```

#### CQRS Commands and Queries
```csharp
// Command
namespace SoftwareEngineerSkills.Application.Tasks.Commands;

public record CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public string Title { get; init; }
    public string Description { get; init; }
    public TaskPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public List<TaskSkillRequirementDto> SkillRequirements { get; init; } = new();
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

// Query
namespace SoftwareEngineerSkills.Application.Tasks.Queries;

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
        
        if (task == null)
            return Result.Failure<TaskDto>($"Task with ID {request.Id} not found.");
            
        return Result.Success(_mapper.Map<TaskDto>(task));
    }
}
```

#### API Controllers
```csharp
namespace SoftwareEngineerSkills.API.Controllers.V1;

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
    
    [HttpPut("{id}/assign")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTask(Guid id, AssignTaskCommand command)
    {
        if (id != command.TaskId)
            return BadRequest("Task ID mismatch");
            
        var result = await _mediator.Send(command);
        
        return result.IsSuccess
            ? Ok(result.Value)
            : result.Error.Contains("not found") 
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}
```

### Error Handling Requirements

- **Result Pattern:** Use the Result pattern for domain and application errors
- **Exception Handling:** Use middleware for global exception handling
- **Validations:** FluentValidation for input validation
- **Logging:** Log all errors with appropriate context

```csharp
// Global Exception Handling Middleware
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
        
        context.Response.StatusCode = statusCode;
        
        var response = new
        {
            status = statusCode,
            message = exception.Message,
            detail = exception is ValidationException validationException 
                ? validationException.Errors 
                : null
        };
        
        return context.Response.WriteAsJsonAsync(response);
    }
}
```

### Testing and Documentation Standards

#### Testing
- **Unit Tests:** Test individual components in isolation with mock dependencies
- **Integration Tests:** Test components with real dependencies using Testcontainers for real database dependencies
- **Functional Tests:** Test the entire API stack with WebApplicationFactory

```csharp
// Unit Test Example
public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTaskCommandHandler _handler;
    
    public CreateTaskCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _unitOfWorkMock.Setup(uow => uow.Tasks).Returns(_taskRepositoryMock.Object);
        
        _handler = new CreateTaskCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Implement user authentication",
            Description = "Add JWT authentication to the API",
            Priority = TaskPriority.High,
            SkillRequirements = new List<TaskSkillRequirementDto>
            {
                new() { Category = SkillCategory.Backend, MinimumLevel = SkillLevel.Intermediate }
            }
        };
        
        var taskDto = new TaskDto 
        { 
            Id = Guid.NewGuid(), 
            Title = command.Title, 
            Description = command.Description,
            Priority = command.Priority,
            Status = TaskStatus.Todo
        };
        
        _mapperMock
            .Setup(m => m.Map<TaskDto>(It.IsAny<Task>()))
            .Returns(taskDto);
            
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(taskDto);
        
        _taskRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()),
            Times.Once);
            
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_TaskWithSkillRequirements_ShouldAddSkillRequirements()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Frontend React Component",
            Description = "Create reusable React component",
            Priority = TaskPriority.Medium,
            SkillRequirements = new List<TaskSkillRequirementDto>
            {
                new() { Category = SkillCategory.Frontend, MinimumLevel = SkillLevel.Advanced },
                new() { Category = SkillCategory.JavaScript, MinimumLevel = SkillLevel.Intermediate }
            }
        };
        
        var taskDto = new TaskDto { Id = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<TaskDto>(It.IsAny<Task>())).Returns(taskDto);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _taskRepositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Task>(t => t.SkillRequirements.Count == 2), 
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

#### Documentation
- **XML Comments:** Use XML comments for all public APIs
- **README:** Keep the project README up-to-date
- **API Documentation:** Swagger with examples and descriptions
- **Architecture Decision Records:** Document key architectural decisions
- **Changelog:** Maintain a changelog for versioning

### Security Considerations

- **Authentication:** JWT/OAuth2 with proper token validation
- **Authorization:** Role and policy-based authorization
- **Input Validation:** Always validate inputs with FluentValidation
- **HTTPS:** Enforce HTTPS in production
- **Security Headers:** Implement security headers
- **Data Protection:** Use data protection APIs for sensitive data
- **Secrets:** Use secret management (User Secrets, Azure Key Vault)

### Performance Requirements

- **Asynchronous Operations:** Use async/await for I/O-bound operations
- **Caching:** Implement caching for frequently accessed data
- **Query Optimization:** Optimize database queries (includes, projections)
- **Pagination:** Use pagination for large collections
- **Response Compression:** Enable response compression
- **Rate Limiting:** Implement rate limiting for APIs

## 3. File-level Instructions

### Domain Layer
- Keep entities focused on business behavior
- Encapsulate state changes with methods
- Use private setters for properties that should only be changed through methods
- Implement value objects for concepts with no identity
- Use domain events for cross-aggregate communication
- Use the invariant validation system (`CheckInvariants`, `EnforceInvariants`) to enforce business rules
- Ensure thread safety when handling domain events
- Extend `SoftDeleteEntity` for entities that need soft deletion capabilities
- Implement proper validation in value objects for domain consistency

### Application Layer
- Implement CQRS pattern (separate commands and queries)
- Use validators for commands and queries
- Return Result objects instead of throwing exceptions
- Implement domain event handlers
- Keep services focused on orchestration

### Infrastructure Layer
- Implement repositories with EF Core
- Use specification pattern for complex queries
- Configure entities using fluent API
- Implement Unit of Work pattern
- Use dependency injection for services
- Implement specialized repositories for soft-delete entities (`ISoftDeleteRepository<T>`)
- Configure global query filters to automatically exclude soft-deleted entities
- Properly handle audit properties through DbContext for `IAuditableEntity` entities

### WebApi Layer
- Implement API versioning
- Use action filters for cross-cutting concerns
- Configure Swagger documentation
- Return appropriate HTTP status codes
- Implement health checks

## 4. Formatting Instructions

- **Clear, Concise Language:** Focus on expressing intent clearly
- **Code Examples:** Provide examples for common patterns
- **Documentation Links:** Reference relevant documentation
- **Context:** Consider architecture constraints and patterns

## Links to Documentation
- [README.md] (../README.md) `@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/README.md`
- [CHANGELOG.md](../CHANGELOG.md) `@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/CHANGELOG.md`
- [Domain README.md](../SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/README.md) `@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/README.md`
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design Reference](https://www.domainlanguage.com/ddd/reference/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Serilog Documentation](https://serilog.net/)
- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/en/latest/)


- use context7 to retrieve the latest documentation
- ALWWAYS Maintain changelog.md file with version history if changes are worth mentioning
- ALLWAYS Review and update README.md file for project overview and document optimization if worth and follows the best practices
