Generate comprehensive unit tests for the active C# class using xUnit, following Microsoft's recommended best practices:

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

If no C# class is currently open in the editor, respond with:
"Please open the C# class you want to test in the editor."

Reference: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices

use context7 to retrieve documentation and best practices.