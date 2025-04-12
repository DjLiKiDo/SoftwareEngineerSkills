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
   - [Integration Testing (Considerations)](#integration-testing-considerations)
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

This document outlines the core architectural principles, patterns, coding standards, and processes for this .NET 9 / C# 14 project. It serves as a definitive guide for **all team members (developers, leads, architects)** and **GitHub Copilot** to ensure the generation and modification of high-quality, consistent, maintainable, and scalable code. The architecture follows Clean Architecture principles, CQRS, Screaming Architecture, and leverages patterns like the Result Pattern and IOptions for robust and modern application development.

**Primary Framework:** .NET 9 / C# 14 (targeting latest stable releases)

## 2. Key Principles

### Core Principles & Goals

*   **Overarching Goals:** Separation of Concerns, Low Coupling, High Cohesion, Testability, Maintainability, Scalability, Understandability.
*   **Clean Architecture:** Strictly adhere to the layer separation and dependency rules (Dependencies flow inwards: API -> Application -> Domain; Infrastructure -> Application -> Domain). See Section 3.
*   **SOLID:** Apply SOLID principles rigorously throughout the codebase.
*   **CQRS (Command Query Responsibility Segregation):** Use MediatR to separate commands (write operations causing state change) from queries (read operations retrieving data).
*   **Screaming Architecture:** Organize Application and API layers primarily by business feature/domain area (e.g., `/Features/Orders/`, `/Controllers/OrdersController.cs`) rather than technical concerns (e.g., `/Services/`, `/Repositories/`). This makes the application's purpose evident from its structure.
*   **DDD (Core Concepts):** Utilize core Domain-Driven Design concepts like Entities, Value Objects, Aggregates (where appropriate), Domain Events, and the Repository pattern within the Domain layer. Protect domain invariants.
*   **Async/Await:** Use `async`/`await` **consistently and correctly** for all I/O-bound operations (database access, network calls, file system access). Avoid `async void` (except for event handlers). Avoid blocking calls (`.Result`, `.Wait()`). Suffix all genuinely asynchronous methods with `Async`.
*   **Dependency Injection (DI):** Leverage .NET's built-in DI container exclusively. Use **constructor injection** as the primary method for resolving dependencies. Register services in layer-specific `DependencyInjection.cs` (or similar extension methods) for better organization. Avoid service locators.
*   **Immutability:** Prefer immutable types (`record` classes/structs) for DTOs, Commands, Queries, Events, and Value Objects. Use `init` setters where appropriate.
*   **Nullability:** Enable and **strictly adhere** to C# nullable reference types (`#nullable enable` at the project level). Eliminate null warnings and design APIs to clearly express nullability intent. Avoid nullable warnings suppression (`!`) unless absolutely necessary and justified.
*   **Avoid Static:** Minimize the use of static classes and methods for application logic or state management to improve testability. Prefer dependency-injected services. Static helper methods for pure functions are acceptable.

### Preferred Libraries

*   **Data Access:** Entity Framework Core (latest stable version for .NET 9)
*   **CQRS:** MediatR (v12.5.0 or later stable version)
*   **Mapping:** AutoMapper (v14.0.0 or later stable version)
*   **Validation:** FluentValidation (v11.11.0 or later stable version)
*   **Logging:** Serilog (preferred, highly configurable via `appsettings.json` and code) or standard `Microsoft.Extensions.Logging`.
*   **API Documentation:** Swashbuckle.AspNetCore (for OpenAPI/Swagger generation)
*   **Testing:** xUnit (v2.9.2 or later stable version), Moq (or NSubstitute), FluentAssertions.

### Code Quality & Style

*   **Language:** All code, comments, documentation, commit messages, and PR descriptions **must be in English**.
*   **Standards:** Follow Microsoft's C# coding conventions and .NET runtime/framework design guidelines. Utilize features from C# 14 where appropriate and beneficial for clarity/conciseness (e.g., primary constructors, collection expressions).
*   **Readability:** Prioritize clear, self-documenting, and well-formatted code. Use meaningful names.
*   **Naming Conventions:** `PascalCase` (types, public members, methods), `camelCase` (parameters, local variables), `_camelCase` (private instance fields), `IMyInterface` (interfaces). Constants can be `PascalCase` or `UPPER_SNAKE_CASE`.
*   **Comments:** Explain *why* something is done a certain way, not *what* the code does (the code should explain the 'what'). Use XML documentation comments (`///`) extensively for all public types and members.
*   **Method Length:** Keep methods short, focused, and adhering to the Single Responsibility Principle. Aim for methods that fit on one screen.
*   **File Length:** Keep files focused. Avoid excessively long files.
*   **Avoid Magic Strings/Numbers:** Use constants (`const` or `static readonly`), enums, or `nameof()` instead of hardcoded string or numeric literals where appropriate, especially for configuration keys, route templates, policy names, etc.

## 3. Architecture

### Domain Layer (`/src/YourSolution.Domain`)

*   **Contents:**
    *   `/Entities/` (e.g., `Order.cs`, `Product.cs`)
    *   `/ValueObjects/` (e.g., `Address.cs`, `Money.cs`)
    *   `/Aggregates/` (Root entities coordinating transactions and invariants, if using explicit Aggregates)
    *   `/Events/` (Domain Events, implementing `MediatR.INotification`, e.g., `OrderCreatedEvent.cs`)
    *   `/Enums/` (Domain-specific enumerations)
    *   `/Errors/` (Static classes defining domain-specific `Error` types for the Result pattern, e.g., `ProductErrors.cs`)
    *   `/Exceptions/` (Custom domain-specific exception types, e.g., `InvalidDomainStateException.cs`. Use sparingly; prefer `Result` for predictable failures.)
    *   `/Abstractions/Persistence/` (Repository interfaces, e.g., `IOrderRepository.cs`, `IUnitOfWork.cs`)
    *   `/Abstractions/Services/` (Domain Service interfaces, if complex logic spans multiple entities)
*   **Dependencies:** **None** on other project layers (Application, Infrastructure, API). Minimal external dependencies (e.g., potentially MediatR for `INotification`).
*   **Core:** Contains the core business logic, rules, and data structures. Completely independent of UI, databases, or external services.
*   **Entities:** Enforce validity through constructors or factory methods (`public static Result<Entity> Create(...)`). Encapsulate state changes within methods. Private setters preferred; use `init` where suitable for construction. Protect invariants rigorously.
*   **Domain Events:** Represent significant occurrences within the domain.

### Application Layer (`/src/YourSolution.Application`)

*   **Contents:**
    *   `/Features/[FeatureName]/Commands/[CommandName]/` (`Command.cs`, `Handler.cs`, `Validator.cs`)
    *   `/Features/[FeatureName]/Queries/[QueryName]/` (`Query.cs`, `Handler.cs`, `Response.cs` or `DTO.cs`)
    *   `/Features/[FeatureName]/DTOs/` (Shared Data Transfer Objects specific to the feature, use `record`)
    *   `/Common/Abstractions/Infrastructure/` (Interfaces for infrastructure concerns needed by the application, e.g., `IEmailSender.cs`, `IDateTimeProvider.cs`, `ICacheService.cs`)
    *   `/Common/Abstractions/Presentation/` (Interfaces for presentation concerns needed by the application, e.g., `ICurrentUserService.cs`)
    *   `/Common/Behaviours/` (MediatR pipeline behaviors: Logging, Validation, UnitOfWork, Performance Monitoring, Caching, etc.)
    *   `/Common/Exceptions/` (Application-specific exceptions, e.g., `NotFoundException.cs`, `ValidationException.cs`. Often used by middleware.)
    *   `/Common/Mappings/` (AutoMapper Profiles and mapping configurations, e.g., `ProductProfile.cs`, `MappingExtensions.cs`)
    *   `/Common/Models/` (Generic models used across features, e.g., `PagedResult<T>`, `Result.cs`/`Result<T>.cs` - if not in Domain or Shared Kernel)
    *   `/Configuration/` (**IOptions class definitions**, plain C# objects representing configuration sections, e.g., `JwtSettings.cs`, `EmailSettings.cs`)
    *   `DependencyInjection.cs` (Extension method to register Application layer services)
*   **Dependencies:** Only depends on **Domain**. **No** dependencies on Infrastructure or API layers.
*   **Orchestration:** Contains application use cases (Commands/Queries). Orchestrates calls to domain objects and infrastructure services (via abstractions).
*   **MediatR (CQRS):** Command/Query handlers encapsulate the logic for a single use case. Use `record` types for Command/Query messages and DTOs. Handlers interact with Domain entities/repositories and publish Domain Events using MediatR's `IPublisher`.
*   **Result Pattern:** **Mandatory** return type for all Command and Query Handlers (`Result` or `Result<T>`). Clearly signals success or specific failures without using exceptions for control flow.
*   **Validation:** Use FluentValidation (`AbstractValidator<T>`). Validation rules defined alongside the Command/Query. Validation is typically executed automatically via a MediatR `ValidationBehavior`.
*   **Mapping:** Use AutoMapper (`Profile`, `IMapper`). Map between Domain Entities and Application DTOs. Inject `IMapper` where needed.
*   **Data Access:** Interact with data **only** through repository and Unit of Work interfaces defined in the Domain layer.

### Infrastructure Layer (`/src/Infrastructure/YourSolution.Infrastructure`)

*   **Contents:**
    *   `/Persistence/DbContexts/` (`ApplicationDbContext.cs`)
    *   `/Persistence/Repositories/` (Implementations of Domain repository interfaces, e.g., `ProductRepository.cs`)
    *   `/Persistence/Configurations/` (EF Core `IEntityTypeConfiguration<TEntity>` implementations for entity mapping)
    *   `/Persistence/Migrations/` (EF Core database migrations)
    *   `/Persistence/` (`UnitOfWork.cs` - Implementation of `IUnitOfWork`, often wrapping `DbContext.SaveChangesAsync`)
    *   `/Services/` (Implementations of Application infrastructure interfaces, e.g., `EmailSender.cs`, `SystemDateTimeProvider.cs`)
    *   `/Authentication/` (JWT generation/validation logic, Identity integration helpers, implementations of `ICurrentUserService`)
    *   `/Caching/` (Implementation of `ICacheService` using Redis, MemoryCache etc.)
    *   `/Configuration/Validators/` (**IOptions custom validation** using `IValidateOptions<T>` implementations, e.g., `JwtSettingsValidator.cs`)
    *   `DependencyInjection.cs` (Extension method to register Infrastructure layer services, DbContext, repositories, etc.)
*   **Dependencies:** Depends on **Application** (to implement its abstractions) and **Domain**. May depend on external libraries (EF Core, Azure SDKs, SendGrid, etc.).
*   **Implementation Details:** Contains all external concerns: data access implementation, external service clients (email, blob storage), authentication mechanisms, logging sinks, caching providers, etc.
*   **EF Core:** Implement repository interfaces using `ApplicationDbContext`. Use `IEntityTypeConfiguration` for clean entity mapping. Repositories **do not** call `SaveChangesAsync()`; this is the responsibility of the Unit of Work (often triggered by a MediatR behavior after a command handler). Use `AsNoTracking()` extensively in query implementations for performance.
*   **Services:** Provide concrete implementations of interfaces defined in `Application/Common/Abstractions/Infrastructure`.
*   **IOptions Validation:** Implement `IValidateOptions<T>` for complex configuration validation rules beyond simple data annotations.

### API / Presentation Layer (`/src/YourSolution.API`)

*   **Contents:**
    *   `/Controllers/` ([FeatureName]Controller.cs - MVC Controllers organized by feature, inheriting from a common `ApiControllerBase`)
    *   `/Endpoints/` (Alternative: Minimal API endpoints organized by feature/tag, using `MapGroup`)
    *   `/Middleware/` (Custom ASP.NET Core middleware, e.g., `ExceptionHandlerMiddleware`, `RequestContextLoggingMiddleware`)
    *   `/Extensions/` (Helper extension methods for `IServiceCollection` or `IApplicationBuilder` specific to API setup)
    *   `/Filters/` (ASP.NET Core Action Filters or Endpoint Filters, if needed)
    *   `/Common/` (Base controllers like `ApiControllerBase.cs`, helper methods for handling `Result` pattern)
    *   `appsettings.json`, `appsettings.Development.json`, etc. (**IOptions configuration source**)
    *   `Program.cs` (**Entry point:** DI container setup, middleware pipeline configuration, logging setup, authentication/authorization configuration, **IOptions registration/binding** (`builder.Services.AddOptions<T>().BindConfiguration(...)`), health checks)
*   **Dependencies:** Depends on **Application** (primarily via `MediatR.ISender`) and **Infrastructure** (only for DI setup in `Program.cs` or API-specific extensions). **Must not** depend directly on Domain.
*   **User Interface:** Handles incoming HTTP requests, routes them to the appropriate Application layer Command/Query via MediatR, and formats the response (e.g., JSON).
*   **Controllers/Endpoints:** Keep controllers **thin**. Their primary responsibility is request/response handling, authentication/authorization checks, and dispatching requests to MediatR (`ISender.Send(...)`). Use a shared base controller (`ApiControllerBase`) for common functionality (like handling `Result` objects). Map `Result` failures to appropriate `ProblemDetails` HTTP responses. Apply authorization attributes (`[Authorize]`). Use `[ProducesResponseType]` attributes extensively for OpenAPI documentation.
*   **`Program.cs`:** Centralized configuration for the web host. Configure services from all layers using their respective `DependencyInjection` extensions. Configure the HTTP request pipeline (middleware order matters). Bind configuration sections to `IOptions` classes.

## 4. Core Patterns

### Clean Architecture

*   **Implementation:** Enforced through strict project references and layer separation as described in Section 3.
*   **Goal:** Create a system that is independent of frameworks, UI, databases, and external agencies. Enhances testability, maintainability, and flexibility by enforcing separation of concerns and the Dependency Rule (dependencies flow inwards).

### CQRS (Command Query Responsibility Segregation)

*   **Implementation:** MediatR (v12.5.0 or later). Commands (`IRequest<Result...>`, handled by `IRequestHandler<,>`) change state. Queries (`IRequest<Result<TResponse>>`, handled by `IRequestHandler<,>`) retrieve state.
*   **Location:** Application layer (`/Features/[FeatureName]/Commands|Queries`).
*   **Goal:** Separate write/command operations from read/query operations. This allows optimizing each path independently (e.g., different data models, different performance strategies, potentially different databases eventually). Simplifies complex logic by focusing handlers on a single responsibility.

### Result Pattern

*   **Implementation:** Custom `Result` and `Result<T>` types (e.g., defined in `Domain.Common` or `Application.Common.Models`). Includes status (Success/Failure) and either a Value (`T`) or an `Error` object.
*   **Usage:** **Mandatory** return type for all Application layer handlers (Commands and Queries). Controller actions map these `Result` objects to appropriate HTTP responses (`Ok`, `NotFound`, `BadRequest`, `NoContent`, etc.).
*   **Goal:** Explicitly handle operation outcomes (including expected failures like validation errors or "not found") without relying on exceptions for control flow. Improves code clarity and predictability. Makes handler signatures honest about possible outcomes.

### Repository Pattern

*   **Interfaces:** Defined in `Domain/Abstractions/Persistence`. Define data access contracts based on Domain concepts (e.g., `IProductRepository`, `IOrderRepository`).
*   **Implementations:** Located in `Infrastructure/Persistence/Repositories`. Implement the domain interfaces using a specific data access technology (e.g., EF Core).
*   **Goal:** Abstract data access logic, decoupling the Application and Domain layers from specific data storage technologies. Facilitates unit testing of Application logic by allowing repositories to be easily mocked.

### Unit of Work Pattern

*   **Interface:** `IUnitOfWork` defined in `Domain/Abstractions/Persistence` (often just includes `Task<int> SaveChangesAsync(CancellationToken cancellationToken)`).
*   **Implementation:** Located in `Infrastructure/Persistence`. Often implemented by wrapping the `DbContext.SaveChangesAsync()` call. Can be explicitly injected into Command Handlers or, more commonly, orchestrated by a MediatR pipeline behavior (`UnitOfWorkBehavior`) that calls `SaveChangesAsync()` after a command handler successfully executes.
*   **Goal:** Ensure atomicity for business transactions. Groups multiple repository operations (Adds, Updates, Deletes) within a single command handler into a single database transaction.

### IOptions Pattern

*   **Definition:** Configuration classes (POCOs, often `record` types) defined in `Application/Configuration`. Represent strongly-typed configuration sections.
*   **Binding:** Configuration values (from `appsettings.json`, environment variables, Azure Key Vault, etc.) are bound to these classes in `API/Program.cs` using `builder.Services.AddOptions<MySettings>().BindConfiguration("SectionName")`.
*   **Validation:**
    *   Use DataAnnotations directly on the configuration class properties (enable with `.ValidateDataAnnotations()`).
    *   Implement custom validation logic using `IValidateOptions<T>` interfaces (defined/implemented in Infrastructure, registered in `Program.cs`).
    *   Ensure validation runs on application startup using `.ValidateOnStart()`.
*   **Usage:** Inject `IOptions<T>` (singleton snapshot), `IOptionsMonitor<T>` (reacts to changes), or `IOptionsSnapshot<T>` (scoped snapshot) into services/handlers that require configuration. Prefer `IOptions<T>` unless change monitoring is needed.
*   **Goal:** Provide strongly-typed, validated, and testable access to application configuration settings, avoiding magic strings and promoting SOLID principles.

### Validation (FluentValidation)

*   **Implementation:** FluentValidation library (v11.11.0 or later).
*   **Location:** Validators (`AbstractValidator<T>`) defined in the Application layer, co-located with the Command/Query they validate (`/Features/[FeatureName]/Commands/[CommandName]/Validator.cs`).
*   **Integration:** Integrated seamlessly into the MediatR pipeline via a generic `ValidationBehavior<TRequest, TResponse>`. This behavior automatically discovers and runs the appropriate validator before the handler executes. Failures result in a `Result.Failure` containing validation errors, preventing the handler from running.
*   **Goal:** Provide a fluent, declarative, robust, and testable way to define and enforce complex input validation rules for Commands and Queries.

### Mapping (AutoMapper)

*   **Implementation:** AutoMapper library (v14.0.0 or later).
*   **Location:** Mapping profiles (`AutoMapper.Profile`) defined in `Application/Common/Mappings`. Use assembly scanning in `Application/DependencyInjection.cs` to register all profiles. Define specific maps (e.g., `CreateMap<Product, ProductDto>();`).
*   **Usage:** Convert between different object types, primarily Domain Entities <-> Application DTOs. Inject `IMapper` via constructor injection where mapping is needed (typically in Query Handlers or sometimes Command Handlers). Consider `.ProjectTo<T>` for optimizing EF Core queries.
*   **Goal:** Simplify, centralize, and reduce boilerplate code for object-to-object mapping logic.

## 5. Error Handling & Security

### Error Handling

*   **Global Exception Handling:** Implement global exception handling using ASP.NET Core's built-in `IExceptionHandler` interface (preferred in .NET 8+) or custom exception handling middleware (`try/catch` around `next()`). Located in `API/Middleware` or `API/Infrastructure` (for `IExceptionHandler`).
*   **ProblemDetails:** Standardize **all** HTTP API error responses using `ProblemDetails` (RFC 7807). The global handler should catch unhandled exceptions and map them to `ProblemDetails` (typically 500 Internal Server Error).
*   **Result Pattern Mapping:** Controller actions (or a base controller helper method) should explicitly map `Result.Failure` outcomes to appropriate `ProblemDetails` responses with corresponding HTTP status codes (e.g., 400 Bad Request for validation errors, 404 Not Found for missing resources, 409 Conflict for business rule violations).
*   **Custom Exceptions:** Define specific exception types in Domain or Application layers (`DomainException`, `NotFoundException`, `ValidationException`) only when necessary for specific error conditions that cannot be adequately represented by the `Result` pattern or need special handling further up the stack (e.g., by the global exception handler). Prefer the `Result` pattern for predictable failures.
*   **Logging:** Log errors comprehensively within the exception handler middleware/service, including stack traces (but be careful not to expose sensitive details in the HTTP response).

### Security

*   **Authentication:** Use token-based authentication, typically JWT Bearer Tokens, integrated via ASP.NET Core Identity or libraries like `Microsoft.AspNetCore.Authentication.JwtBearer`. Configure authentication schemes and options securely in `API/Program.cs`.
*   **Authorization:** Apply `[Authorize]` attribute(s) granularly on controllers or specific endpoints/actions. Use policy-based authorization (`[Authorize(Policy = "RequireAdminRole")]`) for role- or claim-based access control. Define policies in `API/Program.cs`.
*   **Input Validation:** **Mandatory**. Handled primarily by the FluentValidation MediatR pipeline behavior. Also enforce invariants within Domain entities. Sanitize or validate data received from external sources.
*   **Secrets Management:** **NEVER** commit secrets (API keys, connection strings, passwords) to source control. Use:
    *   .NET User Secrets (local development).
    *   Environment Variables.
    *   Azure Key Vault (production/staging environments) with Managed Identity access.
*   **Logging:** **Avoid logging sensitive data** (Personally Identifiable Information (PII), credentials, full tokens, secrets). Configure logging filters or use techniques like data masking if necessary.
*   **Rate Limiting & Throttling:** Implement rate limiting on public APIs to prevent abuse (using ASP.NET Core Rate Limiting middleware).
*   **HTTPS:** Enforce HTTPS for all communication (`UseHttpsRedirection()`, HSTS).
*   **Security Headers:** Configure appropriate security headers (e.g., `Content-Security-Policy`, `X-Content-Type-Options`, `Referrer-Policy`, `Permissions-Policy`) using middleware or dedicated libraries.
*   **Cross-Origin Resource Sharing (CORS):** Configure CORS policies carefully in `API/Program.cs` to allow requests only from trusted origins. Be as specific as possible.
*   **OWASP Top 10:** Be mindful of common web application security risks (Injection, Broken Authentication, Sensitive Data Exposure, etc.) and apply appropriate mitigation strategies throughout the development lifecycle. Regularly review security practices.

## 6. Testing

### Unit Testing

*   **Frameworks:** xUnit (v2.9.2+), Moq (or NSubstitute), FluentAssertions.
*   **Project Structure:** Create separate test projects per layer being tested (e.g., `YourSolution.Application.UnitTests`, `YourSolution.Domain.UnitTests`).
*   **Focus:**
    *   **Application Layer:** Test Command Handlers and Query Handlers logic. Verify correct interaction with mocked dependencies (repositories, services, mapper, publisher), correct `Result` return (Success/Failure, Value/Error), and correct domain event publishing.
    *   **Domain Layer:** Test Entities (business rules, state changes, invariant enforcement), Value Objects (equality, creation logic), and Domain Services. Test Domain Event creation.
*   **Mocks:** Mock all external dependencies using interfaces defined in Domain or Application layers. Inject mocks via constructors. Common mocks include `IRepository<T>`, `IUnitOfWork`, `IMapper`, `IDateTimeProvider`, `IPublisher`, `IEmailSender`, `ICurrentUserService`.
*   **Structure:** Use the Arrange-Act-Assert (AAA) pattern for clear test structure.
*   **Assertions:** Use FluentAssertions for readable and expressive assertions (`result.Should().BeSuccess()`, `result.Value.Should().BeEquivalentTo(expectedDto)`, `mockRepository.Verify(r => r.Add(It.IsAny<Product>()), Times.Once)`). Verify the returned `Result` object thoroughly (IsSuccess/IsFailure, Value, Error properties). Verify interactions with mocks using `Verify()`.
*   **Coverage:** Aim for high test coverage, focusing on:
    *   Happy paths (successful execution).
    *   Failure paths (validation errors, business rule violations, exceptions from mocks, "not found" scenarios).
    *   Edge cases and boundary conditions.
*   **Requirement:** **Relevant unit tests must be included with all pull requests** that introduce or modify logic. Tests must pass in the CI pipeline.

### Integration Testing (Considerations)

*   **Purpose:** Test the interaction between multiple components or layers, often involving real infrastructure dependencies like a database or external APIs (though these can be test doubles).
*   **Tools:** xUnit, `WebApplicationFactory` (for in-memory API testing), Testcontainers (for ephemeral database/service instances), Respawn (for resetting database state).
*   **Scope:** Test API endpoints through the entire stack (API -> Application -> Infrastructure -> Database). Verify request/response cycles, data persistence, and side effects.
*   **Strategy:** Can be placed in a separate project (e.g., `YourSolution.Api.IntegrationTests`). Use techniques to isolate tests and manage shared resources (like databases) effectively. These tests are generally slower and more complex than unit tests but provide higher confidence.

## 7. Documentation

*   **API Documentation:** Generate comprehensive OpenAPI (Swagger) documentation using Swashbuckle. Ensure controllers, endpoints, actions, and DTOs have accurate XML comments (`/// <summary>`, `/// <param>`, `/// <returns>`) and attributes (`[ProducesResponseType(typeof(ResponseType), StatusCodes.StatusXXX)]`, `[Produces(contentType)]`, `[ApiController]`, `[Route]`, `[Tags]`) for rich and accurate documentation.
*   **Code Documentation:** Document all public API members (classes, interfaces, methods, properties, enums) using XML comments (`///`). Explain complex algorithms, non-obvious decisions, or potential pitfalls using inline comments (`//`) where necessary. Keep comments up-to-date with code changes.
*   **README.md:** Maintain a high-quality `README.md` file at the root of the repository. It should include:
    *   Project purpose and overview.
    *   Technology stack.
    *   Prerequisites and setup instructions (building, running locally).
    *   Key architectural decisions and link to this document.
    *   How to run tests.
    *   Contribution guidelines (linking to PR process, coding standards).

## 8. CI/CD & Collaboration

### CI/CD Integration

*   **CI Pipeline (e.g., GitHub Actions, Azure Pipelines):** Triggered automatically on every push or pull request to main development branches (e.g., `develop`, `main`).
    *   **Core Steps:**
        1.  Checkout code.
        2.  Setup .NET SDK.
        3.  Restore dependencies (`dotnet restore`).
        4.  Build solution (`dotnet build --configuration Release`).
        5.  Run unit tests (`dotnet test --configuration Release --no-build --logger trx`).
        6.  (Optional) Run static code analysis (e.g., SonarQube, .NET Analyzers).
        7.  (Optional) Publish test results and code coverage.
    *   **Goal:** Provide rapid feedback on code quality, enforce standards, and prevent regressions before merging code. Failed builds/tests should block PR merging.
*   **CD Pipeline (e.g., GitHub Actions, Azure Pipelines):** Triggered automatically on merge to main/release branches or manually for specific environment deployments.
    *   **Core Steps:**
        1.  Checkout code.
        2.  Build release artifacts (`dotnet publish --configuration Release`).
        3.  Package application (e.g., Docker image, Zip package).
        4.  Provision/Update Infrastructure (using IaC like Bicep/Terraform).
        5.  Deploy application to Azure environments (Dev, Test, Staging, Production). Use strategies like Blue/Green or Canary deployments via Azure App Service Deployment Slots.
        6.  Run post-deployment smoke tests / health checks.
        7.  (For Staging/Prod) Incorporate manual approval gates.
    *   **Goal:** Automate the deployment process reliably and consistently across environments.

### Code Reviews and Pull Requests

*   **Branching Strategy:** Use a standard branching strategy (e.g., GitFlow, GitHub Flow). Create feature branches from a main development branch (e.g., `develop`).
*   **Pull Requests (PRs):** All code changes must be submitted via PRs targeting the main development branch.
*   **Scope:** Keep PRs **small, focused, and atomic**. Address one feature, bug fix, or refactoring task per PR. Large changes should be broken down.
*   **Description:** Provide a clear and concise PR title and description. Explain *what* the change is and *why* it's needed. Link to the relevant work item (e.g., Azure DevOps Task, GitHub Issue). Include testing notes or screenshots if applicable.
*   **CI Checks:** Ensure all CI checks (build, tests) pass before requesting reviews or merging.
*   **Review Process:** Require at least one (or more, depending on team policy) reviewer approval before merging. Reviewers are responsible for ensuring quality and adherence to standards. Respond to review comments promptly.
*   **Review Checklist (for Reviewers):**
    *   **Adherence:** Does the code follow the principles, patterns, and style guidelines outlined in *this document*?
    *   **Correctness:** Does the code correctly implement the required functionality or fix the bug? Are there any logical errors?
    *   **Test Coverage:** Are there sufficient unit tests (and integration tests where applicable) covering the changes (happy paths, failures, edges)? Do the tests provide meaningful assertions?
    *   **Security:** Are there any potential security vulnerabilities introduced (check inputs, secrets handling, authorization)?
    *   **Performance:** Are there any obvious performance bottlenecks or inefficient queries?
    *   **Readability & Maintainability:** Is the code clear, understandable, and easy to maintain? Are names meaningful? Is complexity managed well?
    *   **Documentation:** Are public APIs documented with XML comments? Is the PR description clear?
    *   **SOLID/Clean Architecture:** Are principles like SRP, OCP, and layer boundaries respected?

## 9. Azure Specific Rules

*   **Azure Best Practices:** When generating code, configuration, or scripts related to Azure services (App Service, Functions, Key Vault, Service Bus, SQL Database, Storage, etc.), strictly adhere to current Microsoft Azure best practices and official documentation.
*   **Infrastructure as Code (IaC):** **Strongly prefer** using IaC tools like Bicep or Terraform to define, provision, and manage Azure resources. Store IaC templates in the source control repository. Integrate IaC deployment into the CD pipeline.
*   **Managed Identities:** Utilize Azure Managed Identities for authenticating Azure services (e.g., App Service connecting to Key Vault, SQL Database, Service Bus) wherever possible. **Avoid** storing connection strings or access keys directly in `appsettings.json` or environment variables in Azure.
*   **Azure SDK:** Use the latest stable versions of the official Azure SDKs for .NET (`Azure.*` packages). Follow recommended patterns for client creation, configuration, and lifecycle management (e.g., using `Azure.Identity` for authentication).
*   **Configuration:** Leverage Azure App Configuration and Azure Key Vault for centralized configuration management and secrets, especially across multiple environments.
*   **Logging & Monitoring:** Integrate with Azure Monitor (Application Insights) for comprehensive logging, tracing, metrics, and application performance monitoring (APM). Configure sampling and filtering appropriately.
*   **Scalability & Resiliency:** Design applications considering Azure's scaling capabilities (e.g., App Service Plans scaling, database tiers) and implement resiliency patterns (e.g., retries with exponential backoff using Polly when interacting with Azure services).

## 10. Quick Reference

### Command/Query Structure

```csharp
// --- Command ---
// Location: /src/YourSolution.Application/Features/Products/Commands/CreateProduct/
#nullable enable

using MediatR;
using YourSolution.Application.Common.Models; // Assuming Result lives here
using YourSolution.Domain.Entities; // Assuming Product entity lives here
using YourSolution.Domain.Abstractions.Persistence; // Assuming IProductRepository lives here
using YourSolution.Domain.Errors; // Assuming ProductErrors lives here

// Use 'record' for immutable message contracts
public record CreateProductCommand(string Name, decimal Price, string Sku) : IRequest<Result<Guid>>;

// --- Command Handler ---
// Location: /src/YourSolution.Application/Features/Products/Commands/CreateProduct/
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    // Assume IUnitOfWork might be handled by a behavior, or injected explicitly if needed.
    // private readonly IUnitOfWork _unitOfWork;

    // Use constructor injection
    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. (Optional) Additional validation not covered by FluentValidation
        // if (string.IsNullOrWhiteSpace(request.Sku))
        // {
        //     return Result.Failure<Guid>(ProductErrors.SkuRequired);
        // }

        // 2. Create Domain Entity using Factory Method
        Result<Product> productResult = Product.Create(
            request.Name,
            request.Price,
            request.Sku);

        // 3. Check for domain validation errors returned by Result
        if (productResult.IsFailure)
        {
            // Forward the domain error
            return Result.Failure<Guid>(productResult.Error);
        }
        Product product = productResult.Value;

        // 4. Add to Repository (does not save yet)
        await _productRepository.AddAsync(product, cancellationToken);

        // 5. Save Changes (Often handled by UnitOfWorkBehavior after handler execution)
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Return Success Result with the new Product's ID
        return Result.Success(product.Id);
    }
}

// --- Query ---
// Location: /src/YourSolution.Application/Features/Products/Queries/GetProductById/
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

// --- Query Response DTO ---
// Location: /src/YourSolution.Application/Features/Products/DTOs/
public record ProductDto(Guid Id, string Name, decimal Price, string Sku);

// --- Query Handler ---
// Location: /src/YourSolution.Application/Features/Products/Queries/GetProductById/
using Microsoft.EntityFrameworkCore; // For AsNoTracking, FirstOrDefaultAsync
using YourSolution.Infrastructure.Persistence.DbContexts; // Assuming ApplicationDbContext lives here
using AutoMapper; // Assuming IMapper is used
using YourSolution.Application.Common.Exceptions; // For NotFoundException (optional)

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    // Often query handlers can directly use DbContext for reads if repositories add too much overhead
    // Or use a dedicated IQueryRepository or the standard IProductRepository
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Retrieve data directly using DbContext and EF Core extensions
        // Use AsNoTracking() for read-only queries for performance!
        Product? product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        // 2. Check if entity exists
        if (product is null)
        {
            // Return specific failure Result using defined Domain Errors
            return Result.Failure<ProductDto>(ProductErrors.NotFound(request.Id));
            // Alternative: Throw application specific exception (less preferred if Result is used consistently)
            // throw new NotFoundException(nameof(Product), request.Id);
        }

        // 3. Map Entity to DTO
        ProductDto productDto = _mapper.Map<ProductDto>(product);
        // Or using ProjectTo for potentially more optimized queries:
        // var productDto = await _dbContext.Products
        //     .Where(p => p.Id == request.Id)
        //     .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
        //     .FirstOrDefaultAsync(cancellationToken);

        // 4. Return Success Result with the DTO
        return Result.Success(productDto);
    }
}
```

### IOptions Pattern Example

```csharp
// --- 1. Define Settings Class (Application Layer) ---
// Location: /src/YourSolution.Application/Configuration/ExternalApiSettings.cs
namespace YourSolution.Application.Configuration;

public class ExternalApiSettings
{
    public const string SectionName = "ExternalApi"; // Define constant for section name

    public required string BaseUrl { get; init; } // Use 'required' for non-nullable strings
    public required string ApiKey { get; init; }
    public int TimeoutSeconds { get; init; } = 30; // Provide default values
}


// --- 2. Configure in appsettings.json (API Layer) ---
// Location: /src/YourSolution.API/appsettings.json
/*
{
  "Logging": { ... },
  "AllowedHosts": "*",
  "ConnectionStrings": { ... },
  "ExternalApi": { // Matches SectionName
    "BaseUrl": "https://api.externalservice.com/v1/",
    "ApiKey": "YOUR_API_KEY_GOES_HERE_BUT_USE_SECRETS_MANAGER", // DO NOT HARDCODE
    "TimeoutSeconds": 60
  }
}
*/
// For local dev, use User Secrets for ApiKey:
// dotnet user-secrets set "ExternalApi:ApiKey" "actual-secret-key-value"


// --- 3. Register and Bind in Program.cs (API Layer) ---
// Location: /src/YourSolution.API/Program.cs
using YourSolution.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register IOptions and bind from configuration
builder.Services.AddOptions<ExternalApiSettings>()
    .BindConfiguration(ExternalApiSettings.SectionName) // Binds to "ExternalApi" section
    .ValidateDataAnnotations() // Optional: if using DataAnnotations on settings class
    .ValidateOnStart(); // Ensures validation runs at startup

// Example: Add custom validation (implementation in Infrastructure layer)
// builder.Services.AddSingleton<IValidateOptions<ExternalApiSettings>, ExternalApiSettingsValidator>();

builder.Services.AddControllers();
// ... other service registrations (Application, Infrastructure)

var app = builder.Build();

// ... configure middleware pipeline ...

app.Run();


// --- 4. Inject and Use (e.g., in an Infrastructure Service) ---
// Location: /src/Infrastructure/YourSolution.Infrastructure/Services/ExternalServiceClient.cs
using Microsoft.Extensions.Options;
using YourSolution.Application.Configuration;
using YourSolution.Application.Common.Abstractions.Infrastructure; // Assuming IExternalServiceClient is defined here

namespace YourSolution.Infrastructure.Services;

public class ExternalServiceClient // : IExternalServiceClient (Example)
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiSettings _settings; // Store the value directly

    // Inject IOptions<T>
    public ExternalServiceClient(HttpClient httpClient, IOptions<ExternalApiSettings> settingsOptions)
    {
        _httpClient = httpClient;
        // Access the configured value via .Value
        _settings = settingsOptions.Value;

        // Configure HttpClient based on settings
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _settings.ApiKey);
    }

    public async Task<string?> GetDataAsync(string resource)
    {
        // Use configured settings
        HttpResponseMessage response = await _httpClient.GetAsync(resource);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
```

### Controller Pattern

```csharp
// --- Base API Controller (Optional but Recommended) ---
// Location: /src/YourSolution.API/Common/ApiControllerBase.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YourSolution.Application.Common.Models; // For Result<T>
using Microsoft.AspNetCore.Http; // For StatusCodes

namespace YourSolution.API.Common;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")] // Example versioned route
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator; // Use ISender for sending requests

    // Use protected property for easy access in derived controllers
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    // Helper method to handle Result<T> pattern consistently
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess && result.Value != null)
            return Ok(result.Value);
        if (result.IsSuccess && result.Value == null) // For commands returning Result<Unit> or similar void success
             return NoContent(); // Or Ok() depending on desired response for void success

        // Map specific domain/application errors to status codes if needed
        // This mapping logic could be more sophisticated
        return result.Error.Code switch // Assuming Error has a Code property
        {
            "Validation" => BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, "Validation Error", result.Error.Description)),
            "NotFound" => NotFound(CreateProblemDetails(StatusCodes.Status404NotFound, "Resource Not Found", result.Error.Description)),
            "Conflict" => Conflict(CreateProblemDetails(StatusCodes.Status409Conflict, "Conflict", result.Error.Description)),
            // Add other specific error mappings
            _ => BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, "Bad Request", result.Error.Description)) // Default bad request
        };
    }

    // Helper for creating ProblemDetails (could be more detailed)
    private ProblemDetails CreateProblemDetails(int statusCode, string title, string detail) =>
        new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
            // Instance = HttpContext.Request.Path // Optionally add instance
        };

    // Overload for non-generic Result (e.g., for Commands without a return value)
    protected ActionResult HandleResult(Result result)
    {
         if (result.IsSuccess)
             return NoContent(); // Common for successful commands with no content return

         return result.Error.Code switch
         {
             "Validation" => BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, "Validation Error", result.Error.Description)),
             "NotFound" => NotFound(CreateProblemDetails(StatusCodes.Status404NotFound, "Resource Not Found", result.Error.Description)),
             "Conflict" => Conflict(CreateProblemDetails(StatusCodes.Status409Conflict, "Conflict", result.Error.Description)),
             _ => BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, "Bad Request", result.Error.Description))
         };
    }
}


// --- Feature Controller ---
// Location: /src/YourSolution.API/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using YourSolution.API.Common; // Use the base controller
using YourSolution.Application.Features.Products.Commands.CreateProduct; // Command
using YourSolution.Application.Features.Products.Queries.GetProductById; // Query
using YourSolution.Application.Features.Products.DTOs; // DTO
using Microsoft.AspNetCore.Authorization; // For authorization
using Asp.Versioning; // For API Versioning attributes

namespace YourSolution.API.Controllers;

[ApiVersion("1.0")] // Example using Asp.Versioning
// [Authorize] // Apply authorization as needed
public class ProductsController : ApiControllerBase // Inherit from base
{
    // No need to inject ISender here if using the base class property

    // POST api/v1/products
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)] // Use Guid for Create success
    [ProducesResponseType(StatusCodes.Status201Created)] // Alternative: CreatedAtRoute
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        Result<Guid> result = await Mediator.Send(command);

        // Use the helper method from the base class to handle the result
        // return HandleResult(result);

        // Or if you want CreatedAtRoute specifically for POST success:
         if (result.IsSuccess)
         {
             // Assuming the GetProductById query/route exists and is named "GetProductById"
             return CreatedAtRoute("GetProductById", new { version = "1.0", id = result.Value }, result.Value);
         }
         return HandleResult(result); // Handle failure cases
    }

    // GET api/v1/products/{id}
    [HttpGet("{id:guid}", Name = "GetProductById")] // Add route name for CreatedAtRoute
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        Result<ProductDto> result = await Mediator.Send(query);

        // Use the helper method from the base class
        return HandleResult(result);
    }

    // Other endpoints (PUT, DELETE, List etc.) following the same pattern...
}
```

---