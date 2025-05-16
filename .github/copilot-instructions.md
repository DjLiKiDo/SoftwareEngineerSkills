\
# Copilot Instructions for the SoftwareEngineerSkills .NET Project

This document provides specific instructions for GitHub Copilot to optimize AI assistance for the .NET project located in the `SourceCode/SoftwareEngineerSkills` directory.

## 1. Project Definition

### Primary Domain and Tech Stack
- **Primary Domain:** A RESTful API to manage and expose software engineering skills-related data.
- **Tech Stack:**
    - **Language:** C# (latest version, leveraging C# 12+ features)
    - **Framework:** .NET 9
    - **Web Framework:** ASP.NET Core for .NET 9
    - **Data Access:** Entity Framework Core for .NET 9
    - **Testing:** xUnit (for unit and integration tests), Moq (for mocking dependencies).
    - **API Documentation:** Swashbuckle for OpenAPI/Swagger (inferred from `OpenApiExtensions.cs`).

### Key Architectural Patterns
- **Layered Architecture:** The project is structured into distinct layers:
    - `SoftwareEngineerSkills.API`: Handles HTTP requests, responses, and API-specific concerns.
    - `SoftwareEngineerSkills.Application`: Contains business logic, use cases, and application services.
    - `SoftwareEngineerSkills.Domain`: Includes domain entities, aggregates, value objects, and domain-specific logic.
    - `SoftwareEngineerSkills.Infrastructure`: Implements external concerns like database access, external service integrations.
    - `SoftwareEngineerSkills.Common`: Shared utilities like `Result` and `Error` objects.
- **RESTful APIs:** Expose resources via standard HTTP methods (GET, POST, PUT, DELETE).
- **Dependency Injection (DI):** Heavily utilized throughout the project, configured in `DependencyInjection.cs` files within each layer and `Program.cs`. Consider using C# 12 primary constructors for concise DI in services and controllers.
- **Repository Pattern:** Likely used in the `Infrastructure` layer for data abstraction (common with EF Core).
- **CQRS (Command Query Responsibility Segregation):** Potentially used within the `Application` layer\'s `Features` folder, separating read and write operations.

### Coding Conventions and Standards
- **C# Coding Conventions:** Follow standard [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- **Modern C# Features:** Embrace modern C# 12/13 features such as primary constructors, collection expressions, and other language enhancements where they improve clarity and conciseness.
- **Async/Await:** Use `async` and `await` for all I/O-bound operations to ensure non-blocking execution.
- **LINQ:** Utilize LINQ for data querying and manipulation where appropriate. Leverage new LINQ methods available in .NET 9 like `CountBy`, `AggregateBy`, and `Index`.
- **File Naming:**
    - Services: `ServiceName.cs`
    - Controllers: `ResourceNameController.cs`
    - Entities: `EntityName.cs`
    - DTOs: `ResourceNameDto.cs`, `CreateResourceNameRequest.cs`, etc.
    - Tests: `ClassNameTests.cs`
- **XML Documentation Comments:** Write XML documentation comments for all public types and members to aid understanding and for Swagger documentation generation.
  ```csharp
  /// <summary>
  /// Represents a software engineer skill.
  /// </summary>
  public class Skill
  {
      /// <summary>
      /// Gets or sets the unique identifier for the skill.
      /// </summary>
      public int Id { get; set; }

      /// <summary>
      /// Gets or sets the name of the skill.
      /// </summary>
      public string Name { get; set; }
  }
  ```
- **EditorConfig:** Adhere to settings in any `.editorconfig` file if present in the project or solution root.

### Common Terminology and Abbreviations
- **API:** Application Programming Interface
- **HTTP:** Hypertext Transfer Protocol
- **REST:** Representational State Transfer
- **CRUD:** Create, Read, Update, Delete
- **DI:** Dependency Injection
- **EF Core:** Entity Framework Core
- **DTO:** Data Transfer Object
- **POCO:** Plain Old CLR Object
- **SDK:** Software Development Kit
- **TFM:** Target Framework Moniker

### Project Structure and Organization
```
SourceCode/SoftwareEngineerSkills/
├── SoftwareEngineerSkills.sln          # Solution file
├── src/
│   ├── SoftwareEngineerSkills.API/             # API layer (Controllers, Middleware, Program.cs)
│   │   ├── Controllers/
│   │   ├── Extensions/ (e.g., OpenApiExtensions.cs)
│   │   ├── Middleware/ (e.g., GlobalExceptionHandler.cs)
│   │   └── Program.cs
│   ├── SoftwareEngineerSkills.Application/     # Application logic (Services, Features, CQRS handlers)
│   │   ├── Features/
│   │   └── DependencyInjection.cs
│   ├── SoftwareEngineerSkills.Domain/          # Core domain model (Entities, Value Objects, Interfaces)
│   │   ├── Entities/
│   │   ├── Aggregates/
│   │   └── Exceptions/
│   ├── SoftwareEngineerSkills.Infrastructure/  # Data persistence, external services
│   │   ├── Persistence/ (e.g., DbContext, Repositories)
│   │   └── Services/
│   └── SoftwareEngineerSkills.Common/          # Shared utilities (Result, Error)
└── tests/
    ├── SoftwareEngineerSkills.API.UnitTests/
    ├── SoftwareEngineerSkills.Application.UnitTests/
    ├── SoftwareEngineerSkills.Domain.UnitTests/
    ├── SoftwareEngineerSkills.Infrastructure.UnitTests/
    └── SoftwareEngineerSkills.IntegrationTests/
```

## 2. Workspace-Specific Instructions

### Preferred Code Patterns and Practices
- **ASP.NET Core for .NET 9:**
    - Ensure the project\'s Target Framework Moniker (TFM) is updated to `net9.0` in `.csproj` files.
    - Update NuGet package references for `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, etc., to their latest `9.0.x` versions.
    - Use either Minimal APIs or Controller-based APIs. Given the `Controllers` directory, prefer controller-based.
    - Leverage built-in DI for service registration and resolution.
    - Use middleware for cross-cutting concerns (e.g., `GlobalExceptionHandler.cs`).
    - For static file serving, use `app.MapStaticAssets();` as a more optimized alternative to `app.UseStaticFiles();` in `Program.cs`.
    - Utilize `JsonSerializerOptions.Web` for web-optimized JSON serialization defaults where applicable (e.g., `string json = JsonSerializer.Serialize(myObject, JsonSerializerOptions.Web);`).
- **Entity Framework Core for .NET 9:**
    - Define `DbContext` in the `Infrastructure` layer.
    - Use Code-First approach with migrations.
    - Implement repository pattern for data access abstraction.
    - Write asynchronous queries (`ToListAsync`, `FirstOrDefaultAsync`, etc.).
    - Utilize new LINQ operators like `Order()` and `OrderDescending()` for simplified key-based sorting.
    - Be aware that `Math.Min`/`Math.Max` can translate to `LEAST`/`GREATEST` SQL functions on supported databases (e.g., SQL Server 2022+). Use `EF.Functions.Least` and `EF.Functions.Greatest` for explicit database function mapping when needed.
    - For performance-critical scenarios, explore EF Core 9 features like [compiled models](https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#compiled-models).
    - Note improvements in query translations for complex scenarios (e.g., aggregates over aggregates/subqueries) and better handling of null comparisons.
- **Error Handling:**
    - Utilize the `Result` pattern from `SoftwareEngineerSkills.Common` for operations that can fail.
      ```csharp
      // Example of a service method returning a Result
      public async Task<Result<SkillDto>> GetSkillByIdAsync(int id)
      {
          var skill = await _context.Skills.FindAsync(id);
          if (skill == null)
          {
              return Result.Failure<SkillDto>(new Error("Skill.NotFound", "Skill not found"));
          }
          return Result.Success(new SkillDto { Id = skill.Id, Name = skill.Name });
      }
      ```
    - The `GlobalExceptionHandler.cs` should be used to translate `Result` failures or other exceptions into appropriate HTTP responses.
- **Testing (xUnit & Moq):**
    - **xUnit:**
        - Use `[Fact]` for simple test cases.
        - Use `[Theory]` with `[InlineData]` or `[MemberData]` for parameterized tests.
        - Organize tests by the class or feature they are testing.
    - **Moq:**
        - Create mocks using `new Mock<IService>()`.
        - Setup mock behavior using `mock.Setup(s => s.Method(It.IsAny<string>())).ReturnsAsync(expectedResult)`.
        - Verify interactions using `mock.Verify(s => s.Method(It.IsAny<string>()), Times.Once)`.
      ```csharp
      // Example xUnit test with Moq
      public class SkillServiceTests
      {
          private readonly Mock<ISkillRepository> _skillRepositoryMock;
          private readonly SkillService _skillService;

          public SkillServiceTests()
          {
              _skillRepositoryMock = new Mock<ISkillRepository>();
              _skillService = new SkillService(_skillRepositoryMock.Object);
          }

          [Fact]
          public async Task GetSkillByIdAsync_WhenSkillExists_ReturnsSuccessResult()
          {
              // Arrange
              var skill = new Skill { Id = 1, Name = "C#" };
              _skillRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(skill);

              // Act
              var result = await _skillService.GetSkillByIdAsync(1);

              // Assert
              Assert.True(result.IsSuccess);
              Assert.Equal(skill.Name, result.Value.Name);
          }
      }
      ```
- **DTOs:** Use Data Transfer Objects for API request/response models and for transferring data between layers. Do not expose domain entities directly through the API.
- **Validation:** Implement input validation, potentially using FluentValidation or data annotations on DTOs.

### Testing and Documentation Standards
- **Testing:**
    - **Unit Tests:**
        - `SoftwareEngineerSkills.Application.UnitTests` for application services and business logic.
        - `SoftwareEngineerSkills.Domain.UnitTests` for domain entity logic and value objects.
        - `SoftwareEngineerSkills.Infrastructure.UnitTests` for infrastructure components (e.g., repository implementations, if logic exists beyond simple EF Core calls).
        - `SoftwareEngineerSkills.API.UnitTests` for controller logic (mocking dependencies).
    - **Integration Tests:**
        - `SoftwareEngineerSkills.IntegrationTests` for testing API endpoints, including the interaction between API, Application, and Infrastructure layers (potentially using an in-memory database or test containers).
    - Aim for high test coverage for critical business logic.
- **Documentation:**
    - **OpenAPI/Swagger:** Ensure `OpenApiExtensions.cs` is correctly configured to generate comprehensive API documentation. Include XML comments for DTOs and controller actions.
    - **XML Comments:** All public classes, methods, and properties should have descriptive XML comments.
    - **READMEs:** Consider adding README files to individual project directories if they have specific setup or usage instructions.

### Security Considerations
- **HTTPS:** Ensure the API is configured to use HTTPS in production.
- **Input Validation:** Rigorously validate all incoming data to prevent injection attacks and ensure data integrity.
- **Authentication & Authorization:** If authentication/authorization is needed, implement standard ASP.NET Core Identity or JWT-based solutions. For .NET 9, explore features like Pushed Authorization Requests if using OIDC. Store secrets and sensitive configuration securely (e.g., using User Secrets, Azure Key Vault).
- **Principle of Least Privilege:** Ensure components and services only have the permissions necessary to perform their tasks.
- **Dependency Vulnerabilities:** Regularly update NuGet packages to patch known vulnerabilities.

### Performance Requirements
- **Efficient Database Queries:** Write optimized EF Core queries. Avoid N+1 problems by using `Include` and `ThenInclude` for eager loading or projections (`Select`) for specific data.
- **Asynchronous Operations:** Utilize `async/await` for all I/O-bound operations to prevent thread blocking and improve scalability. Consider new .NET 9 features like `Task.WhenEach` for managing multiple concurrent async operations if applicable.
- **Caching:** Consider caching strategies for frequently accessed, rarely changing data.
- **Response Times:** Aim for fast API response times, especially for frequently used endpoints.

## 3. Formatting Instructions

- **Language:** Use clear, concise C# for code examples. All generated code and comments must be in English.
- **Code Examples:** Provide relevant C# code examples for patterns related to ASP.NET Core for .NET 9, EF Core for .NET 9, xUnit, and Moq as shown above.
- **Links to Documentation:**
    - [.NET 9 Overview](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview)
    - [What's new in ASP.NET Core 9.0](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-9.0)
    - [What's new in EF Core 9.0](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-9.0)
    - [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
    - [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
    - [xUnit.net Documentation](https://xunit.net/)
    - [Moq Quickstart](https://github.com/moq/moq/wiki/Quickstart)
- **Task-Specific Context:** When asking Copilot to generate code for a specific task:
    - Specify the target project (e.g., `SoftwareEngineerSkills.Application`).
    - Mention the relevant class or file to be modified.
    - Provide context about existing related code or interfaces.
    - Clearly define expected inputs, outputs, and behavior.

## 4. Aditional context
If official documentation is needed use context7 to retrieve it
Utilize the documents in the `Docs` folder as they contain relevant information about architecture, design patterns, and other project-specific guidelines.

This document should be updated as the project evolves and new patterns or standards are adopted.
