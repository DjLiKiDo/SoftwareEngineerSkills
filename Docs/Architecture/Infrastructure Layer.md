# Infrastructure Layer Implementation

## Overview

The Infrastructure layer provides concrete implementations of the abstractions defined in the Domain layer. It includes implementations for data persistence, external services, logging, caching, and other cross-cutting concerns. This layer is the outermost layer of the Clean Architecture and depends on both the Domain and Application layers.

## Key Components

### 1. Persistence

#### Database Context
- **ApplicationDbContext**: Entity Framework Core context that provides access to the database.
- **Configuration**: Entity configuration using Fluent API to define mappings between domain entities and database tables.

#### Repository Pattern
- **Generic Repository**: `EfRepository<T>` implements common data access operations.
- **Specialized Repositories**: Custom implementations for specific entities with specialized queries.

#### Unit of Work
- **UnitOfWork**: Coordinates operations across multiple repositories and ensures database transaction integrity.

### 2. External Services

#### Email Service
- SMTP-based email service for sending notifications.
- Supports HTML content and attachments.

#### Caching
- Memory and distributed caching implementations.
- Support for various cache providers (In-Memory, Distributed, Redis).

#### Background Processing
- Queue-based background task processing.
- Hosted service for executing background tasks.

### 3. Cross-Cutting Concerns

#### Logging
- Structured logging configuration.
- Support for multiple logging providers.

#### Observability
- Integration with OpenTelemetry for distributed tracing.
- Health checks for monitoring system health.

## Configuration

The Infrastructure layer is configured using the Options pattern with configuration sections in appsettings.json:

```json
{
  "Database": {
    "Provider": "inmemory",
    "ConnectionString": "",
    "EnableDetailedErrors": true
  },
  "Email": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587
  },
  "Cache": {
    "Provider": "memory",
    "DefaultExpirationMinutes": 30
  }
}
```

## Usage

The Infrastructure layer is registered in the DI container through the `AddInfrastructureServices` extension method:

```csharp
// In Program.cs or Startup.cs
services.AddInfrastructureServices(configuration);
```

To use specific services in your application layer:

```csharp
// Example: Using email service
public class NotificationService
{
    private readonly IEmailService _emailService;
    
    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task SendWelcomeEmail(string email, string username)
    {
        await _emailService.SendEmailAsync(
            email,
            "Welcome to our platform!",
            $"Hello {username}, welcome to our platform!",
            isHtml: false);
    }
}
```

## Database Initialization

To initialize the database with migrations and seed data, call:

```csharp
// In Program.cs after building the app
await app.InitializeDatabaseAsync();
```
