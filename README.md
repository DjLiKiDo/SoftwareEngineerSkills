# SoftwareEngineerSkills - Modern Architecture Example for Complex API in .NET 9

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com) <!-- Replace with your CI badge -->
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) <!-- Or the license you use -->
[![.NET Version](https://img.shields.io/badge/.NET-9.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![C# Version](https://img.shields.io/badge/C%23-14-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)

This repository serves as a comprehensive and modern example for building a large, complex API using .NET 9 and C# 14. It demonstrates the implementation of robust architectural principles and contemporary design patterns to create a scalable, maintainable, testable, and high-performance application.

## Table of Contents

1.  [Introduction](#introduction)
2.  [Foundational Architectural Principles](#foundational-architectural-principles)
    *   [Clean Architecture](#clean-architecture)
    *   [Domain-Driven Design (DDD)](#domain-driven-design-ddd)
    *   [CQRS (Command Query Responsibility Segregation)](#cqrs-command-query-responsibility-segregation)
    *   [Screaming Architecture](#screaming-architecture)
3.  [Key Design Patterns Implemented](#key-design-patterns-implemented)
    *   [Result Pattern](#result-pattern)
    *   [Repository and Unit of Work Patterns](#repository-and-unit-of-work-patterns)
    *   [IOptions Pattern](#ioptions-pattern)
4.  [Technology Stack](#technology-stack)
5.  [Leveraging .NET 9 and C# 14 Features](#leveraging-net-9-and-c-14-features)
6.  [Project Structure](#project-structure)
7.  [Getting Started](#getting-started)
    *   [Prerequisites](#prerequisites)
    *   [Cloning](#cloning)
    *   [Building](#building)
    *   [Running the API](#running-the-api)
    *   [Running Tests](#running-tests)
8.  [Implementation Highlights](#implementation-highlights)
    *   [API Documentation with OpenAPI](#api-documentation-with-openapi)
    *   [Robust Error Handling](#robust-error-handling)
    *   [Validation with FluentValidation](#validation-with-fluentvalidation)
    *   [Mapping with AutoMapper](#mapping-with-automapper)
    *   [Typed Configuration](#typed-configuration)
    *   [Security](#security)
    *   [Logging](#logging)
9.  [Further Considerations](#further-considerations)
10. [Contributing](#contributing)
11. [License](#license)

## Introduction

The goal of this project is to provide a solid foundation and practical example for developing complex APIs in the modern .NET ecosystem. It showcases how to combine design patterns like Clean Architecture, DDD, and CQRS to manage complexity and build software that closely aligns with business needs. Emphasis is placed on separation of concerns, testability, maintainability, and leveraging the latest features of .NET and C#.

## Foundational Architectural Principles

The architecture of this API is based on several key principles:

### Clean Architecture

We adopt Clean Architecture for a clear **separation of concerns** between the different layers of the application:

*   **Domain:** Contains the core business logic, entities, value objects, domain events, and repository interfaces. It is independent of any framework or infrastructure.
*   **Application:** Orchestrates the use cases (commands and queries), defines DTOs (Data Transfer Objects), and interfaces for infrastructure concerns (e.g., `IEmailSender`). It depends on the Domain.
*   **Infrastructure:** Implements the interfaces defined in Domain and Application. Contains technical details such as data access (EF Core), integration with external services, logging, etc. It depends on Domain and Application.
*   **API/Presentation:** Exposes functionality through HTTP endpoints. Handles requests, responses, authentication, authorization, and serialization. It depends on Application.

The **dependency rule** is fundamental: dependencies always flow inwards (API -> Application -> Domain). This promotes maintainability, testability, and independent evolution of the layers.

### Domain-Driven Design (DDD)

DDD guides the modeling of the application's core to accurately reflect the business domain.

*   **Strategic:** Focuses on defining **Bounded Contexts** (clear boundaries where a specific domain model applies) and a **Ubiquitous Language** (shared vocabulary between developers and domain experts within a context).
*   **Tactical:** Uses building blocks such as:
    *   **Entities:** Objects with a unique identity (e.g., Order, Customer).
    *   **Value Objects:** Immutable descriptive objects without a unique identity (e.g., Address, Money). Often implemented as C# `record` types.
    *   **Aggregates:** Clusters of entities and value objects treated as a single transactional unit, with an **Aggregate Root** as the sole entry point.
    *   **Domain Events:** Represent significant occurrences in the domain (e.g., `OrderCreated`), enabling loose coupling.

DDD is primarily applied in the Domain layer.

### CQRS (Command Query Responsibility Segregation)

We separate operations that modify state (**Commands**) from those that read data (**Queries**).

*   **Benefits:** Allows optimizing read and write paths independently, improving performance and scalability. Simplifies data models for each operation.
*   **Implementation:** We use the **MediatR** library to mediate between commands/queries and their handlers. Commands typically operate on Domain Aggregates, while queries might use optimized projections directly from the infrastructure if needed.

### Screaming Architecture

We organize the code structure, especially in the Application and API layers, around **business features** or domain areas, rather than by technical types (e.g., `Controllers`, `Services`).

*   **Example:** Everything related to orders might reside under `/Features/Orders` (in Application) and `Controllers/OrdersController.cs` (in API).
*   **Benefits:** Makes the application's purpose evident from the project structure, improving code understanding and navigability.

## Key Design Patterns Implemented

In addition to architectural principles, we utilize several specific design patterns:

### Result Pattern

*   **Purpose:** To explicitly represent the outcome of an operation (success or failure) at the Application layer boundary.
*   **Implementation:** 
    * **Domain Layer:** Uses specific domain exceptions (`DomainException`, `ProductNameTooShortException`, etc.) to signal business rule violations and invariants. This keeps the domain focused purely on expressing business rules and state.
    * **Application Layer:** Acts as a boundary that catches domain exceptions and translates them into `Result<T>` or `Result` objects. Command and query handlers return these Result objects, which indicate whether the operation succeeded and, if it failed, provide error details (codes, messages).
*   **Benefits:** 
    * **Clear Separation of Concerns:** Domain focuses on business rules with appropriate exceptions, while Application handles exception translation.
    * **Rich Domain Model:** Domain can express its rules through specific exceptions that clearly communicate the violation.
    * **Consistent API Boundary:** All Application use cases provide a uniform return type for consistent error handling.
    * **Predictable Control Flow:** Prevents exceptions from propagating beyond the Application boundary, improving robustness.

### Repository and Unit of Work Patterns

*   **Repository:** Abstracts data access logic. Provides a collection-like interface for interacting with domain entities, hiding the specifics of the persistence technology (EF Core). Facilitates testability (via mocks).
*   **Unit of Work (UoW):** Groups multiple data access operations into a single atomic transaction. Ensures data consistency by committing or rolling back all changes together. While EF Core (`DbContext`) implicitly implements UoW, an explicit `IUnitOfWork` interface can enhance complex transaction management and testability.

### IOptions Pattern

*   **Purpose:** To manage application configuration in a strongly-typed manner.
*   **Implementation:** POCO classes/records are defined to represent sections of `appsettings.json` or other configuration sources. .NET's dependency injection system binds these and makes them available for injection (`IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`).
*   **Benefits:** Type-safe, testable, and validatable configuration (using Data Annotations or `IValidateOptions`).

## Technology Stack

*   **Framework:** .NET 9
*   **Language:** C# 14
*   **Web API:** ASP.NET Core 9
*   **ORM:** Entity Framework Core 9 (initially with In-Memory provider, prepared for others like SQL Server, PostgreSQL)
*   **Mediator (CQRS):** MediatR
*   **Validation:** FluentValidation
*   **Mapping:** AutoMapper
*   **Logging:** Serilog (configurable)
*   **API Documentation:** Native OpenAPI support in ASP.NET Core + Swagger UI / Scalar
*   **Testing:** xUnit, Moq, FluentAssertions

## Leveraging .NET 9 and C# 14 Features

This project aims to utilize relevant features from the latest versions:

*   **.NET 9:**
    *   **Performance Improvements:** Optimizations in JIT, GC, ARM64 support.
    *   **Native OpenAPI Support:** Built-in OpenAPI specification generation (`Microsoft.AspNetCore.OpenApi`).
    *   **HTTP/3:** Enhancements in network performance.
    *   **`MapStaticAssets` Middleware:** Optimized static file delivery.
    *   *Others:* Serialization improvements, Feature Switches, etc.
*   **C# 14:**
    *   **`field` Keyword:** Simplifies access to auto-generated backing fields in properties.
    *   **Collection Expressions:** Concise syntax for creating collections.
    *   **Primary Constructors:** Boilerplate reduction in classes and structs (already present in records).
    *   **Implicit Span Conversions:** Performance improvements with memory operations.
    *   **`nameof` with unbound generics:** More flexibility.

## Project Structure

The solution is organized following Clean Architecture and Screaming Architecture principles:

```
/YourSolution.sln
|-- src
|   |-- YourSolution.Domain             # Business logic, entities, VOs, events, domain interfaces
|   |-- YourSolution.Application        # Use cases (Commands/Queries by Feature), DTOs, infrastructure interfaces
|   |-- YourSolution.Infrastructure     # Data access implementation (EF Core), external services
|   `-- YourSolution.API                # ASP.NET Core API project, Controllers (by Feature), Middleware, Configuration
|-- tests
|   |-- YourSolution.Domain.Tests       # Unit tests for Domain
|   |-- YourSolution.Application.Tests  # Unit tests for Application (with mocks)
|   |-- YourSolution.Infrastructure.Tests # Integration/unit tests for Infrastructure
|   `-- YourSolution.API.Tests          # Integration tests for API Endpoints
`-- ... (other files like .gitignore, README.md)```

Within `Application` and `API`, organization is based on *Features* (business capabilities) to reflect Screaming Architecture.

## Getting Started

### Prerequisites

*   .NET 9 SDK (Download from [here](https://dotnet.microsoft.com/download/dotnet/9.0))
*   Git

### Cloning

```bash
git clone https://github.com/your-username/your-repository.git # Replace with your repo URL
cd your-repository
```

### Building

```bash
dotnet build
```

### Running the API

Navigate to the API project directory and run:

```bash
cd src/YourSolution.API
dotnet run
```

The API will be available by default at `https://localhost:PORT` or `http://localhost:PORT` (ports are defined in `launchSettings.json`).

You can explore the API and its documentation via:

*   **OpenAPI Specification:** `/openapi/v1/openapi.json`
*   **Swagger UI:** `/swagger` (if configured)
*   **Scalar UI:** `/scalar` (if configured)

### Running Tests

From the solution root directory:

```bash
dotnet test```

## Implementation Highlights

*   **API Documentation with OpenAPI:** Uses .NET 9's built-in support (`AddOpenApi()`, `MapOpenApi()`). Swagger UI (`Swashbuckle.AspNetCore.SwaggerUI`) or Scalar (`Scalar.AspNetCore`) can be easily added for a visual, interactive interface. Endpoints are enriched with metadata (`WithSummary`, `WithDescription`, `[EndpointDescription]`, etc.).
*   **Robust Error Handling:**
    *   **Result Pattern:** Used in the Application layer for expected failures (validation, not found).
    *   **Global Exception Middleware:** Catches unhandled exceptions in the API layer, logs them, and returns a standardized `ProblemDetails` response (RFC 7807).
    *   **Result-to-HTTP Mapping:** Controllers map failure `Result` objects to appropriate HTTP status codes (400, 404, etc.) and `ProblemDetails`.
*   **Validation with FluentValidation:**
    *   Fluent validation rules are defined for commands and queries in the Application layer.
    *   A MediatR **Pipeline Behavior** (`ValidationBehavior`) automatically executes these validations before the handler processes the request. On failure, it returns a `Result.Failure` with the errors.
*   **Mapping with AutoMapper:**
    *   Simplifies mapping between Domain Entities and Application/API DTOs.
    *   Mapping profiles are defined in the Application layer.
    *   Injected `IMapper` or `ProjectTo<T>` (for efficient EF Core projections) is used.
*   **Typed Configuration:** The `IOptions` pattern is used for safe and testable access to configuration. Validation is applied on startup (`ValidateOnStart()`).
*   **Security:**
    *   **Authentication:** Set up for JWT Bearer (or other mechanisms).
    *   **Authorization:** Use of `[Authorize]` and policy-based authorization.
    *   **HTTPS:** Enabled by default.
    *   **Input Validation:** Crucial (FluentValidation).
    *   **Secrets Management:** Recommended use of User Secrets (dev) and Azure Key Vault (prod).
    *   **CORS:** Configuration to restrict allowed origins.
*   **Logging:** Configured with **Serilog** for structured logging. A MediatR Pipeline Behavior (`LoggingBehavior`) can log request ingress/egress. Exception middleware logs unhandled errors.

## Further Considerations

*   **Database:** The project starts with EF Core In-Memory for ease of development. Switching to a persistent database (SQL Server, PostgreSQL) requires updating the provider configuration in `Infrastructure` and managing migrations (`dotnet ef migrations add`, `dotnet ef database update`).
*   **Testing:** The strategy includes unit tests (Domain, Application) and integration tests (Infrastructure, API).
*   **CI/CD:** Setting up a Continuous Integration and Continuous Deployment pipeline (GitHub Actions, Azure Pipelines) early on is recommended.

## Contributing

Contributions are welcome! Please open an issue to discuss significant changes or report bugs. If you'd like to contribute code, please fork the repository and submit a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details. <!-- Ensure you have a LICENSE file -->
