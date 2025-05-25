---
applyTo: "**/*Repository.cs,**/*Service.cs,**/Infrastructure/**/*.cs"
---

# Infrastructure Layer Standards for SoftwareEngineerSkills Project

This instruction file provides comprehensive guidelines for implementing infrastructure components including repositories, data access, external service integrations, and cross-cutting concerns for the Development Team Task Board system.

## Project Context

**Domain:** Development Team Task Board
**Architecture:** Clean Architecture with DDD
**Data Access:** Entity Framework Core 9
**Database:** SQL Server (production), SQLite (development/testing)
**External Services:** Email notifications, file storage, identity providers

## Repository Pattern Implementation

### Repository Interface Design Guidelines
- Define generic repository interfaces for common CRUD operations
- Create specialized interfaces for domain-specific queries
- Implement read-only repository interfaces for query-only scenarios
- Design specialized interfaces for soft-delete entity operations
- Use generic constraints to ensure type safety with base entity types
- Include async methods with cancellation token support
- Provide methods for paging, filtering, and counting operations

### Generic Repository Implementation Standards
- Implement generic repository base class for common operations
- Use Entity Framework Core DbContext and DbSet for data access
- Include proper logging for all repository operations
- Implement async methods consistently throughout the repository
- Handle include operations for related entity loading
- Provide both filtered and unfiltered query methods
- Implement proper exception handling and resource disposal 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);
        var items = await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }
}
```

### Specialized Repository Implementation
### Entity-Specific Query Methods
- Implement domain-specific query methods that reflect business operations
- Use meaningful method names that express business intent
- Include related entities using EF Core's `Include` method when needed
- Apply appropriate ordering based on business requirements

### Complex Filtering and Search
- Build dynamic queries using IQueryable for flexible filtering
- Implement search functionality across relevant text fields
- Support date range filtering for time-based queries
- Return paginated results for large datasets

### Performance Considerations
- Use async methods consistently for all database operations
- Apply indexes on frequently queried properties
- Use projections to select only required data when appropriate
- Implement efficient pagination with proper ordering

### Related Entity Handling
- Load related entities only when needed to avoid N+1 problems
- Use explicit loading strategies (Include, ThenInclude) appropriately
- Consider lazy loading implications for performance

## Soft Delete Repository Guidelines

### Soft Delete Interface Design
- Extend generic repository interfaces to include soft-delete operations
- Implement methods to query both active and deleted entities
- Provide hard delete functionality for permanent removal
- Include restoration capabilities for recovering soft-deleted entities

### Query Filter Implementation
- Use EF Core global query filters to automatically exclude soft-deleted entities
- Provide explicit methods to bypass filters when needed
- Handle soft-delete state changes properly in repository methods
- Ensure audit trail consistency with soft-delete operations

### Soft Delete Operations
- Implement proper soft delete marking through domain entity methods
- Handle cascading soft deletes according to business rules
- Provide restoration functionality with proper validation
- Log soft delete and restoration operations for audit purposes

## Unit of Work Pattern Guidelines

### Interface Design
- Define a central interface that aggregates all repository interfaces
- Include transaction management methods for atomic operations
- Provide SaveChanges methods with proper cancellation token support
- Implement IDisposable for proper resource cleanup

### Implementation Standards
- Inject all required repositories through constructor
- Handle domain event dispatching before saving changes
- Implement transaction boundaries for complex operations
- Provide rollback capabilities for failed operations

### Domain Event Integration
- Collect domain events from aggregate roots before saving
- Clear events from entities after dispatching
- Use MediatR or similar for publishing domain events
- Ensure proper ordering of event handling and persistence

### Error Handling and Cleanup
- Properly dispose of database transactions
- Handle SaveChanges failures with appropriate error responses
- Log transaction operations for debugging and monitoring
- Implement proper resource disposal patterns

## DbContext Configuration Guidelines

### Constructor Design
- Inject required services through constructor (current user service, date/time service)
- Accept DbContextOptions for configuration
- Avoid complex logic in constructor

### DbSet Configuration
- Expose domain entities as DbSet properties
- Use descriptive property names that match entity names
- Configure all entities that require persistence

### Model Builder Configuration
- Apply all entity configurations from assembly using ApplyConfigurationsFromAssembly
- Configure global query filters for soft-delete entities automatically
- Override OnModelCreating for custom configurations
- Use reflection to apply consistent patterns across entities

### SaveChanges Override
- Handle audit property updates automatically
- Set Created/CreatedBy for new entities
- Set LastModified/LastModifiedBy for updated entities
- Use injected services to get current user and timestamp information

### Query Filter Implementation
- Automatically exclude soft-deleted entities through global query filters
- Use expression trees to build proper filter conditions
- Apply filters to all entities implementing ISoftDelete interface
- Ensure filters can be bypassed when explicitly needed

## Entity Configuration Guidelines

### Configuration Class Structure
- Implement IEntityTypeConfiguration<T> for each domain entity
- Use descriptive table names that match entity names
- Configure primary keys explicitly using HasKey
- Set up all required properties with appropriate constraints

### Property Configuration
- Set maximum length constraints for string properties
- Configure enum properties with string conversion for readability
- Mark optional properties as IsRequired(false)
- Configure precision and scale for decimal properties

### Relationship Configuration
- Configure foreign key relationships with proper delete behavior
- Use SetNull for optional relationships to prevent cascade deletes
- Use Restrict for required relationships to maintain referential integrity
- Configure many-to-many relationships with explicit join entities when needed

### Owned Entity Configuration
- Use OwnsMany or OwnsOne for value objects that belong to the entity
- Configure owned entity properties with appropriate constraints
- Set up navigation properties for owned entities properly

### Index Configuration
- Create indexes on frequently queried properties
- Add composite indexes for common query patterns
- Consider unique indexes for business key constraints
- Index foreign key properties for relationship queries

### Audit Property Configuration
- Configure audit properties (Created, CreatedBy, LastModified, LastModifiedBy)
- Set appropriate maximum lengths for user identifier fields
- Mark audit properties as required or optional based on business rules

### Domain Event Exclusion
- Ignore domain events from persistence using Ignore() method
- Domain events should not be stored in database
- Events are transient and handled during SaveChanges

## External Service Integration Guidelines

### Service Interface Design
- Define clear interfaces for external service contracts
- Use async methods with cancellation token support
- Include proper error handling for external service failures
- Design interfaces that abstract implementation details

### Service Implementation Standards
- Inject configuration through IOptions pattern
- Use HttpClient for HTTP-based services with proper lifecycle management
- Implement proper logging for all external service interactions
- Handle timeouts and network failures gracefully

### Error Handling and Resilience
- Log all service operations with structured logging
- Implement try-catch blocks for external service calls
- Provide meaningful error messages for service failures
- Consider circuit breaker patterns for unreliable services

### Configuration Management
- Use strongly-typed configuration classes
- Validate configuration at startup
- Use environment-specific configuration files
- Store sensitive information in secure configuration stores

### Email Service Guidelines
- Abstract email sending behind service interfaces
- Support both HTML and plain text email formats
- Include template-based email generation
- Handle email delivery failures with proper logging
- Support both synchronous and asynchronous sending

## Caching Implementation Guidelines

### Cache Service Interface Design
- Define generic cache interfaces that work with any serializable type
- Include methods for Get, Set, Remove operations with async support
- Support expiration policies (absolute, sliding, never expire)
- Provide pattern-based removal methods when supported by cache provider

### Distributed Cache Implementation
- Use IDistributedCache abstraction for cache independence
- Implement JSON serialization for complex objects
- Handle serialization/deserialization errors gracefully
- Log cache operations for monitoring and debugging

### Error Handling Strategy
- Treat cache failures as non-fatal operations
- Return default values when cache retrieval fails
- Log cache errors as warnings rather than errors
- Ensure application continues to function without cache

### Cache Key Management
- Use consistent key naming conventions
- Include version or timestamp information in keys when needed
- Consider key expiration strategies
- Implement cache invalidation patterns for related data

### Performance Considerations
- Use appropriate expiration times based on data volatility
- Consider memory usage when caching large objects
- Implement cache-aside pattern for data loading
- Monitor cache hit ratios and performance metrics

## Performance Considerations

### Connection Pooling
- Configure appropriate connection pool sizes
- Use connection string parameters for optimization
- Monitor connection usage

### Query Optimization
- Use appropriate indexes
- Implement projection for large objects
- Use async methods consistently
- Consider read replicas for query-heavy operations

### Memory Management
- Dispose resources properly
- Use streaming for large datasets
- Implement pagination for large collections
- Consider memory-mapped files for large file operations

## Error Handling and Resilience Guidelines

### Retry Policy Implementation
- Use Polly library for implementing retry policies
- Configure exponential backoff for transient failures
- Set appropriate retry counts based on operation criticality
- Log retry attempts for monitoring and debugging
- Handle specific exception types that warrant retries

### Circuit Breaker Pattern
- Implement circuit breakers for external service calls
- Configure failure thresholds based on service reliability
- Set appropriate break duration for service recovery
- Log circuit breaker state changes for monitoring
- Provide fallback mechanisms when circuit is open

### HTTP Client Resilience
- Add retry policies to HttpClient configurations
- Combine multiple policies (retry + circuit breaker) for robust handling
- Configure timeouts appropriate for each service
- Handle HttpRequestException and TaskCanceledException specifically
- Use named HttpClient configurations for different services

### Logging and Monitoring
- Log all resilience policy activations
- Include correlation IDs for tracking requests across retries
- Monitor policy effectiveness and adjust configurations
- Alert on excessive retry attempts or circuit breaker activations

## Testing Infrastructure Components Guidelines

### Repository Testing Standards
- Use in-memory database or test containers for repository tests
- Create isolated test contexts for each test method
- Seed test data that represents realistic business scenarios
- Test both successful operations and edge cases
- Verify proper entity state changes and relationships

### Test Data Management
- Use builder patterns or object mothers for creating test entities
- Ensure test data isolation between test methods
- Clean up test data properly after each test
- Use meaningful test data that reflects real business scenarios

### Database Testing Strategies
- Use separate database for integration tests
- Apply migrations in test setup
- Test database constraints and relationships
- Verify cascading operations work correctly
- Test query performance with realistic data volumes

### Mock vs Real Dependencies
- Use real database for repository and data access tests
- Mock external services and APIs in infrastructure tests
- Use real DbContext with in-memory provider for unit tests
- Mock time and user services for consistent test results

### Test Organization and Cleanup
- Implement IDisposable for proper resource cleanup
- Group related tests in test collections when needed
- Use descriptive test method names that explain the scenario
- Follow Arrange-Act-Assert pattern consistently
- Ensure tests can run in isolation and in parallel

Remember: Infrastructure components should be focused on technical concerns and should not contain business logic. Keep them simple, testable, and follow the single responsibility principle.
