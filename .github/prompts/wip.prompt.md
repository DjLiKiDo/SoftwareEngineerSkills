# Task: Create Complete Unit Test Suite for SoftwareEngineerSkills Solution

I need assistance generating a comprehensive suite of unit tests for all relevant C# files within the SoftwareEngineerSkills solution. The goal is to achieve good test coverage for all layers: Domain, Application, Infrastructure, and API.

Please adhere to all guidelines, coding standards, and architectural patterns detailed in the project's Copilot unit testing instructions:
`@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/.github/prompts/unit-test-instructions.md`

And the general project guidelines:
`@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/.github/copilot-instructions.md`

**Overall Objectives:**

1.  **Identify Testable Files:** Scan the solution (`SoftwareEngineerSkills.sln`) and identify all `.cs` files that are candidates for unit testing. This typically includes:
    *   Domain: Entities, Value Objects, Domain Services (if any).
    *   Application: Command Handlers, Query Handlers, Validators, Application Services.
    *   Infrastructure: Repositories (mocking EF Core or using in-memory for some tests), external service clients (mocking the actual client), utility classes.
    *   API: Controllers (mocking MediatR and other dependencies).
    *   Exclude: Auto-generated files (e.g., `*.designer.cs`), `Program.cs`, `Startup.cs` or `DependencyInjection.cs` (unless they contain complex logic worth testing in isolation, which is rare for DI setup), test projects themselves, and simple DTOs/POCOs with no logic.

2.  **Generate Test Classes:** For each identified candidate file, create a corresponding test class in the appropriate unit test project (e.g., `SoftwareEngineerSkills.Domain.UnitTests`, `SoftwareEngineerSkills.Application.UnitTests`, etc.).
    *   The test class should mirror the namespace and folder structure of the file under test.
    *   Name the test class by appending `Tests` to the original class name (e.g., `CustomerTests.cs`, `CreateCustomerCommandHandlerTests.cs`).

3.  **Generate Test Methods:** Within each test class, generate unit tests for all public methods, constructors, and properties with logic.
    *   Follow the naming convention: `MethodName_Scenario_ExpectedBehavior`.
    *   Cover valid inputs, invalid inputs, edge cases, and different logical paths.
    *   Ensure all dependencies are mocked using Moq.
    *   Use FluentAssertions for all assertions.
    *   Adhere to the Arrange-Act-Assert (AAA) pattern.
    *   Test for correct `Result` pattern outcomes (both `Success` and `Failure` with appropriate values or errors).
    *   Test for expected exceptions where the `Result` pattern is not used (e.g., guard clauses in constructors).

4.  **Specific Focus Areas per Layer:**
    *   **Domain Layer:**
        *   Entities: Test constructors, state-changing methods, validation logic within entities, domain event creation.
        *   Value Objects: Test constructors, equality, validation rules.
    *   **Application Layer:**
        *   Command/Query Handlers: Test the `Handle` method, mock all dependencies (repositories, mappers, services), verify interactions with mocks, assert the `Result` object.
        *   Validators: Test validation rules for commands and queries.
    *   **Infrastructure Layer:**
        *   Repositories: If testing with mocked EF Core, verify correct `DbSet` interactions. If using an in-memory provider, test basic CRUD operations. (Focus on mocking for pure unit tests as per instructions).
        *   Services: Mock external dependencies and verify interactions.
    *   **API Layer (Controllers):**
        *   Action Methods: Mock `IMediator`, verify `Send` is called with the correct command/query, assert the `IActionResult` type and status code, and check the returned value if applicable.

**Process:**

*   Please process the solution systematically, one project layer at a time if that helps manage complexity (e.g., start with Domain, then Application, etc.).
*   For each file, provide the generated test code.
*   If a file is deemed not to require unit tests (e.g., a simple DTO), please state that and briefly explain why.

Refer to the official .NET 9, xUnit, Moq, and FluentAssertions documentation via `context7` if specific new API details or advanced testing scenarios are needed.

Let's start by identifying all candidate C# files for unit testing across the solution, grouped by their respective projects.
