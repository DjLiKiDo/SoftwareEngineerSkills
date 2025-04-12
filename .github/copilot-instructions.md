# Project Architecture and GitHub Copilot Guidance

## Table of Contents
1. [Purpose](#1-purpose)
2. [Key Principles](#2-key-principles)
   - [Core Principles & Goals](#core-principles--goals)
   - [Preferred Libraries](#preferred-libraries)
   - [Code Quality & Style](#code-quality--style)
3. [Architecture](#3-architecture)
   - [Domain Layer](#domain-layer-srcyoursolutiondomain)
   - [Application Layer](#application-layer-srcyoursolutionapplication)
   - [Infrastructure Layer](#infrastructure-layer-srcinfrastructureyoursolutioninfrastructure)
   - [API / Presentation Layer](#api--presentation-layer-srcyoursolutionapi)
4. [Core Patterns](#4-core-patterns)
   - [Clean Architecture](#clean-architecture)
   - [CQRS (Command Query Responsibility Segregation)](#cqrs-command-query-responsibility-segregation)
   - [Result Pattern](#result-pattern)
   - [Repository Pattern](#repository-pattern)
   - [Unit of Work Pattern](#unit-of-work-pattern)
   - [IOptions Pattern](#ioptions-pattern)
   - [Validation (FluentValidation)](#validation-fluentvalidation)
   - [Mapping (AutoMapper)](#mapping-automapper)
5. [Error Handling & Security](#5-error-handling--security)
   - [Error Handling](#error-handling)
   - [Security](#security)
6. [Testing](#6-testing)
   - [Unit Testing](#unit-testing)
7. [Documentation](#7-documentation)
8. [CI/CD & Collaboration](#8-cicd--collaboration)
   - [CI/CD Integration](#cicd-integration)
   - [Code Reviews and Pull Requests](#code-reviews-and-pull-requests)
9. [Azure Specific Rules](#9-azure-specific-rules)
10. [Quick Reference](#10-quick-reference)
    - [Command/Query Structure](#commandquery-structure)
    - [IOptions Pattern Example](#ioptions-pattern-example)
    - [Controller Pattern](#controller-pattern)

## 1. Purpose

This document outlines the core architectural principles, patterns, coding standards, and processes for this .NET 9 / C# 14 project. It serves as a guide for both human developers and GitHub Copilot to ensure the generation and modification of high-quality, consistent, maintainable, and scalable code, following Clean Architecture, CQRS, Screaming Architecture, and the IOptions pattern.

**Primary Framework:** .NET 9 / C# 14

## 2. Key Principles

### Core Principles & Goals

*   **Overarching Goals:** Separation of Concerns, Low Coupling, High Cohesion, Testability, Maintainability, Scalability.
*   **Clean Architecture:** Strictly adhere to the layer separation (detailed in Section 3).
*   **SOLID:** Apply SOLID principles rigorously throughout the codebase.
*   **CQRS (Command Query Responsibility Segregation):** Use MediatR to separate commands (write operations) from queries (read operations).
*   **Screaming Architecture:** Organize Application and API layers primarily by business feature/domain area (e.g., `/Features/Products/`, `/Controllers/ProductsController.cs`) rather than technical concerns.
*   **DDD (Core Concepts):** Utilize core Domain-Driven Design concepts like Entities, Value Objects, Domain Events, and the Repository pattern.
*   **Async/Await:** Use `async`/`await` **consistently** for all I/O-bound operations. Avoid blocking calls. Suffix async methods with `Async`.
*   **Dependency Injection (DI):** Leverage .NET's built-in DI container. Use constructor injection primarily. Register services in layer-specific `DependencyInjection.cs` files.
*   **Immutability:** Prefer immutable types (`record`) for DTOs, Commands, Queries, and Events.
*   **Nullability:** Enable and strictly adhere to C# nullable reference types (`#nullable enable`).

### Preferred Libraries

*   **Data Access:** Entity Framework Core
*   **CQRS:** MediatR (v12.5.0 or later)
*   **Mapping:** AutoMapper (v14.0.0 or later)
*   **Validation:** FluentValidation (v11.11.0 or later)
*   **Logging:** Serilog (preferred, configurable via `appsettings.json`)
*   **API Documentation:** Swashbuckle.AspNetCore (for OpenAPI/Swagger)
*   **Testing:** xUnit (v2.9.2 or later), Moq, FluentAssertions

### Code Quality & Style

*   **Language:** All code, comments, documentation **must be in English**.
*   **Standards:** Follow Microsoft's C# coding conventions and .NET best practices.
*   **Readability:** Prioritize clear, understandable, well-formatted code.
*   **Naming Conventions:** `PascalCase` (types, public members), `camelCase` (parameters, locals), `_camelCase` (private fields), `IMyInterface`.
*   **Comments:** Explain *why*, not *what*. Use XML documentation comments (`///`) for public APIs.
*   **Method Length:** Keep methods short and focused.

## 3. Architecture

### Domain Layer (`/src/YourSolution.Domain`)

*   **Contents:** Entities, Aggregates (if used), Value Objects, Domain Events (`INotification`), Repository/Domain Service Interfaces (in `/Abstractions/Persistence|Services`), Domain Enums, Custom Domain Exceptions.
*   **Dependencies:** None on other project layers. Minimal external dependencies.
*   **Entities:** Enforce validity via factory methods (`Create`). Encapsulate state changes. Private setters preferred.

### Application Layer (`/src/YourSolution.Application`)

*   **Contents:**
    *   `/Features/[FeatureName]/Commands/[CommandName]/Command.cs|Handler.cs|Validator.cs`
    *   `/Features/[FeatureName]/Queries/[QueryName]/Query.cs|Handler.cs|Response.cs`
    *   `/Features/[FeatureName]/DTOs/` (Shared DTOs for the feature)
    *   `/Common/Abstractions/Infrastructure/` (Interfaces like `IEmailSender`, `IDateTimeProvider`)
    *   `/Common/Abstractions/Presentation/` (Interfaces like `ICurrentUserService`)
    *   `/Common/Behaviours/` (MediatR pipelines: Logging, Validation, UnitOfWork, etc.)
    *   `/Common/Exceptions/` (Application-specific exceptions like `NotFoundException`)
    *   `/Common/Mappings/` (AutoMapper Profiles)
    *   `/Common/Models/` (Generic models like `PagedResult<T>`)
    *   `/Configuration/` (**IOptions class definitions** like `AuthenticationSettings.cs`)
*   **Dependencies:** Only on **Domain**.
*   **MediatR (CQRS):** Handlers contain use case logic. Use `record` for messages. Publish Domain Events via `IPublisher`.
*   **Result Pattern:** **Mandatory** return type for all Handlers (`Result` or `Result<T>`).
*   **Validation:** Use FluentValidation (`AbstractValidator<T>`). Handled by `ValidationBehavior`.
*   **Mapping:** Use AutoMapper (`Profile`, `IMapper`). Map Entities <-> DTOs.
*   **Data Access:** Use repository/UoW interfaces **only**.

### Infrastructure Layer (`/src/Infrastructure/YourSolution.Infrastructure`)

*   **Contents:**
    *   `/Persistence/DbContexts/ApplicationDbContext.cs`
    *   `/Persistence/Repositories/` (Implementations like `ProductRepository.cs`)
    *   `/Persistence/Configurations/` (EF Core `IEntityTypeConfiguration`)
    *   `/Persistence/Migrations/`
    *   `/Services/` (Implementations like `EmailSender.cs`)
    *   `/Authentication/` (JWT logic, Identity helpers)
    *   `/Configuration/Validators/` (**IOptions custom validation** `IValidateOptions<T>`)
    *   `DependencyInjection.cs`
*   **Dependencies:** On **Application** and **Domain**.
*   **EF Core:** Implement repository interfaces. Use `AppDbContext`. Use `IEntityTypeConfiguration`. Repositories **do not** call `SaveChangesAsync()`. Use `AsNoTracking()` for queries. Implement `IUnitOfWork` (often via `AppDbContext` or a dedicated behavior).
*   **Services:** Provide concrete implementations of Application infrastructure interfaces.

### API / Presentation Layer (`/src/YourSolution.API`)

*   **Contents:**
    *   `/Controllers/[FeatureName]Controller.cs` (MVC Controllers, organized by feature)
    *   `/Endpoints/` (Alternative for Minimal APIs, organized by feature)
    *   `/Middleware/` (Custom middleware like `ExceptionHandlerMiddleware`)
    *   `/Extensions/` (DI / Pipeline setup helpers)
    *   `/Filters/` (ASP.NET Core Action Filters)
    *   `appsettings.json` / `appsettings.Development.json` (**IOptions configuration source**)
    *   `Program.cs` (**IOptions registration/binding**, DI container setup, middleware pipeline)
*   **Dependencies:** On **Application** (via `ISender`) and **Infrastructure** (only for DI setup in `Program.cs`).
*   **Controllers:** Use **MVC Controllers**. Keep controllers **thin**. Inherit `ApiControllerBase`. Inject `ISender`. Dispatch Commands/Queries. Use `HandleResult()` helpers. Apply authorization attributes. Use `[ProducesResponseType]`.
*   **`Program.cs`:** Configure DI, logging, auth, middleware, IOptions binding (`builder.Services.AddOptions<T>().BindConfiguration(...)`).

## 4. Core Patterns

This section summarizes the key architectural and design patterns employed.

### Clean Architecture

*   **Implementation:** Enforced through strict layer separation:
    *   Domain (`YourSolution.Domain`)
    *   Application (`YourSolution.Application`)
    *   Infrastructure (`YourSolution.Infrastructure`)
    *   API / Presentation (`YourSolution.API`)
*   **Goal:** Separation of concerns, testability, maintainability. Dependencies flow inwards.

### CQRS (Command Query Responsibility Segregation)

*   **Implementation:** MediatR (v12.5.0 or later).
*   **Location:** Application layer (`/Features/[FeatureName]/Commands|Queries`).
*   **Goal:** Separate write operations (Commands) from read operations (Queries) for optimized handling and scalability.

### Result Pattern

*   **Implementation:** Custom `Result` and `Result<T>` types (e.g., in `Domain.Common` or a shared kernel).
*   **Usage:** **Mandatory** return type for all Application layer handlers (Commands and Queries).
*   **Goal:** Explicitly communicate the outcome (success or failure with errors) without relying on exceptions for control flow.

### Repository Pattern

*   **Interfaces:** Defined in `Domain/Abstractions/Persistence`.
*   **Implementations:** Located in `Infrastructure/Persistence/Repositories`.
*   **Goal:** Abstract data access logic, decoupling the Application layer from specific data storage technologies (like EF Core) and facilitating unit testing via mocks.

### Unit of Work Pattern

*   **Implementation:** Often implicitly handled by EF Core's `DbContext.SaveChangesAsync()` within a transaction, potentially orchestrated by a MediatR pipeline behavior (`UnitOfWorkBehavior`). An explicit `IUnitOfWork` interface (defined in Domain, implemented in Infrastructure) can also be used.
*   **Goal:** Ensure atomicity for operations involving multiple repository actions within a single command handler.

### IOptions Pattern

*   **Definition:** Configuration classes defined in `Application/Configuration`.
*   **Binding:** Configuration values (from `appsettings.json`, environment variables, etc.) are bound in `API/Program.cs` using `builder.Services.AddOptions<T>().BindConfiguration("SectionName")`.
*   **Validation:** Use DataAnnotations on the configuration classes (validated via `.ValidateDataAnnotations()`) and/or custom `IValidateOptions<T>` implementations (registered in Infrastructure, validated via `.ValidateOnStart()` or similar).
*   **Usage:** Inject `IOptions<T>` or `IOptionsMonitor<T>` into services/handlers that require configuration.
*   **Goal:** Provide strongly-typed access to application configuration settings.

### Validation (FluentValidation)

*   **Implementation:** FluentValidation (v11.11.0 or later).
*   **Location:** Validators (`AbstractValidator<T>`) defined in the Application layer, typically alongside the Command/Query they validate (`/Features/[FeatureName]/Commands/[CommandName]/Validator.cs`).
*   **Integration:** Integrated into the MediatR pipeline via a `ValidationBehavior`.
*   **Goal:** Provide declarative, robust, and testable input validation rules.

### Mapping (AutoMapper)

*   **Implementation:** AutoMapper (v14.0.0 or later).
*   **Location:** Mapping profiles (`Profile`) defined in `Application/Common/Mappings`.
*   **Usage:** Convert between different object types, primarily Entities <-> DTOs. Inject `IMapper`.
*   **Goal:** Simplify and centralize object-to-object mapping logic.

## 5. Error Handling & Security

### Error Handling

*   **Middleware:** Implement global exception handling middleware (e.g., implementing `IExceptionHandler` in .NET 8+ or custom middleware).
*   **Responses:** Use standardized `ProblemDetails` (RFC 7807) for all HTTP API errors. Map exceptions or `Result` failures to `ProblemDetails`.
*   **Custom Exceptions:** Create specific exception types in Domain or Application layers where necessary (e.g., `DomainException`, `NotFoundException`), but prefer the Result pattern for expected business rule failures or validation errors.

### Security

*   **Authentication:** Use JWT Bearer Tokens via ASP.NET Core Identity / JWT middleware. Configure in `Program.cs`.
*   **Authorization:** Apply `[Authorize]` attribute on controllers/endpoints. Use policies (`[Authorize(Policy = ...)]`) for role- or claim-based access control. Define policies in `Program.cs`.
*   **Input Validation:** Mandatory. Handled by FluentValidation pipeline behavior and potentially domain entity invariants.
*   **Secrets Management:** **NEVER** commit secrets to source control. Use User Secrets (development), Azure Key Vault, environment variables, or other secure configuration providers.
*   **Logging:** Avoid logging sensitive data (PII, credentials, secrets). Configure logging filters if necessary.
*   **OWASP Top 10:** Be mindful of common web vulnerabilities and apply mitigation strategies.

## 6. Testing

### Unit Testing

*   **Tools:** xUnit (v2.9.2 or later), Moq (or similar mocking framework like NSubstitute), FluentAssertions.
*   **Focus:** Application layer handlers (Commands/Queries), Domain entities/value objects/logic.
*   **Mocks:** Mock all external dependencies (repositories, services, `IDateTimeProvider`, `IMapper`, etc.) using interfaces defined in Domain or Application layers.
*   **Structure:** Arrange-Act-Assert (AAA).
*   **Assertions:** Verify the returned `Result` (IsSuccess/IsFailure, Value, Error). Verify interactions with mocks (`Verify()`). Use FluentAssertions for readable assertions.
*   **Coverage:** Test success paths, failure paths (validation errors, exceptions, business rule failures), and edge cases. **Include relevant tests with all pull requests.**
*   **Scalability:** For larger projects, consider separating tests by type (Unit, Integration, Acceptance) into different projects or folders. Integration tests might use tools like `Testcontainers` or `WebApplicationFactory`.

## 7. Documentation

*   **API:** Generate automatic API documentation using Swashbuckle/OpenAPI. Ensure controllers and DTOs have appropriate XML comments (`///`) and attributes (`[ProducesResponseType]`) for rich documentation.
*   **Code:** Document all public API members (classes, methods, properties) using XML comments (`///`). Explain complex logic or non-obvious decisions with inline comments (`//`).
*   **README:** Maintain a project `README.md` with setup instructions, architectural overview, and contribution guidelines.

## 8. CI/CD & Collaboration

### CI/CD Integration

*   **CI Pipeline (e.g., GitHub Actions):** Trigger on push/PR to development branches.
    *   Steps: Restore dependencies -> Build solution -> Run unit tests -> Perform static code analysis (optional).
    *   Goal: Validate code quality and prevent regressions before merging. Enforce PR checks.
*   **CD Pipeline (e.g., GitHub Actions):** Trigger on merge to main/release branches.
    *   Steps: Build release artifacts -> Deploy to Azure environments (Dev, Staging, Prod).
    *   Goal: Automate deployment process. Follow Azure best practices (e.g., deployment slots, infrastructure as code, approval gates).

### Code Reviews and Pull Requests

*   **Scope:** Keep Pull Requests (PRs) small and focused on a single feature, bugfix, or refactoring task.
*   **Documentation:** Provide a clear PR description, linking to the relevant issue/ticket. Explain the "what" and "why" of the changes.
*   **Testing:** Ensure PRs include necessary unit tests (and integration tests where applicable) for the changes made. Verify tests pass in the CI pipeline.
*   **Review Checklist:** Reviewers should check for:
    *   Adherence to coding standards and architectural principles (this document).
    *   Correctness and completeness of the implementation.
    *   Adequate test coverage.
    *   Potential performance or security issues.
    *   Readability and maintainability.
    *   Clear documentation (code comments, PR description).

## 9. Azure Specific Rules

*   **Azure Best Practices:** When generating code specifically for Azure services (e.g., Azure Functions, App Service configuration, Key Vault integration, Service Bus), generating Azure CLI or PowerShell commands, or suggesting Azure deployment strategies, strictly adhere to current Azure best practices and documentation.
*   **Infrastructure as Code (IaC):** Prefer IaC tools like Bicep or Terraform for managing Azure resources.
*   **Managed Identities:** Use Managed Identities for Azure resource authentication wherever possible, avoiding connection strings or keys in configuration.
*   **Azure SDK:** Utilize the latest stable Azure SDKs for .NET.

## 10. Quick Reference

### Command/Query Structure
```csharp
// Command (in Application/Features/...)
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<Guid>>;

// Command Handler (in Application/Features/...)
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    // Inject dependencies (repository, unit of work, etc.)
    public CreateProductCommandHandler(/*...dependencies...*/) { /*...*/ }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate (often done by pipeline behavior)
        // 2. Create Domain Entity
        var productResult = Product.Create(request.Name, request.Price);
        if (productResult.IsFailure) return Result.Failure<Guid>(productResult.Error);
        var product = productResult.Value;

        // 3. Add to Repository
        _repository.Add(product);

        // 4. Save Changes (often done by pipeline behavior or explicit UoW call)
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Return Result
        return Result.Success(product.Id);
    }
}

// Query (in Application/Features/...)
public record GetProductQuery(Guid Id) : IRequest<Result<ProductDto>>;

// Query Response DTO (in Application/Features/.../DTOs or Common/Models)
public record ProductDto(Guid Id, string Name, decimal Price);

// Query Handler (in Application/Features/...)
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    // Inject dependencies (repository/DbContext, mapper)
    public GetProductQueryHandler(/*...dependencies...*/) { /*...*/ }

    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        // 1. Retrieve data (use AsNoTracking for queries)
        var product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>(Domain.Errors.ProductErrors.NotFound(request.Id)); // Example error
        }

        // 2. Map to DTO
        var productDto = _mapper.Map<ProductDto>(product);

        // 3. Return Result
        return Result.Success(productDto);
    }
}