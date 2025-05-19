Task: Implement Domain-Driven Design Core Components

Technical Requirements:
1. Create foundational domain model classes in the Core/Domain layer:
   - BaseEntity: Abstract base class with Id and common entity behavior
   - ValueObject: Abstract class for immutable value types with equality comparison
   - AggregateRoot: Base class extending BaseEntity with domain event handling

2. Define the IAggregateRoot interface:
   - Include methods for tracking and publishing domain events
   - Add versioning/concurrency control properties
   - Implement clear boundary enforcement mechanisms

3. Implement invariant validation system:
   - Create CheckInvariants method for validation logic
   - Add EnforceInvariants method to be called after state changes
   - Throw DomainValidationException for violations
   - Support both synchronous and async validation

Expected Deliverables:
- Clean, well-documented base classes in /src/SoftwareEngineerSkills.Domain
- Unit tests demonstrating proper invariant enforcement
- XML documentation for public APIs
- Example usage in README.md

Architecture Constraints:
- No external dependencies beyond core .NET libraries
- Follow SOLID principles and DDD best practices
- Ensure thread safety for concurrent operations
- Maximum cyclomatic complexity of 5 per method

#fetch Reference: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/

Priority: High - Blocking task for domain model implementation