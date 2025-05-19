# GitHub Copilot Instructions for .NET 9 Enterprise Web API Template

This document provides instructions for GitHub Copilot to optimize AI assistance for this project.

## 1. Project Definition

### Primary Domain and Tech Stack
- **Primary Domain:** Enterprise Web API development using .NET 9
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
namespace Domain.Entities;

public class Customer : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public Email EmailAddress { get; private set; }
    public Address ShippingAddress { get; private set; }
    
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    
    private Customer() { } // For EF Core
    
    public Customer(string name, Email email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EmailAddress = email ?? throw new ArgumentNullException(nameof(email));
    }
    
    public void UpdateShippingAddress(Address address)
    {
        ShippingAddress = address ?? throw new ArgumentNullException(nameof(address));
        AddDomainEvent(new CustomerAddressUpdatedEvent(Id, address));
    }
    
    public void AddOrder(Order order)
    {
        _orders.Add(order);
        AddDomainEvent(new OrderAddedEvent(Id, order.Id));
    }
}
```

#### Value Objects
```csharp
namespace Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }
    
    private Email() { } // For EF Core
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));
            
        if (!IsValidEmail(value))
            throw new BusinessRuleException("Invalid email format");
            
        Value = value;
    }
    
    private bool IsValidEmail(string email)
    {
        // Email validation logic
        return true; // Simplified for example
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }
    
    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string email) => new(email);
}
```

#### CQRS Commands and Queries
```csharp
// Command
namespace Application.Customers.Commands;

public record CreateCustomerCommand : IRequest<Result<CustomerDto>>
{
    public string Name { get; init; }
    public string Email { get; init; }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var email = new Email(request.Email);
            var customer = new Customer(request.Name, email);
            
            await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(_mapper.Map<CustomerDto>(customer));
        }
        catch (BusinessRuleException ex)
        {
            return Result.Failure<CustomerDto>(ex.Message);
        }
    }
}

// Query
namespace Application.Customers.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IReadRepository<Customer> _repository;
    private readonly IMapper _mapper;
    
    public GetCustomerByIdQueryHandler(IReadRepository<Customer> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (customer == null)
            return Result.Failure<CustomerDto>($"Customer with ID {request.Id} not found.");
            
        return Result.Success(_mapper.Map<CustomerDto>(customer));
    }
}
```

#### API Controllers
```csharp
namespace WebApi.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id));
        
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer(CreateCustomerCommand command)
    {
        var result = await _mediator.Send(command);
        
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCustomerById), new { id = result.Value.Id }, result.Value)
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
public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCustomerCommandHandler _handler;
    
    public CreateCustomerCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _unitOfWorkMock.Setup(uow => uow.Customers).Returns(_customerRepositoryMock.Object);
        
        _handler = new CreateCustomerCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCustomer()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = "Test Customer",
            Email = "test@example.com"
        };
        
        var customerDto = new CustomerDto { Id = Guid.NewGuid(), Name = command.Name, Email = command.Email };
        
        _mapperMock
            .Setup(m => m.Map<CustomerDto>(It.IsAny<Customer>()))
            .Returns(customerDto);
            
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(customerDto);
        
        _customerRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Once);
            
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
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
- ALWWAYS Maintain changelog.md file with version history
- ALLWAYS Review and update README.md file for project overview if needed
