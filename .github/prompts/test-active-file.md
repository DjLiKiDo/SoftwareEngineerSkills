Let's  work on the unit tests for the active class

Refer to .github/instructions/unit-test-instructions.md for the detailed instructions.

1. Implement unit tests for the Domain layer that:
   - Mirror the exact folder/file structure of the Domain project
   - Test all public interfaces, entities, value objects, and domain services
   - Include both positive and negative test scenarios
   - Follow AAA (Arrange-Act-Assert) pattern

2. Achieve minimum test coverage requirements:
   - Overall code coverage: 90% or higher
   - Line coverage: 90% or higher
   - Branch coverage: 90% or higher

3. Test organization:
   - Group tests by domain concept/entity
   - Use descriptive test names following `MethodName_Scenario_ExpectedBehavior` pattern.
   - Create separate test classes for each domain object
   - Place tests in parallel folder structure matching source code

4. Testing guidelines:
   - Focus on business rules and invariants
   - Test domain validations and constraints
   - Verify entity state transitions
   - Ensure domain events are properly raised
   - Test aggregate root behavior

5. Use appropriate testing tools:
   - xUnit for test framework
   - FluentAssertions for readable assertions
   - Moq for mocking dependencies (if needed)

Track and report coverage metrics using the integrated test explorer and coverage tools.