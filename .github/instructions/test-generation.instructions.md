---
applyTo: '"**/*Test.cs" | "**/*Tests.cs"'
---

# GitHub Copilot - Unit Test Generation Instructions for SoftwareEngineerSkills Project (.NET 9, xUnit, Moq, FluentAssertions)

This document provides detailed instructions for GitHub Copilot to generate high-quality unit tests for the SoftwareEngineerSkills project. These tests should adhere to .NET 9 best practices, utilizing xUnit as the testing framework, Moq for mocking dependencies, and FluentAssertions for expressive assertions.

## 1. Core Principles for Unit Testing

*   **Target:** Unit tests should focus on a single unit of work (e.g., a method or a small group of related methods within a class).
*   **Isolation:** Dependencies of the unit under test (SUT) must be mocked or stubbed to ensure tests are isolated and deterministic. Use Moq for this.
*   **Readability:** Tests should be easy to read and understand. Follow the Arrange-Act-Assert (AAA) pattern.
*   **Naming Convention:**
    *   Use the `MethodName_Scenario_ExpectedBehavior` pattern.
    *   Example: `Handle_ValidCommand_ShouldCreateCustomerAndReturnSuccessResult`
    *   Example: `Constructor_NullArgument_ShouldThrowArgumentNullException`
*   **One Assertion (Logical):** While multiple physical assertions (e.g., checking multiple properties of a DTO) are acceptable if they verify a single logical outcome, strive to test one specific aspect or behavior per test method.
*   **No Logic in Tests:** Avoid conditional logic (if/else, switch), loops, or complex computations within test methods. Tests should be straightforward.
*   **Repeatability:** Tests must be repeatable and produce the same result every time they are run, regardless of the environment.

## 2. xUnit Framework Usage

*   **Test Methods:** Mark test methods with the `[Fact]` attribute for parameterless tests.
*   **Parameterized Tests:** Use `[Theory]` with `[InlineData]`, `[MemberData]`, or `[ClassData]` for tests that run with different inputs.
*   **Setup and Teardown:**
    *   Use the constructor for per-test setup.
    *   Implement `IDisposable` for per-test cleanup if needed (xUnit automatically calls `Dispose`).
    *   For shared setup/teardown across tests in a class, use xUnit's fixture mechanisms (`IClassFixture<T>`).

## 3. Moq for Mocking

*   **Creating Mocks:** `var mock = new Mock<IDependency>();`
*   **Setting up Methods:**
    *   `mock.Setup(d => d.Method(It.IsAny<string>())).Returns(expectedValue);`
    *   `mock.Setup(d => d.MethodAsync(It.IsAny<int>())).ReturnsAsync(expectedValue);`
    *   `mock.Setup(d => d.VoidMethod(It.IsAny<Input>()));` (for void methods)
    *   `mock.Setup(d => d.MethodWithOutput(out outVar)).Returns(true);` (for methods with out parameters)
*   **Setting up Properties:** `mock.SetupGet(d => d.Property).Returns(value);`
*   **Verifying Calls:**
    *   `mock.Verify(d => d.Method(It.Is<string>(s => s.Length > 0)), Times.Once());`
    *   `mock.Verify(d => d.VoidMethod(input), Times.Exactly(2));`
*   **Strict Mocks:** Consider `MockBehavior.Strict` for mocks where any unconfigured call results in an exception, ensuring all interactions are explicitly defined.
*   **Injecting Mocks:** Pass mock objects (`mock.Object`) to the constructor of the System Under Test (SUT).

## 4. FluentAssertions for Assertions

*   **Clarity:** FluentAssertions provide more readable and descriptive assertions compared to traditional `Assert` classes.
*   **Common Assertions:**
    *   `result.Should().Be(expected);`
    *   `actual.Should().NotBeNull();`
    *   `collection.Should().Contain(item);`
    *   `collection.Should().HaveCount(expectedCount);`
    *   `collection.Should().BeEmpty();`
    *   `stringResult.Should().StartWith("prefix").And.EndWith("suffix");`
    *   `objectInstance.Should().BeOfType<ExpectedType>();`
    *   `dto.Should().BeEquivalentTo(expectedDto, options => options.Excluding(x => x.Id));` (for comparing complex objects, useful for DTOs)
*   **Exception Testing:**
    *   `Action act = () => sut.MethodThatThrows();`
    *   `act.Should().Throw<SpecificExceptionType>().WithMessage("Expected message part*");` (use wildcards for message matching if needed)
    *   `Func<Task> actAsync = async () => await sut.AsyncMethodThatThrows();`
    *   `await actAsync.Should().ThrowAsync<SpecificExceptionType>();`
*   **Result Pattern Testing (referencing project's Result pattern):**
    *   `result.IsSuccess.Should().BeTrue();`
    *   `result.IsFailure.Should().BeTrue();`
    *   `result.Value.Should().NotBeNull();` (if success)
    *   `result.Value.Should().BeEquivalentTo(expectedDto);` (if success and value is an object)
    *   `result.Error.Should().NotBeNull();` (if failure)
    *   `result.Error.Message.Should().Contain("specific error message");` (if failure)

## 5. Project Structure and Test Organization

*   **Test Projects:** Each main project (Domain, Application, Infrastructure, API) should have a corresponding unit test project.
    *   `SoftwareEngineerSkills.Domain.UnitTests`
    *   `SoftwareEngineerSkills.Application.UnitTests`
    *   `SoftwareEngineerSkills.Infrastructure.UnitTests`
    *   `SoftwareEngineerSkills.API.UnitTests`
*   **Namespace and Folder Structure:** Mirror the folder and namespace structure of the SUT within the test project.
    *   Example: Tests for `SoftwareEngineerSkills.Application.Features.Customers.Commands.CreateCustomerCommandHandler` should be in `SoftwareEngineerSkills.Application.UnitTests/Features/Customers/Commands/CreateCustomerCommandHandlerTests.cs`.

## 6. What to Test

*   **Public API:** Focus on testing the public contract of your classes.
*   **Business Logic:** Core domain logic, command handlers, query handlers.
*   **Conditional Logic:** Test all branches of conditional statements.
*   **Loops:** Test with zero, one, and multiple iterations if behavior varies.
*   **Edge Cases:** Null inputs, empty collections, boundary values.
*   **Error Handling:** Verify correct exceptions are thrown or `Result` failures are returned for invalid inputs or states.
*   **Domain Entities & Value Objects:**
    *   Test constructors for valid and invalid states.
    *   Test methods that change state.
    *   Test domain event emissions.
    *   For Value Objects, test equality components and validation logic.
*   **Application Layer (Commands/Queries):**
    *   Test `Handle` methods of command and query handlers.
    *   Mock dependencies (repositories, mappers, other services).
    *   Verify interactions with mocks.
    *   Assert the `Result` object (success/failure, value, error).
    *   Test validators associated with commands/queries.
*   **Infrastructure Layer:**
    *   Test repository implementations (can be more integration-like if hitting an in-memory DB, or mock EF Core contexts for pure unit tests).
    *   Test service integrations (mock external clients).
*   **API Layer (Controllers):**
    *   Test action methods.
    *   Mock `IMediator`.
    *   Verify `_mediator.Send()` is called with the correct command/query.
    *   Assert the `IActionResult` returned (e.g., `OkObjectResult`, `NotFoundResult`, `CreatedAtActionResult`).
    *   Check status codes.

## 7. Specific Examples (from `copilot-instructions.md`)

Refer to the `CreateCustomerCommandHandlerTests` example in the main `copilot-instructions.md` for a practical illustration of testing a command handler using Moq and FluentAssertions. Adapt this pattern for other components.

```csharp
// Unit Test Example (from copilot-instructions.md - ensure this is consistent)
public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock; // Assuming ICustomerRepository is part of IUnitOfWork or directly used
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepositoryMock = new Mock<ICustomerRepository>(); // Or setup specific repo from UoW
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock.Setup(uow => uow.Customers).Returns(_customerRepositoryMock.Object);
        // If CustomerRepository is not directly on IUnitOfWork, adjust setup.
        // Example: _unitOfWorkMock.Setup(uow => uow.GetRepository<ICustomerRepository>()).Returns(_customerRepositoryMock.Object);

        _handler = new CreateCustomerCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCustomerAndReturnSuccessResult() // Adjusted name for clarity
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = "Test Customer",
            Email = "test@example.com"
        };

        var customerDto = new CustomerDto { Id = Guid.NewGuid(), Name = command.Name, Email = command.Email };

        _mapperMock
            .Setup(m => m.Map<CustomerDto>(It.IsAny<Customer>()))
            .Returns(customerDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(customerDto);

        _customerRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Customer>(c => c.Name == command.Name && c.EmailAddress.Value == command.Email), It.IsAny<CancellationToken>()), // More specific It.Is<>
            Times.Once);

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = "Test Customer",
            Email = "invalid-email" // Assuming Email value object throws BusinessRuleException
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        // This assumes your Email value object or Customer constructor throws a BusinessRuleException
        // which is caught and translated to a Result.Failure in the handler.
        // Adjust the expected message based on your actual implementation.
        result.Error.Message.Should().Contain("Invalid email format"); 

        _customerRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never); // Ensure customer is not added

        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never); // Ensure SaveChanges is not called
    }
}
```

## 8. Async Testing

*   Test methods returning `Task` or `Task<T>` should be `async Task`.
*   Use `await` when calling async methods under test.
*   Use `await actAsync.Should().ThrowAsync<...>()` for async methods that throw exceptions.

## 9. Adherence to General Project Guidelines

*   Follow all coding conventions, naming standards, and architectural patterns defined in the main `copilot-instructions.md` for the SoftwareEngineerSkills project.
*   Pay special attention to the **Result Pattern** for error handling. Tests should verify both success and failure paths of the Result pattern.

## 10. Documentation and Official Resources

*   **xUnit:** [https://xunit.net/](https://xunit.net/)
*   **Moq:** [https://github.com/moq/moq](https://github.com/moq/moq)
*   **FluentAssertions:** [https://fluentassertions.com/](https://fluentassertions.com/)
*   **.NET Testing:** [https://docs.microsoft.com/en-us/dotnet/core/testing/](https://docs.microsoft.com/en-us/dotnet/core/testing/)

Use `context7` to query official documentation for specific API details or advanced scenarios if needed.
