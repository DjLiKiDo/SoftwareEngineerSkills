# Project Architecture and GitHub Copilot Guidance (copilot-instructions.md)

## 1. Purpose

This document outlines the core architectural principles, patterns, coding standards, and processes for this .NET 9 / C# 14 project. It serves as a guide for both human developers and GitHub Copilot to ensure the generation and modification of high-quality, consistent, maintainable, and scalable code, following Clean Architecture, CQRS, Screaming Architecture, and the IOptions pattern.

**Primary Framework:** .NET 9 / C# 14

## 2. Key Principles

### Core Principles & Goals

*   **Overarching Goals:** Separation of Concerns, Low Coupling, High Cohesion, Testability, Maintainability, Scalability.
*   **Clean Architecture:** Strictly adhere to the layer separation:
    *   **Domain:** Business core, entities, value objects, domain events, repository/domain service interfaces. No external dependencies (except base .NET/common libs). Resides typically in `/src/YourSolution.Domain`.
    *   **Application:** Use cases, command/query handlers, DTOs, application logic, infrastructure interfaces (e.g., `IEmailSender`). Depends *only* on Domain. Includes IOptions class definitions. Resides typically in `/src/YourSolution.Application`.
    *   **Infrastructure:** Implementation details (database access, email sending, auth services, external APIs). Depends on Application (to implement its interfaces). Includes IOptions custom validators. Resides typically in `/src/Infrastructure/YourSolution.Infrastructure`.
    *   **API / Presentation:** Entry point (ASP.NET Core MVC Controllers), middleware, DI setup, configuration source. Depends on Application (for MediatR) and Infrastructure (for DI setup). Resides typically in `/src/YourSolution.API`.
    *   **Dependency Rule:** Dependencies flow inwards: `API` -> `Application` -> `Domain`. `Infrastructure` -> `Application`.
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
*   **CQRS:** MediatR
*   **Mapping:** AutoMapper
*   **Validation:** FluentValidation
*   **Logging:** Serilog (preferred, configurable via `appsettings.json`)
*   **API Documentation:** Swashbuckle.AspNetCore (for OpenAPI/Swagger)

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
    *   `/Common/Behaviours/` (MediatR pipelines: Logging, Validation, etc.)
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
*   **EF Core:** Implement repository interfaces. Use `AppDbContext`. Use `IEntityTypeConfiguration`. Repositories **do not** call `SaveChangesAsync()`. Use `AsNoTracking()` for queries. Implement `IUnitOfWork` (often via `AppDbContext`).
*   **Services:** Provide concrete implementations.

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

*   **Result Pattern:** Standard for Application returns. Explicit success/failure. Use `Domain.Common.Error`.
*   **MediatR (CQRS):** Enforces separation. Single responsibility handlers. `INotification` for events.
*   **Repository Pattern:** Abstract persistence. Clear interfaces (Domain). Implementations (Infrastructure).
*   **Unit of Work Pattern:** Ensure command atomicity (via behavior or explicit `IUnitOfWork.SaveChangesAsync()`). Interface (Domain), Implementation (Infrastructure).
*   **IOptions Pattern:**
    *   **Definition:** Classes defined in `Application/Configuration`.
    *   **Source:** `appsettings.json` (and environment variants) in API project.
    *   **Registration/Binding:** Done in `API/Program.cs` using `builder.Services.AddOptions<T>().BindConfiguration("SectionName").ValidateDataAnnotations()...`.
    *   **Validation:** DataAnnotations (on classes in Application), `IValidateOptions<T>` (custom validation logic in Infrastructure).
    *   **Usage:** Inject `IOptions<T>` or `IOptionsMonitor<T>` where needed (Handlers, Services).
*   **Validation (FluentValidation):** Declarative rules (`AbstractValidator<T>` in Application), integrated via MediatR pipeline.
*   **Mapping (AutoMapper):** Simplify object mapping (`Profile` in Application, inject `IMapper`).

## 5. Error Handling & Security

### Error Handling

*   **Middleware:** Implement global exception handling middleware (`IExceptionHandler`).
*   **Responses:** Use standardized `ProblemDetails` for all HTTP errors.
*   **Custom Exceptions:** Create specific exception types where necessary (but prefer Result pattern for expected failures).

### Security

*   **Authentication:** Use JWT Bearer Tokens via ASP.NET Core Identity / JWT middleware.
*   **Authorization:** Apply `[Authorize]` attribute. Use policies (`[Authorize(Policy = ...)]`).
*   **Input Validation:** Mandatory (FluentValidation, Domain/Handler checks).
*   **Secrets Management:** **NEVER** commit secrets. Use User Secrets (dev), Azure Key Vault, or equivalent.
*   **Logging:** Avoid logging sensitive data.

## 6. Testing

*   **Tools:** xUnit, Moq, FluentAssertions.
*   **Focus:** Application layer handlers, Domain entities/logic.
*   **Mocks:** Mock all external dependencies.
*   **Structure:** Arrange-Act-Assert (AAA).
*   **Assertions:** Verify `Result`. Verify mock interactions (`Verify()`).
*   **Coverage:** Test success/failure paths, edge cases. **Include tests with pull requests.**
*   **Scalability:** In large projects, include not only unit tests but also integration, acceptance, and performance tests. Use a separate solution or folder structure to manage these different types of tests more effectively.

## 7. Documentation

*   **API:** Generate automatic API documentation using Swashbuckle/OpenAPI.
*   **Code:** Document all public API endpoints and complex logic using XML comments (`///`).

## 8. CI/CD & Collaboration

### CI/CD Integration

*   **CI Pipeline (GitHub Actions):** Trigger on push/PR. Restore -> Build -> Test -> Analyze. Validate PRs before merging.
*   **CD Pipeline (GitHub Actions):** Trigger on merge (main/release). Build artifacts -> Deploy to Azure environments (Dev, Staging, Prod) following best practices (slots, approvals).

### Code Reviews and Pull Requests

*   **Scope:** Keep PRs small and focused (single feature/bugfix).
*   **Documentation:** Clear PR description, link issues.
*   **Testing:** Ensure PRs include relevant unit/integration tests.
*   **Review Checklist:** Adherence to standards (this doc), documentation, test coverage, performance/security, architectural compliance (Clean Arch, SOLID, CQRS, Repo/UoW).

## 9. Azure Specific Rules

*   `@azure Rule` - Use Azure Best Practices: When generating code for Azure, running terminal commands for Azure, or performing operations related to Azure, invoke your `get_azure_best_practices` tool if available.

- @azure Rule - Use Azure Best Practices: When generating code for Azure, running terminal commands for Azure, or performing operations related to Azure, invoke your `get_azure_best_practices` tool if available.
