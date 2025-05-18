# .NET 9 Enterprise Web API Template

[![Build Status](https://github.com/yourusername/SoftwareEngineerSkills/actions/workflows/dotnet.yml/badge.svg)](https://github.com/yourusername/SoftwareEngineerSkills/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)

A production-ready .NET 9 Web API template implementing enterprise-level best practices for building scalable, maintainable, and secure APIs following Clean Architecture and Domain-Driven Design principles.

## Table of Contents
- [Architecture Overview](#architecture-overview)
- [Key Features](#key-features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Design Patterns & Principles](#design-patterns--principles)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Observability](#observability)
- [Security](#security)
- [Performance Optimization](#performance-optimization)
- [CI/CD & DevOps](#cicd--devops)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)

## Architecture Overview

This template implements **Clean Architecture** with a focus on **Domain-Driven Design** principles and **Screaming Architecture**. The solution is organized into multiple layers with clear separation of concerns:

```
├── src
│   ├── Core
│   │   ├── Domain              # Entities, Value Objects, Domain Events
│   │   └── Application         # Business Logic, Commands, Queries
│   ├── Infrastructure          # Data Access, External Services
│   └── Presentation
│       └── WebApi              # Controllers, Middleware, Configuration
└── tests
    ├── UnitTests
    ├── IntegrationTests
    └── FunctionalTests
```

## Key Features

### Core Technologies
- **.NET 9 / ASP.NET Core** - Latest framework version
- **C# 14+** - Modern language features
- **Entity Framework Core 9** - Data access (abstracted via Repository Pattern)

### Architecture & Design Patterns
- **Clean Architecture** with DDD principles and Screaming Architecture
- **CQRS with MediatR** for separating read and write operations
- **Repository Pattern with Unit of Work** for data access abstraction
- **IOptions Pattern** for strongly-typed configuration management
- **Result Pattern** for exception-free error handling
- **Entity Auditing System** with sophisticated tracking and soft delete capabilities

### API Documentation & Versioning
- **OpenAPI/Swagger** using Swashbuckle for API documentation
- **API Versioning** support for backward compatibility

### Testing Framework
- **xUnit** for unit and integration tests
- **Moq** for mocking dependencies
- **FluentAssertions** for readable test assertions
- **Testcontainers** for integration testing with real dependencies

### Observability Stack
- **Structured logging** with Serilog
- **OpenTelemetry** for distributed tracing
- **Health checks** implementation for monitoring
- **Metrics exposure** (Prometheus/AppMetrics)
- **Correlation ID tracking** across service calls

### Security Features
- **JWT/OAuth2 authentication** and authorization
- **HTTPS enforcement** with proper configuration
- **FluentValidation** for comprehensive input validation
- **CORS policy** configuration
- **Secrets management** solution

### Performance Optimizations
- **Async/await** implementation throughout
- **Caching strategy** (In-memory/Redis)
- **Response compression** middleware
- **Rate limiting** implementation

### Operational Features
- **Dependency injection** configuration
- **Background job processing**
- **Feature flags** implementation
- **Resilience patterns** using Polly
- **Docker containerization**
- **CI/CD pipeline** with GitHub Actions

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerization)
- IDE of choice ([Visual Studio 2022+](https://visualstudio.microsoft.com/vs/), [VS Code](https://code.visualstudio.com/), [JetBrains Rider](https://www.jetbrains.com/rider/))

### Installation

1. Clone this repository:
```bash
git clone https://github.com/yourusername/SoftwareEngineerSkills.git
cd SoftwareEngineerSkills
```

2. Build the solution:
```bash
dotnet build
```

3. Run the API:
```bash
cd src/Presentation/WebApi
dotnet run
```

4. Navigate to `https://localhost:5001/swagger` to view the API documentation.

### Configuration

The template uses the IOptions pattern for configuration management:

1. Update settings in `appsettings.json` or use environment-specific files like `appsettings.Development.json`
2. For sensitive data, use:
   - User Secrets during development (`dotnet user-secrets`)
   - Environment variables
   - Azure Key Vault or other secret management solutions for production

## Project Structure

The solution follows Clean Architecture principles with these main projects:

### Core Layer
- **Domain Project**: Contains entities, value objects, enums, exceptions, interfaces, and domain events
- **Application Project**: Contains business logic, commands/queries (CQRS), validators, and application services

### Infrastructure Layer
- **Persistence Project**: EF Core configurations, repositories, migrations, and data access
- **Infrastructure Project**: External service implementations, logging, caching, messaging, etc.

### Presentation Layer
- **WebApi Project**: Controllers, filters, middleware, API versioning, and Swagger configuration

### Tests
- **UnitTests**: For testing individual components in isolation
- **IntegrationTests**: For testing components with their dependencies
- **FunctionalTests**: For testing the API endpoints from client perspective

## Design Patterns & Principles

### Clean Architecture
The solution is structured around the dependency rule where dependencies flow inward, with the Domain at the center. External concerns like UI, database, and third-party services are on the outer layers.

### Domain-Driven Design
- **Entities**: Domain objects with identity and lifecycle
- **Value Objects**: Immutable domain objects without identity
- **Domain Events**: For side-effect processing
- **Aggregates**: Cluster of domain objects treated as a single unit

### CQRS (Command Query Responsibility Segregation)
- **Commands**: For write operations that change state
- **Queries**: For read operations that return data without changing state
- **MediatR**: Used to dispatch commands and queries

### Repository Pattern
- Abstracts data access
- Provides collection-like interface for domain entities
- Coupled with Unit of Work for transaction management

### Result Pattern
- Eliminates exceptions for expected error paths
- Returns rich result objects with status, errors, and data

## API Documentation

API documentation is automatically generated using Swagger/OpenAPI:

- **Swagger UI**: Available at `/swagger` endpoint
- **API Versioning**: Supports URL path, query string, and header versioning
- **XML Comments**: Used to document API endpoints and models

## Testing

### Unit Testing
- Uses xUnit as the test framework
- Moq for creating test doubles
- FluentAssertions for more readable assertions

### Integration Testing
- Testcontainers for testing with real database and dependencies
- In-memory database for faster tests
- Respects application boundaries

### Functional Testing
- Tests the entire application stack
- Uses WebApplicationFactory to bootstrap the application
- Real HTTP requests against in-memory test server

## Observability

### Logging
- Structured logging with Serilog
- Multiple sinks configuration (Console, File, Elasticsearch)
- Log enrichment with contextual information

### Tracing
- OpenTelemetry integration for distributed tracing
- Trace context propagation
- Exporters for Jaeger/Zipkin/other tracing systems

### Health Checks
- Endpoint: `/health` provides system health
- Custom health checks for critical dependencies
- Integration with monitoring systems

### Metrics
- Exposes application metrics in Prometheus format
- Custom metrics for business KPIs
- Dashboard templates included

## Security

### Authentication & Authorization
- JWT Bearer token authentication
- Role-based and policy-based authorization
- OAuth2/OpenID Connect support

### Data Protection
- Input validation using FluentValidation
- Output sanitization to prevent XSS
- HTTPS enforcement
- Proper CORS configuration

### Security Headers
- Content Security Policy
- X-Content-Type-Options
- X-Frame-Options
- And other security headers

## Performance Optimization

### Caching
- Response caching middleware
- Distributed cache abstraction with Redis implementation
- Cache invalidation strategy

### Asynchronous Processing
- Async/await usage throughout the codebase
- Task-based Asynchronous Pattern
- Background job processing for long-running tasks

### Efficiency
- Response compression
- Rate limiting to protect resources
- Pagination for large result sets

## CI/CD & DevOps

### Containerization
- Dockerfile and docker-compose configuration
- Multi-stage builds for optimized images
- Container orchestration support (Kubernetes manifests)

### GitHub Actions
- Build and test workflows
- Code quality analysis
- Security scanning
- Automated deployments

### Infrastructure as Code
- Deployment templates for Azure/AWS
- Environment configuration
- Resource provisioning scripts

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure your code adheres to our coding standards and includes appropriate tests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you need help with this template, please:

- Open an issue on GitHub
- Contact the maintainers at [example@example.com](mailto:example@example.com)
- Join our community on [Discord](https://discord.gg/example)

---

## 🌟 Star this repository if you find it useful! 🌟
