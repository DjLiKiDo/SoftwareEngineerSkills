Improve my unt tests and coverage

use context7 to query the knowledge from the official documentation.
Run the unit test suite in Visual Studio Code
Address any failures found
   - Review test failures and error messages
   - Fix test implementation issues without modifying application code
   - Re-run tests to verify fixes

Once all tests pass:
   - Generate and analyze code coverage report
   - Identify untested code paths or scenarios
   - Add missing test cases to achieve adequate coverage

Requirements:
1. Test Framework & Libraries:
   - xUnit for test framework
   - Moq for mocking dependencies
   - FluentAssertions for assertions

2. Test Structure:
   - Use [Fact] or [Theory] attributes for test methods
   - Follow Arrange-Act-Assert pattern
   - Name tests as [TestedMethod]_[Scenario]_[ExpectedResult]
   - Create test class named [TestedClass]Tests
   - Use constructor or test collection fixtures for shared setup
   - Maintain test isolation

3. Coverage Requirements:
   - Test all public methods and properties
   - Verify constructor parameter validation
   - Test null/empty/invalid inputs
   - Cover happy path and error scenarios
   - Validate exception handling
   - Test boundary conditions
   - Coverage >= 80% for all methods

4. Implementation Guidelines:
   - Mock external dependencies using Moq
   - Use meaningful test data (avoid magic strings/numbers)
   - Write focused, single-purpose tests
   - Add // Arrange, // Act, // Assert comments
   - Document complex test scenarios
   - Use static test data or constants for repeated values
   - Replicate proyect structure in test project

#fetch Reference: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices

Note: Focus on improving test quality and coverage while preserving existing application functionality, unless the changes are necessary to fix the tests and improves the implementation
