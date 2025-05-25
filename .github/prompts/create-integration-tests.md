# Integration Test Generation Prompt

**Metadata:**
```yaml
category: testing
complexity: intermediate
applyTo:
  - "**/Controllers/**/*.cs"
  - "**/Handlers/**/*.cs"
  - "**/Services/**/*.cs"
instructions:
  - code-generation-standards.instructions.md
  - unitTests.instructions.md
  - api-design-standards.instructions.md
  - infrastructure-standards.instructions.md
```

## Request

Generate comprehensive integration tests for the selected component that test the entire flow from API endpoints through to the database using real dependencies where appropriate.

## Context

You are working with a .NET 9 enterprise application using Clean Architecture, DDD patterns, CQRS with MediatR, Entity Framework Core, and Testcontainers for integration testing. Focus on testing realistic scenarios that validate the complete application stack.

## Requirements

### Test Structure
- Use WebApplicationFactory for API integration tests
- Use Testcontainers for database dependencies  
- Test actual HTTP requests and responses
- Validate database state changes
- Test cross-cutting concerns (validation, authorization, logging)

### Test Categories
- **Happy Path Tests**: Valid scenarios with expected outcomes
- **Error Handling Tests**: Invalid inputs, business rule violations, database errors
- **Authentication/Authorization Tests**: Access control validation
- **Performance Tests**: Basic load and response time validation
- **Database Integration Tests**: CRUD operations with real database

### Quality Requirements
- Clear test naming that describes the scenario
- Proper test data setup and cleanup
- Assertion of both HTTP responses and side effects
- Use of realistic test data that matches domain rules
- Proper async/await patterns

## Example Output

```csharp
namespace SoftwareEngineerSkills.IntegrationTests.Controllers.V1;

public class TasksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly string _connectionString;

    public TasksControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();
            
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _connectionString = _postgresContainer.GetConnectionString();
        
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace DbContext with test database
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(_connectionString));
                    
                // Replace external services with mocks if needed
                services.RemoveAll<IEmailService>();
                services.AddScoped<IEmailService, MockEmailService>();
            });
        }).CreateClient();
        
        // Ensure database is created and seeded
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
        await SeedTestDataAsync(context);
    }

    [Fact]
    public async Task CreateTask_WithValidData_ShouldReturnCreatedTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Implement user authentication",
            Description = "Add JWT authentication to the API",
            Priority = TaskPriority.High,
            SkillRequirements = new List<TaskSkillRequirementDto>
            {
                new() { Category = SkillCategory.Backend, MinimumLevel = SkillLevel.Intermediate }
            }
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdTask = await response.Content.ReadFromJsonAsync<TaskDto>();
        createdTask.Should().NotBeNull();
        createdTask!.Title.Should().Be(command.Title);
        createdTask.Description.Should().Be(command.Description);
        createdTask.Priority.Should().Be(command.Priority);
        createdTask.Status.Should().Be(TaskStatus.Todo);
        createdTask.SkillRequirements.Should().HaveCount(1);
        
        // Verify database state
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var taskFromDb = await context.Tasks
            .Include(t => t.SkillRequirements)
            .FirstOrDefaultAsync(t => t.Id == createdTask.Id);
            
        taskFromDb.Should().NotBeNull();
        taskFromDb!.Title.Should().Be(command.Title);
        taskFromDb.SkillRequirements.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task AssignTask_WithInvalidSkills_ShouldReturnBadRequest()
    {
        // Arrange
        var task = await CreateTestTaskAsync();
        var developer = await CreateTestDeveloperAsync();
        
        var assignCommand = new AssignTaskCommand
        {
            TaskId = task.Id,
            DeveloperId = developer.Id
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/tasks/{task.Id}/assign", assignCommand);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Developer does not have required skills");
        
        // Verify task is not assigned in database
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var taskFromDb = await context.Tasks.FindAsync(task.Id);
        taskFromDb!.AssignedDeveloperId.Should().BeNull();
    }
    
    [Fact]
    public async Task GetTaskById_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var task = await CreateTestTaskAsync();
        
        // Act
        var response = await _client.GetAsync($"/api/v1/tasks/{task.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var returnedTask = await response.Content.ReadFromJsonAsync<TaskDto>();
        returnedTask.Should().NotBeNull();
        returnedTask!.Id.Should().Be(task.Id);
        returnedTask.Title.Should().Be(task.Title);
    }
    
    [Fact]
    public async Task GetTaskById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/api/v1/tasks/{invalidId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateTask_WithInvalidTitle_ShouldReturnBadRequest(string invalidTitle)
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = invalidTitle,
            Description = "Valid description",
            Priority = TaskPriority.Medium
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    private async Task<TaskDto> CreateTestTaskAsync()
    {
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = TaskPriority.Medium,
            SkillRequirements = new List<TaskSkillRequirementDto>
            {
                new() { Category = SkillCategory.Backend, MinimumLevel = SkillLevel.Advanced }
            }
        };
        
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", command);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TaskDto>() 
            ?? throw new InvalidOperationException("Failed to create test task");
    }
    
    private async Task<DeveloperDto> CreateTestDeveloperAsync()
    {
        var command = new CreateDeveloperCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Position = "Software Developer",
            Skills = new List<DeveloperSkillDto>
            {
                new() { Category = SkillCategory.Frontend, Level = SkillLevel.Intermediate }
            }
        };
        
        var response = await _client.PostAsJsonAsync("/api/v1/developers", command);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<DeveloperDto>()
            ?? throw new InvalidOperationException("Failed to create test developer");
    }
    
    private static async Task SeedTestDataAsync(ApplicationDbContext context)
    {
        // Add any required seed data for tests
        // This could include reference data, lookup values, etc.
        await context.SaveChangesAsync();
    }
    
    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _postgresContainer.DisposeAsync();
    }
}
```

## Checklist

- [ ] Tests cover happy path scenarios
- [ ] Tests cover error conditions and edge cases
- [ ] Tests validate HTTP status codes and response content
- [ ] Tests verify database state changes
- [ ] Tests use Testcontainers for real database dependencies
- [ ] Tests properly set up and clean up test data
- [ ] Tests are isolated and can run in any order
- [ ] Tests use realistic domain data that follows business rules
- [ ] Tests validate both API responses and side effects
- [ ] Tests include performance and load considerations where appropriate
