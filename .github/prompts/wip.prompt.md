---
mode: 'agent'
tools: ['codebase', 'file', 'fetch', 'context7']
description: 'Work-in-progress development tasks with full project context'
---

# Work-in-Progress Development Assistant

This prompt provides comprehensive support for ongoing development tasks in the SoftwareEngineerSkills .NET 9 enterprise project.

## Project Context

You are working on a **Development Team Task Board** system built with:
- **.NET 9** with **Clean Architecture** and **Domain-Driven Design**
- **Entity Framework Core 9** with advanced domain patterns
- **CQRS** with MediatR for command/query separation
- **Rich domain model** with aggregates, value objects, and domain events

## Key Project Guidelines

### Architecture References
- [Complete Code Generation Standards](../instructions/code-generation.instructions.md)
- [Project Instructions](../copilot-instructions.md)

### References
- [README](../../README.md)
- [Domain](../../SoftwareEngineerSkills/src/SoftwareEngineerSkills.Domain/README.md)
- [Application](../../SoftwareEngineerSkills/src/SoftwareEngineerSkills.Application/README.md)
- [Infrastructure](../../SoftwareEngineerSkills/src/SoftwareEngineerSkills.Infrastructure/README.md)
- [API](../../SoftwareEngineerSkills/src/SoftwareEngineerSkills.API/README.md)

## Development Assistance

### What I Can Help With

1. **Create New Components**
   - Domain entities following DDD patterns
   - CQRS commands and queries with handlers
   - API controllers with proper HTTP semantics
   - Value objects with immutable design
   - Domain services for cross-aggregate operations

2. **Implement Patterns**
   - Clean Architecture layer separation
   - Repository pattern with Unit of Work
   - Result pattern for error handling
   - Domain events and event handlers
   - Specification pattern for complex queries

3. **Generate Tests**
   - Unit tests with xUnit, Moq, FluentAssertions
   - Integration tests with Testcontainers
   - Domain entity behavior testing
   - API endpoint testing with proper setup

4. **Apply Best Practices**
   - Entity Framework configuration
   - FluentValidation for input validation
   - Proper async/await patterns
   - Security and performance optimizations

### Code Generation Standards

I will automatically apply these standards from our instruction files:

#### Domain Layer
- Use `AggregateRoot` for main business entities with thread-safe event handling
- Use `BaseEntity` for simple entities within aggregates
- Use `SoftDeleteEntity` when soft deletion is required
- Implement business behavior through methods, not property setters
- Add domain events for important state changes
- Include comprehensive invariant validation

#### Application Layer  
- Implement CQRS with clear command/query separation
- Use FluentValidation for all input validation
- Return Result objects instead of throwing exceptions
- Implement proper error handling and logging

#### API Layer
- Follow RESTful conventions with proper HTTP status codes
- Implement API versioning and OpenAPI documentation
- Use proper async patterns throughout
- Include comprehensive error handling middleware

## Quick Start Examples

### Create a New Entity
```
Create a Project entity that manages a collection of tasks with start/end dates, status tracking, and project manager assignment. Include proper domain events and business rules.
```

### Add a Feature
```
Implement task assignment functionality that validates developer skills against task requirements, updates workload tracking, and sends domain events for notifications.
```

### Generate Tests
```
Create comprehensive unit tests for the Task entity focusing on the assignment logic, skill validation, and domain event generation.
```

### Refactor Code
```
Refactor the existing Developer class to use the Repository pattern and improve the skill management methods following our DDD guidelines.
```

## Current Development Context

When working on tasks, I will:

1. **Analyze Requirements** - Understand the business need
2. **Apply Architecture** - Follow Clean Architecture principles  
3. **Implement Patterns** - Use established DDD and CQRS patterns
4. **Generate Quality Code** - Include validation, events, and proper error handling
5. **Create Tests** - Provide comprehensive test coverage
6. **Document Changes** - Include XML comments and clear naming

## Advanced Scenarios

### Complex Business Rules
For implementing sophisticated business logic, I'll use:
- Specification pattern for complex validations
- Domain services for cross-aggregate operations  
- Policy pattern for configurable business rules
- Event sourcing for audit requirements

### Performance Optimization
When performance is critical, I'll apply:
- Proper EF Core query patterns with projections
- Caching strategies for read-heavy operations
- Async patterns throughout the stack
- Pagination for large data sets

### Integration Patterns
For external system integration, I'll implement:
- Adapter pattern for external services
- Circuit breaker for resilience
- Outbox pattern for reliable event publishing
- Background services for long-running operations

## Ready to Assist

I'm ready to help with any development task. Just describe what you need, and I'll:

✅ Apply all project standards automatically  
✅ Generate enterprise-quality code  
✅ Include comprehensive error handling  
✅ Follow Clean Architecture principles  
✅ Implement proper DDD patterns  
✅ Create appropriate tests  
✅ Provide clear documentation  

**What would you like to work on today?**