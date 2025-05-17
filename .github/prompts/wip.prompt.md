Create a production-ready .NET 9 Web API Template implementing enterprise-level best practices and industry standards. The template should serve as a foundation for building scalable, maintainable, and secure APIs.

Architecture & Design Patterns:
- Clean Architecture with DDD principles and Screaming Architecture
- CQRS with MediatR
- Repository Pattern with Unit of Work
- IOptions Pattern for configuration management 
- Result Pattern for exception-free error handling

Technical Requirements:

1. Core Technologies:
   - .NET 9 / ASP.NET Core
   - C# 14+
   - Entity Framework Core 9

2. API Documentation & Versioning:
   - OpenAPI/Swagger using Swashbuckle
   - API versioning support

3. Testing Framework:
   - xUnit for unit and integration tests
   - Moq for mocking
   - FluentAssertions for assertions
   - Testcontainers for integration testing

4. Observability Stack:
   - Structured logging with Serilog
   - OpenTelemetry for distributed tracing
   - Health checks implementation
   - Metrics exposure (Prometheus/AppMetrics)
   - Correlation ID tracking

5. Security Features:
   - JWT/OAuth2 authentication
   - HTTPS enforcement
   - FluentValidation for input validation
   - CORS policy configuration
   - Secrets management solution

6. Performance Optimizations:
   - Async/await implementation
   - Caching strategy (In-memory/Redis)
   - Response compression
   - Rate limiting middleware

7. Operational Features:
   - Dependency injection configuration
   - Background job processing
   - Feature flags implementation
   - Resilience patterns using Polly
   - Docker containerization
   - CI/CD pipeline with GitHub Actions

The solution should include documentation, examples, and best practices for extending the template for specific business requirements.