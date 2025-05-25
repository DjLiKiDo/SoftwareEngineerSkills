---
applyTo: '"**/*Test.cs" | "**/*Tests.cs"'
---

# GitHub Copilot - Unit Test Generation Instructions (.NET 9, xUnit, Moq, FluentAssertions)

This document provides comprehensive guidelines for generating high-quality unit tests for .NET 9 projects following enterprise standards and best practices.

## 1. Core Testing Principles

*   **Single Responsibility:** Each test should verify one specific behavior or scenario
*   **Isolation:** Mock all external dependencies to ensure deterministic, independent tests
*   **Readability:** Structure tests using the Arrange-Act-Assert (AAA) pattern with clear intent
*   **Naming Convention:** Use `MethodName_Scenario_ExpectedBehavior` pattern for descriptive test names
*   **Logical Assertions:** Group related assertions that verify a single logical outcome
*   **Simplicity:** Avoid complex logic, loops, or conditional statements within test methods
*   **Determinism:** Tests must produce consistent results across all environments and executions

## 2. xUnit Testing Framework Guidelines

*   **Test Attributes:** Use `[Fact]` for single-scenario tests and `[Theory]` with data attributes for parameterized tests
*   **Test Lifecycle:** Utilize constructor for test setup and `IDisposable` for cleanup when needed
*   **Shared Resources:** Implement `IClassFixture<T>` for expensive setup shared across test methods
*   **Data Sources:** Leverage `[InlineData]`, `[MemberData]`, or `[ClassData]` for parameterized test scenarios

## 3. Moq Mocking Guidelines

*   **Mock Creation:** Create mock objects for all external dependencies of the system under test
*   **Method Setup:** Configure return values and behaviors for mocked methods using appropriate matchers
*   **Property Setup:** Mock property getters and setters when behavior depends on property values
*   **Verification:** Verify expected interactions with dependencies occurred with correct parameters
*   **Strict Behavior:** Consider strict mock behavior when precise interaction verification is required
*   **Async Support:** Use async-compatible setup methods for mocking asynchronous operations

## 4. FluentAssertions Testing Guidelines

*   **Readable Assertions:** Use fluent syntax to create self-documenting test assertions
*   **Object Comparison:** Compare complex objects using equivalency assertions with exclusion options
*   **Collection Assertions:** Verify collection contents, counts, and ordering as appropriate
*   **Exception Testing:** Assert exception types and messages for error scenarios
*   **Result Pattern Testing:** Verify success/failure states and associated values/errors
*   **Type Assertions:** Confirm object types and inheritance relationships
*   **String Assertions:** Test string content with pattern matching and case sensitivity options

## 5. Test Organization Standards

*   **Project Structure:** Maintain separate test projects for each application layer with corresponding namespaces
*   **File Organization:** Mirror the folder structure of the system under test within test projects
*   **Test Classes:** Create dedicated test classes for each class being tested with descriptive naming
*   **Test Grouping:** Organize related tests using nested classes or shared test fixtures when appropriate

## 6. Testing Coverage Guidelines

*   **Public Contracts:** Focus testing on public methods and behaviors that define the component's interface
*   **Business Logic:** Prioritize testing of core domain logic, command handlers, and query handlers
*   **Decision Points:** Test all conditional branches and edge cases within business logic
*   **Error Scenarios:** Verify proper handling of invalid inputs and exceptional conditions
*   **State Changes:** Test methods that modify object state and verify resulting domain events
*   **Layer-Specific Testing:**
    *   **Domain Layer:** Entity construction, business rules, value object equality, domain events
    *   **Application Layer:** Command/query handlers, validation logic, result patterns
    *   **Infrastructure Layer:** Repository implementations, external service integrations
    *   **API Layer:** Controller actions, HTTP status codes, request/response mapping

## 7. Test Implementation Patterns

### Command Handler Testing
*   **Setup:** Mock repository dependencies, unit of work, and mapping services
*   **Verification:** Assert successful result creation and verify repository interactions
*   **Failure Cases:** Test validation failures and business rule violations

### Domain Entity Testing
*   **Construction:** Test valid object creation and validation of constructor parameters
*   **Behavior:** Verify state changes and domain event generation
*   **Invariants:** Test business rule enforcement and constraint validation

### Value Object Testing
*   **Equality:** Verify value-based equality semantics and hash code consistency
*   **Immutability:** Ensure objects cannot be modified after creation
*   **Validation:** Test input validation and constraint enforcement

### Controller Testing
*   **Mediator Integration:** Mock mediator and verify correct command/query dispatch
*   **Response Mapping:** Assert proper HTTP status codes and response object creation
*   **Error Handling:** Test failure scenarios and error response formatting

## 8. Asynchronous Testing Guidelines

*   **Async Methods:** Mark test methods as `async Task` when testing asynchronous operations
*   **Await Usage:** Use `await` consistently when calling async methods under test
*   **Exception Testing:** Use async-compatible assertion methods for exception scenarios
*   **Cancellation:** Test cancellation token handling in long-running operations

## 9. Result Pattern Testing Standards

*   **Success Scenarios:** Verify `IsSuccess` state and validate returned values
*   **Failure Scenarios:** Assert `IsFailure` state and examine error information
*   **Error Details:** Test error message content and error type classification
*   **Business Rules:** Verify domain-specific error conditions and validation failures

## 10. Best Practices and Standards

*   **Architectural Alignment:** Follow Clean Architecture dependency rules and DDD principles
*   **Naming Consistency:** Use project naming conventions and maintain consistent test organization
*   **Documentation:** Include XML documentation for complex test scenarios
*   **Performance:** Keep tests fast and avoid unnecessary I/O operations in unit tests

## 11. Reference Documentation

*   **xUnit:** Official testing framework for .NET with extensive documentation
*   **Moq:** Popular mocking framework for .NET applications
*   **FluentAssertions:** Assertion library providing readable test assertions
*   **.NET Testing:** Microsoft's official guidance on testing .NET applications
*   **Context7:** Use available tools to query official documentation for specific API details or advanced scenarios

## 12. Quality Standards

*   **Code Coverage:** Aim for high test coverage while prioritizing meaningful tests over metrics
*   **Test Maintenance:** Write tests that are easy to maintain and update as requirements evolve
*   **Documentation:** Include clear comments for complex test scenarios and business rule validations
*   **Performance:** Ensure tests execute quickly and can be run frequently during development
