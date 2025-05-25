---
applyTo: "**/*.cs"
---

# Code Generation Standards and Rules for SoftwareEngineerSkills Project

This instruction file provides comprehensive guidelines for GitHub Copilot when generating C# code for the SoftwareEngineerSkills .NET 9 enterprise project following Clean Architecture and Domain-Driven Design principles.

## Project Context

**Architecture Pattern:** Clean Architecture with Domain-Driven Design
**Framework:** .NET 9 Enterprise Application
**Domain Modeling:** Rich domain model with aggregates, entities, value objects, and domain events

### Architectural Standards

### Clean Architecture Layers
1. **Domain Layer**: Core business logic, entities, value objects, domain events
2. **Application Layer**: Use cases, CQRS commands/queries, application services
3. **Infrastructure Layer**: Data access, external services, persistence
4. **API Layer**: Controllers, middleware, configuration

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
- **Classes/Interfaces/Methods**: PascalCase (`EntityService`, `IRepository`)
- **Properties/Public Fields**: PascalCase (`PropertyName`, `CreatedDate`)
- **Private Fields**: _camelCase with underscore prefix (`_privateField`, `_unitOfWork`)
- **Local Variables/Parameters**: camelCase (`localVariable`, `methodParameter`)
- **Constants**: PascalCase (`MaxItemCount`)
- **Interfaces**: Prefix with 'I' (`IEntityRepository`, `IAggregateRoot`)

### Domain Layer Guidelines

#### Entity Implementation Standards
- Extend `AggregateRoot` for main business entities requiring thread-safe event handling
- Use private setters for properties that should only be changed through business methods
- Implement parameterless constructor for EF Core and public constructor with validation
- Expose collections as `IReadOnlyCollection` with private backing fields
- Generate domain events for significant state changes
- Implement invariant validation in `CheckInvariants()` method
- Call `EnforceInvariants()` after state modifications

#### Value Object Implementation Standards
- Extend `ValueObject` base class for immutable concepts without identity
- Implement `GetEqualityComponents()` for proper equality semantics
- Validate all inputs in constructor with appropriate guard clauses
- Use factory methods for complex creation scenarios
{
    public PropertyType PropertyName { get; private set; }
    public AnotherType AnotherProperty { get; private set; }
    public DateTime CreatedDate { get; private set; }
    
    private ValueObjectName() { } // EF Core
    
    public ValueObjectName(PropertyType propertyName, AnotherType anotherProperty, DateTime createdDate)
    {
        PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        AnotherProperty = anotherProperty;
        CreatedDate = createdDate;
        
        if (createdDate > DateTime.UtcNow)
            throw new BusinessRuleException("Creation date cannot be in the future");
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PropertyName;
        yield return AnotherProperty;
        yield return CreatedDate.Date;
    }
}
```

### Application Layer Code Generation

#### CQRS Command Pattern
```csharp
public record CreateEntityCommand : IRequest<Result<EntityDto>>
{
    public required string PropertyName { get; init; }
    public required string Description { get; init; }
    public EntityPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public List<RelatedRequirementDto> Requirements { get; init; } = [];
}

### Application Layer Guidelines

#### Command Handler Implementation Standards
- Implement `IRequestHandler<TCommand, Result<TResponse>>` interface
- Use dependency injection for repositories, mappers, and services
- Return `Result<T>` pattern for success/failure scenarios
- Validate inputs using FluentValidation
- Handle domain exceptions and convert to appropriate result types

#### Query Handler Implementation Standards  
- Implement `IRequestHandler<TQuery, Result<TResponse>>` interface
- Use read-only repository interfaces for data retrieval
- Apply projection and filtering at the database level
- Return mapped DTOs rather than domain entities
- Implement caching strategies for frequently accessed data

### API Layer Guidelines

#### Controller Implementation Standards
- Apply `[ApiController]` attribute and proper routing conventions
- Use `IMediator` for command and query dispatch
- Return appropriate HTTP status codes and response types
- Implement proper request validation and error handling
- Document API endpoints with OpenAPI attributes
- Apply authentication and authorization policies consistently

### Testing Guidelines

#### Unit Test Implementation Standards
- Follow Arrange-Act-Assert pattern for test structure
- Use descriptive test method names following convention patterns
- Mock all external dependencies using appropriate mocking frameworks
- Assert both success and failure scenarios using fluent assertions
- Verify mock interactions and state changes appropriately
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
