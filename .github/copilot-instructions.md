# Project Architecture and GitHub Copilot Guidance (.NET 9 / C# 14)

## Table of Contents
1. [Purpose](#1-purpose)
2. [Core Mandates & Principles](#2-core-mandates--principles)
   - [Non-Negotiable Rules](#non-negotiable-rules)
   - [Key Architectural Goals](#key-architectural-goals)
   - [Preferred Libraries & Versions](#preferred-libraries--versions)
   - [Code Quality & Style](#code-quality--style)
3. [Architecture Layers (Clean Architecture)](#3-architecture-layers-clean-architecture)
   - [Domain Layer (`src/YourSolution.Domain`)](#domain-layer-srcyoursolutiondomain)
   - [Application Layer (`src/YourSolution.Application`)](#application-layer-srcyoursolutionapplication)
   - [Infrastructure Layer (`src/YourSolution.Infrastructure`)](#infrastructure-layer-srcyoursolutioninfrastructure)
   - [API / Presentation Layer (`src/YourSolution.API`)](#api--presentation-layer-srcyoursolutionapi)
4. [Core Patterns Implementation Guide](#4-core-patterns-implementation-guide)
   - [Clean Architecture Enforcement](#clean-architecture-enforcement)
   - [CQRS (Command Query Responsibility Segregation) with MediatR](#cqrs-command-query-responsibility-segregation-with-mediatr)
   - [Result Pattern (Mandatory)](#result-pattern-mandatory)
   - [Repository Pattern](#repository-pattern)
   - [Unit of Work (UoW) Pattern](#unit-of-work-uow-pattern)
   - [IOptions Pattern for Configuration](#ioptions-pattern-for-configuration)
   - [Validation with FluentValidation](#validation-with-fluentvalidation)
   - [Mapping with AutoMapper](#mapping-with-automapper)
   - [Screaming Architecture](#screaming-architecture)
5. [Critical Cross-Cutting Concerns](#5-critical-cross-cutting-concerns)
   - [Error Handling Strategy](#error-handling-strategy)
   - [Security Best Practices](#security-best-practices)
6. [Testing Strategy](#6-testing-strategy)
   - [Unit Testing Requirements](#unit-testing-requirements)
   - [Integration Testing Considerations](#integration-testing-considerations)
7. [Documentation Standards](#7-documentation-standards)
8. [Development Workflow & Collaboration](#8-development-workflow--collaboration)
   - [CI/CD Pipeline Integration](#cicd-pipeline-integration)
   - [Code Reviews and Pull Requests (PRs)](#code-reviews-and-pull-requests-prs)
9. [Azure Development Rules](#9-azure-development-rules)
10. [Quick Reference & Code Examples (Copilot Focus)](#10-quick-reference--code-examples-copilot-focus)
    - [Command/Query Structure Template](#commandquery-structure-template)
    - [IOptions Pattern Usage Example](#ioptions-pattern-usage-example)
    - [Controller Implementation Pattern](#controller-implementation-pattern)

## 1. Purpose

This document is the **definitive implementation guide** for this .NET 9 / C# 14 project. It outlines the mandatory architectural principles, patterns, coding standards, and processes. It serves as the **source of truth** for **all team members** and **GitHub Copilot** to ensure the generation and modification of high-quality, consistent, maintainable, and scalable code, adhering strictly to the established architecture (Clean Architecture, CQRS, DDD concepts, Result Pattern).

**Primary Framework:** .NET 9 / C# 14 (targeting latest stable releases)

## 2. Core Mandates & Principles

### Non-Negotiable Rules

*   **Clean Architecture Adherence:** **Strictly** enforce layer separation and the **Dependency Rule** (Dependencies **MUST** flow inwards: API -> Application -> Domain; Infrastructure -> Application -> Domain). No violations allowed.
*   **Result Pattern Usage:** **Mandatory** return type (`Result` or `Result<T>`) for **all** Application layer handlers (Commands and Queries). Avoid exceptions for predictable control flow.
*   **Async/Await Consistency:** **Mandatory:** Use `async`/`await` correctly for **all** I/O-bound operations. **AVOID** `async void` (except UI event handlers, not applicable here). **AVOID** blocking calls (`.Result`, `.Wait()`). Suffix all genuinely asynchronous methods with `Async`.
*   **Dependency Injection:** Use .NET's built-in DI container **exclusively**. **Mandatory:** Use **constructor injection**. Register services in layer-specific extensions (`DependencyInjection.cs`). **AVOID** Service Locators.
*   **Nullability:** Enable and **strictly adhere** to C# nullable reference types (`#nullable enable`). Eliminate warnings. Design APIs with clear nullability intent. **AVOID** null suppression (`!`) unless absolutely necessary and documented.
*   **Immutability:** **Strongly prefer** immutable types (`record` classes/structs) for DTOs, Commands, Queries, Events, and Value Objects. Use `init` setters where appropriate.
*   **English Language:** **Mandatory:** All code, comments, documentation, commit messages, and PR descriptions **MUST** be in English.

### Key Architectural Goals

*   Achieve: **Separation of Concerns**, Low Coupling, High Cohesion, Testability, Maintainability, Scalability, Understandability.
*   Apply **SOLID** principles rigorously.
*   Utilize core **DDD concepts** (Entities, Value Objects, Aggregates, Domain Events, Repositories) within the Domain layer. Protect domain invariants fiercely.

### Preferred Libraries & Versions

Use these specific versions or **later compatible stable versions**. Justify any deviation.

*   **Data Access:** Entity Framework Core (for .NET 9)
*   **CQRS Mediator:** MediatR (`v12.5.0+`)
*   **Mapping:** AutoMapper (`v14.0.0+`)
*   **Validation:** FluentValidation (`v11.11.0+`)
*   **Logging:** Serilog (Preferred) or `Microsoft.Extensions.Logging`
*   **API Documentation:** Swashbuckle.AspNetCore (`6.6.0+`) or leverage native `.NET 9 OpenAPI` + UI (SwaggerUI/Scalar)
*   **Testing:** xUnit (`v2.9.2+`), Moq (or NSubstitute), FluentAssertions (`6.12.0+`)

### Code Quality & Style

*   **Standards:** Follow Microsoft's C# coding conventions and .NET runtime design guidelines. Use C# 14 features (primary constructors, collection expressions, `field` keyword) where they improve clarity/conciseness.
*   **Readability:** Prioritize clear, self-documenting code. Use meaningful names. Keep methods and files short and focused (SRP).
*   **Naming:** `PascalCase` (types, public members, methods), `camelCase` (parameters, locals), `_camelCase` (private fields). `IMyInterface` for interfaces. Constants: `PascalCase` or `UPPER_SNAKE_CASE`.
*   **Comments:** Explain **WHY**, not what (code explains 'what'). Use XML doc comments (`///`) extensively for all public types/members.
*   **Magic Literals:** **AVOID** magic strings/numbers. Use `const`, `static readonly`, `enum`, `nameof()`, or configuration (`IOptions`).
*   **Statics:** **Minimize** static classes/methods for logic/state. Prefer DI. Static helpers for pure functions are OK.

## 3. Architecture Layers (Clean Architecture)

**Dependency Rule Reminder:** API -> Application -> Domain | Infrastructure -> Application -> Domain

### Domain Layer (`src/YourSolution.Domain`)

*   **Purpose:** Core business logic, rules, state. **FRAMEWORK INDEPENDENT**.
*   **Contents:** Entities, Value Objects (`record` or `record struct`), Aggregates, Domain Events (`INotification`), Enums, Custom Domain Exceptions (for business rule violations), Domain Errors (for exception message consistency), Abstractions (`/Persistence/IRepository.cs`, `/Persistence/IUnitOfWork.cs`, `/Services/IDomainService.cs`).
*   **Dependencies:** **ZERO** dependencies on other project layers. Minimal external libs (e.g., MediatR.Contracts).
*   **Key Rules:** Enforce invariants within Entities/Aggregates. Use Factory Methods (`public static Entity Create(...)`) which throw domain-specific exceptions if validation fails. Prefer private setters. Domain operations throw specific exceptions when business rules are violated.

### Application Layer (`src/YourSolution.Application`)

*   **Purpose:** Orchestrates use cases, contains application logic, acts as boundary for domain exceptions.
*   **Contents:** Organized by **Feature** (Screaming Architecture):
    *   `/Features/[FeatureName]/Commands/[CommandName]/` (`Command.cs` (`record`), `Handler.cs`, `Validator.cs`)
    *   `/Features/[FeatureName]/Queries/[QueryName]/` (`Query.cs` (`record`), `Handler.cs`, `Response.cs` (`record` DTO))
    *   `/Features/[FeatureName]/DTOs/` (Feature-specific DTOs, use `record`)
    *   `/Common/Abstractions/Infrastructure/` (Interfaces for Infra: `IEmailSender`, `IDateTimeProvider`, etc.)
    *   `/Common/Abstractions/Presentation/` (Interfaces for Presentation: `ICurrentUserService`)
    *   `/Common/Behaviours/` (MediatR Pipelines: Logging, Validation, UoW, Caching, etc.)
    *   `/Common/Exceptions/` (Application exceptions: `NotFoundException`, etc.)
    *   `/Common/Mappings/` (AutoMapper Profiles)
    *   `/Common/Models/` (Shared models: `PagedResult<T>`, `Result.cs`/`Result<T>.cs`)
    *   `/Configuration/` (**IOptions class definitions**: POCOs/`record`s for settings)
    *   `DependencyInjection.cs` (Service registration extension)
*   **Dependencies:** Depends **ONLY** on **Domain**. **NO** dependencies on Infrastructure or API.
*   **Key Rules:** Handlers implement single use cases. Use **MediatR** (`ISender`) for dispatching. **Return `Result<T>` MANDATORY**. **Catch domain exceptions and translate to `Result.Failure`**. Interact with Infra **ONLY** via abstractions. Use **FluentValidation** for input validation. Use **AutoMapper** for Entity <-> DTO mapping.

### Infrastructure Layer (`src/YourSolution.Infrastructure`)

*   **Purpose:** Implements abstractions defined in Application/Domain. Handles external concerns.
*   **Contents:** `/Persistence/` (DbContext, Repository implementations, EF Core Configs (`IEntityTypeConfiguration`), Migrations, UoW implementation), `/Services/` (EmailSender, DateTimeProvider implementations), `/Authentication/` (JWT logic, `ICurrentUserService` impl), `/Caching/`, `/Configuration/Validators/` (**IOptions `IValidateOptions<T>` implementations**), `DependencyInjection.cs`.
*   **Dependencies:** Depends on **Application** and **Domain**. May depend on external libraries (EF Core, Azure SDK, etc.).
*   **Key Rules:** Implement repository interfaces using EF Core. Use `IEntityTypeConfiguration`. **Repositories MUST NOT call `SaveChangesAsync()`** (UoW responsibility). Use `AsNoTracking()` extensively for queries. Implement `IValidateOptions<T>` for complex config validation.

### API / Presentation Layer (`src/YourSolution.API`)

*   **Purpose:** Exposes application functionality via HTTP. Handles request/response cycle.
*   **Contents:** `/Controllers/` (Organized by Feature, **Thin**, inherit `ApiControllerBase`), `/Endpoints/` (Alternative: Minimal APIs by Feature), `/Middleware/` (Exception Handling, Logging), `/Extensions/`, `/Filters/`, `/Common/` (`ApiControllerBase.cs`), `appsettings.json`, `Program.cs`.
*   **Dependencies:** Depends on **Application** (via `MediatR.ISender`). **MUST NOT** depend directly on Domain. Can reference Infrastructure **only** for DI setup in `Program.cs`.
*   **Key Rules:** Keep Controllers/Endpoints **THIN**: delegate work to MediatR (`ISender.Send`). Handle request mapping, authentication/authorization, response formatting. Use `ApiControllerBase` helper to map `Result` to `ProblemDetails` HTTP responses. Configure DI, middleware, auth, logging, health checks, **IOptions binding** in `Program.cs`. Define API Versioning strategy. Use `[ProducesResponseType]` extensively.

## 4. Core Patterns Implementation Guide

### Clean Architecture Enforcement

*   **How:** Strict project references. Adherence to layer responsibilities (Section 3). Constant vigilance during code reviews.
*   **Why:** Decoupling, Testability, Maintainability, Framework Independence.

### CQRS (Command Query Responsibility Segregation) with MediatR

*   **How:** Use MediatR (`v12.5.0+`). Define Commands (`IRequest<Result...>`) and Queries (`IRequest<Result<TResponse>>`) as immutable `record` types in Application Layer Features. Implement corresponding `IRequestHandler<,>`. Inject `ISender` into Controllers/Endpoints to dispatch requests.
*   **Why:** Separate write/read paths for independent optimization and complexity management.

### Result Pattern (Mandatory)

*   **How:** Define shared `Result` and `Result<T>` types (e.g., in `Application.Common.Models` or a shared kernel). Application layer handlers **MUST** return `Result` or `Result<T>`. Domain layer throws exceptions for business rule violations, which Application layer catches and translates to appropriate `Result.Failure` responses. Controllers **MUST** map `Result` outcomes (Success/Failure) to appropriate HTTP status codes and `ProblemDetails` responses using the `ApiControllerBase` helper.
*   **Why:** Clear separation of concerns: Domain layer focuses on business rules with appropriate exceptions, while Application layer handles exception translation and provides a consistent error handling approach. Explicit, predictable handling of operation outcomes without relying on exceptions flowing beyond the Application boundary. Improves clarity and robustness.

### Repository Pattern

*   **How:** Define interfaces (`IRepository<T>`, `IProductRepository`) in `Domain/Abstractions/Persistence`. Implement interfaces in `Infrastructure/Persistence/Repositories` using EF Core. Inject interfaces into Application Handlers.
*   **Why:** Abstracts data access, decouples Application/Domain from EF Core, facilitates unit testing.

### Unit of Work (UoW) Pattern

*   **How:** Define `IUnitOfWork` (typically `Task<int> SaveChangesAsync(...)`) in `Domain/Abstractions/Persistence`. Implement in `Infrastructure/Persistence` (wrapping `DbContext.SaveChangesAsync`). **Orchestrate via MediatR Pipeline Behavior** (`UnitOfWorkBehavior`) which calls `SaveChangesAsync()` **after** successful command handler execution. Avoid manual `SaveChangesAsync` calls in handlers.
*   **Why:** Ensures atomicity for business transactions spanning multiple repository operations within a single command.

### IOptions Pattern for Configuration

*   **How:** Define POCOs/`record`s in `Application/Configuration`. Bind configuration sections (from `appsettings.json`, env vars, Key Vault) in `API/Program.cs` using `builder.Services.AddOptions<T>().BindConfiguration("SectionName")`. Validate using DataAnnotations (`.ValidateDataAnnotations()`) or custom `IValidateOptions<T>` (implemented in Infra, registered in `Program.cs`). **Mandatory:** use `.ValidateOnStart()`. Inject `IOptions<T>`, `IOptionsMonitor<T>`, or `IOptionsSnapshot<T>` into services.
*   **Why:** Strongly-typed, validated, testable configuration access. Avoids magic strings.

### Validation with FluentValidation

*   **How:** Use FluentValidation (`v11.11.0+`). Define `AbstractValidator<T>` classes in Application Layer, co-located with the Command/Query they validate. **Integrate via MediatR Pipeline Behavior** (`ValidationBehavior`). Behavior runs validation automatically before handler execution. Failure returns `Result.Failure`.
*   **Why:** Fluent, declarative, robust, testable input validation. Keeps handlers clean.

### Mapping with AutoMapper

*   **How:** Use AutoMapper (`v14.0.0+`). Define `Profile` classes in `Application/Common/Mappings`. Use assembly scanning in `Application/DependencyInjection.cs`. Inject `IMapper` where needed (Handlers). Consider `.ProjectTo<T>` for optimizing EF Core queries directly to DTOs.
*   **Why:** Simplifies and centralizes object-to-object mapping (Entity <-> DTO). Reduces boilerplate.

### Screaming Architecture

*   **How:** Organize folders and namespaces within Application and API layers primarily by **business feature** (e.g., `/Features/Orders/`, `/Controllers/OrdersController.cs`), not by technical type (e.g., `/Services/`, `/Repositories/`).
*   **Why:** Makes the application's purpose and structure immediately clear. Improves navigability and understandability.

## 5. Critical Cross-Cutting Concerns

### Error Handling Strategy

*   **Domain Exceptions:** Domain layer **SHOULD** throw specific domain exceptions when business rules or invariants are violated (`DomainException`, `ProductNameTooShortException`, etc.).
*   **Application Boundary:** Application layer handlers **MUST** catch all domain exceptions and translate them to appropriate `Result.Failure` responses. 
*   **Global Handling:** Implement global exception handling using **`IExceptionHandler`** (preferred in .NET 8+) or custom middleware in API layer. Catch unhandled exceptions (infrastructure issues, bugs, etc.).
*   **Standard Response:** **Mandatory:** Standardize **all** API error responses using **`ProblemDetails`** (RFC 7807). Global handler maps uncaught exceptions to 500 `ProblemDetails`.
*   **Result Mapping:** Controllers **MUST** map `Result.Failure` outcomes to appropriate `ProblemDetails` responses with correct HTTP status codes (400, 404, 409, etc.) via `ApiControllerBase` helper.
*   **Logging:** Log errors comprehensively in the global handler (including stack trace for unexpected errors). **DO NOT** expose stack traces or sensitive details in HTTP responses.

### Security Best Practices

*   **Authentication:** Implement robust token-based auth (JWT recommended). Configure securely in `Program.cs`.
*   **Authorization:** Apply `[Authorize]` attributes granularly. Use policy-based authorization for roles/claims. Define policies in `Program.cs`.
*   **Input Validation:** **Mandatory** via FluentValidation pipeline behavior. Enforce invariants in Domain Entities. Sanitize external inputs.
*   **Secrets Management:** **NEVER** commit secrets. Use .NET User Secrets (dev), Environment Variables, and **Azure Key Vault** (prod/staging) with **Managed Identities**.
*   **Logging:** **AVOID** logging sensitive data (PII, secrets, tokens). Use masking if needed.
*   **Rate Limiting:** Implement on public endpoints using ASP.NET Core middleware.
*   **HTTPS:** **Enforce HTTPS** (`UseHttpsRedirection()`, HSTS).
*   **Security Headers:** Configure headers (CSP, X-Content-Type-Options, etc.).
*   **CORS:** Configure policies **carefully** and **specifically** in `Program.cs`. Allow only trusted origins.
*   **OWASP Top 10:** Be mindful of common vulnerabilities. Implement mitigations. Regularly review security.

## 6. Testing Strategy

### Unit Testing Requirements

*   **Tools:** xUnit, Moq (or NSubstitute), FluentAssertions.
*   **Structure:** Separate test projects (`*.UnitTests`). Use Arrange-Act-Assert (AAA).
*   **Scope:** Test Application handlers (verify logic, mock interactions, `Result` outcome, event publishing). Test Domain entities (rules, invariants), VOs, Domain Services.
*   **Mocks:** Mock **all** external dependencies (repositories, UoW, services, mapper, publisher) using interfaces. Inject mocks.
*   **Assertions:** Use **FluentAssertions** for readable assertions. **Thoroughly verify `Result` objects** (IsSuccess/IsFailure, Value, Error). Verify mock interactions (`Verify()`).
*   **Coverage:** Aim for high coverage of logic, focusing on happy paths, **failure paths** (validation, rules, exceptions from mocks, not found), and edge cases.
*   **Mandatory:** **Relevant unit tests MUST accompany all PRs**. Tests **MUST** pass in CI.

### Integration Testing Considerations

*   **Purpose:** Verify interactions between layers (API -> App -> Infra -> DB). Test full request/response cycles.
*   **Tools:** xUnit, `WebApplicationFactory`, Testcontainers (for DBs/services), Respawn (DB reset).
*   **Scope:** Can be in separate `*.IntegrationTests` projects. Use in-memory DB for faster tests where appropriate, or Testcontainers for real DB testing. Slower but provide higher confidence.

## 7. Documentation Standards

*   **API Documentation (OpenAPI):** **Mandatory:** Generate rich OpenAPI docs using Swashbuckle/Native support. Requires accurate XML comments (`/// <summary>`, etc.) and attributes (`[ProducesResponseType]`, `[ApiController]`, etc.) on Controllers, Actions, DTOs.
*   **Code Documentation:** Document all public members (`///`). Explain **WHY** for complex/non-obvious code. Keep comments updated.
*   **README.md:** High-level overview, setup, tech stack, link to *this document* for implementation details.

## 8. Development Workflow & Collaboration

### CI/CD Pipeline Integration

*   **CI (GitHub Actions / Azure Pipelines):** Trigger on push/PR to dev branches. Steps: Checkout -> Setup .NET -> Restore -> Build -> **Run Unit Tests** -> (Optional) Static Analysis -> Publish Results. **MUST** block PR merge on failure.
*   **CD (GitHub Actions / Azure Pipelines):** Trigger on merge/manually. Steps: CI Steps -> Publish Artifacts -> IaC Deploy (Bicep/Terraform) -> Deploy App (App Service Slots) -> Smoke Tests -> (Optional) Approvals. **Automate deployment**.

### Code Reviews and Pull Requests (PRs)

*   **Branching:** Use GitFlow or GitHub Flow (feature branches from `develop`/`main`).
*   **PRs:** **Mandatory** for all changes. Target main dev branch.
*   **Scope:** **Keep PRs SMALL, FOCUSED, ATOMIC**. One feature/bugfix per PR.
*   **Description:** Clear title/description. Explain **WHAT** and **WHY**. Link work items. Include test notes.
*   **CI Checks:** **MUST** pass before review/merge.
*   **Review Process:** Require reviewer approval(s). Reviewers ensure quality and adherence to *this document*. Respond to comments promptly.
*   **Reviewer Checklist:** Adherence to this guide? Correctness? Test Coverage (Unit Tests MANDATORY)? Security? Performance? Readability? Documentation? SOLID/Clean Arch principles respected?

## 9. Azure Development Rules

*   **Azure Best Practices:** **Strictly** follow current Microsoft Azure best practices for all Azure services used.
*   **Infrastructure as Code (IaC):** **Mandatory:** Use **Bicep** or Terraform for defining and managing Azure resources. Store templates in source control. Integrate into CD pipeline.
*   **Managed Identities:** **Mandatory:** Use Azure Managed Identities for Azure service-to-service authentication (App Service to Key Vault/SQL DB/Service Bus, etc.). **AVOID** connection strings/keys in config/env vars in Azure.
*   **Azure SDK:** Use latest stable official Azure SDKs (`Azure.*`). Follow recommended patterns (`Azure.Identity`).
*   **Configuration:** Use **Azure App Configuration** and **Azure Key Vault** (integrated via Managed Identity) for centralized config/secrets management.
*   **Monitoring:** **Mandatory:** Integrate with **Azure Monitor (Application Insights)** for comprehensive logging, tracing, metrics, APM. Configure appropriately.
*   **Resiliency:** Design for scalability/resiliency. Use patterns like retries (Polly) for transient Azure service issues.

## 10. Quick Reference & Code Examples (Copilot Focus)

**Copilot:** Use these examples as **primary templates** when generating code for Controllers, Commands, Queries, and IOptions usage. Pay close attention to patterns like **`Result<T>`**, **`ISender`**, **`IOptions<T>`**, **`AsNoTracking()`**, and the **`HandleResult`** helper.

### Command/Query Structure Template

```csharp
// --- Command ---
// Location: /src/YourSolution.Application/Features/Products/Commands/CreateProduct/
#nullable enable

using MediatR;
using YourSolution.Application.Common.Models; // **Result<T>** lives here
using YourSolution.Domain.Entities;           // Product entity lives here
using YourSolution.Domain.Abstractions.Persistence; // IProductRepository lives here
using YourSolution.Domain.Errors;             // ProductErrors lives here
using YourSolution.Domain.ValueObjects;       // If using VOs like SKU
using YourSolution.Domain.Exceptions;         // Domain exceptions live here

// **Use 'record' for immutable messages**
public record CreateProductCommand(string Name, decimal Price, string Sku) : IRequest<Result<Guid>>;

// --- Command Handler ---
// Location: /src/YourSolution.Application/Features/Products/Commands/CreateProduct/
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    // IUnitOfWork likely handled by Pipeline Behavior, not explicitly called here

    // **Use constructor injection**
    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Call Domain Factory Method which may throw domain exceptions
            // Note: No Result<T> at Domain level - uses exceptions instead
            Product product = Product.Create(
                request.Name,
                request.Price,
                request.Sku
            );

            // 2. Add to Repository (does not save)
            await _productRepository.AddAsync(product, cancellationToken);

            // 3. Save Changes -> Handled by UnitOfWorkBehavior pipeline

            // 4. **Return Success Result**
            return Result.Success(product.Id);
        }
        catch (ProductNameTooShortException ex)
        {
            // **Translate domain exceptions to Result.Failure**
            return Result.Failure<Guid>(ProductErrors.InvalidName(ex.Message));
        }
        catch (InvalidSkuException ex)
        {
            return Result.Failure<Guid>(ProductErrors.InvalidSku(ex.Message));
        }
        catch (DomainException ex) // Base domain exception
        {
            return Result.Failure<Guid>(ProductErrors.General(ex.Message));
        }
    }
}

// --- Query ---
// Location: /src/YourSolution.Application/Features/Products/Queries/GetProductById/
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

// --- Query Response DTO ---
// Location: /src/YourSolution.Application/Features/Products/DTOs/
// **Use 'record' for immutable DTOs**
public record ProductDto(Guid Id, string Name, decimal Price, string Sku);

// --- Query Handler ---
// Location: /src/YourSolution.Application/Features/Products/Queries/GetProductById/
using Microsoft.EntityFrameworkCore; // For EF Core specific methods
using YourSolution.Infrastructure.Persistence.DbContexts; // Direct DbContext often OK for queries
using AutoMapper;
using AutoMapper.QueryableExtensions; // For ProjectTo

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly ApplicationDbContext _dbContext; // Direct context for optimized reads
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Query using DbContext & ProjectTo for efficiency
        ProductDto? productDto = await _dbContext.Products
            .Where(p => p.Id == request.Id)
            // **Use ProjectTo for efficient mapping**
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            // **Use AsNoTracking implicitly with ProjectTo, otherwise add explicitly**
            // .AsNoTracking() // Not needed with ProjectTo
            .FirstOrDefaultAsync(cancellationToken);

        // 2. **Check if found and return specific failure Result**
        if (productDto is null)
        {
            return Result.Failure<ProductDto>(ProductErrors.NotFound(request.Id));
        }

        // 3. **Return Success Result with DTO**
        return Result.Success(productDto);
    }
}
```

### IOptions Pattern Usage Example

```csharp
// --- 1. Define Settings Class (Application Layer) ---
// Location: /src/YourSolution.Application/Configuration/ExternalApiSettings.cs
namespace YourSolution.Application.Configuration;

// **Use 'record' or class**
public record ExternalApiSettings
{
    public const string SectionName = "ExternalApi"; // **Constant for section name**

    public required string BaseUrl { get; init; } // **'required' for non-nullable**
    public required string ApiKey { get; init; } // **Store sensitive data in Key Vault/User Secrets**
    public int TimeoutSeconds { get; init; } = 30; // Default value
}

// --- 2. Configure in appsettings.json (API Layer) ---
// Location: /src/YourSolution.API/appsettings.json
/*
{
  "ExternalApi": { // **Matches SectionName**
    "BaseUrl": "https://api.externalservice.com/v1/",
    // "ApiKey": "DO NOT STORE HERE - Use User Secrets or Key Vault"
    "TimeoutSeconds": 60
  }
}
*/
// Local Dev: dotnet user-secrets set "ExternalApi:ApiKey" "..."
```
