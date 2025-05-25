---
mode: 'agent'
tools: ['codebase']
description: 'Generate comprehensive unit tests following testing best practices'
applyTo: '**/tests/**/*.cs'
---

# Create Unit Tests

Generate comprehensive unit tests following enterprise testing patterns for the SoftwareEngineerSkills project.

## Context

You are creating unit tests using:
- **xUnit** as the test framework
- **Moq** for mocking dependencies
- **FluentAssertions** for readable assertions
- **AutoFixture** for test data generation
- **AAA Pattern** (Arrange, Act, Assert)

## Instructions

### 1. Test Class Structure

Follow patterns from [Code Generation Standards](../instructions/code-generation.instructions.md):

```csharp
namespace SoftwareEngineerSkills.Domain.UnitTests.Aggregates.TaskAggregate;

public class TaskTests
{
    private readonly Fixture _fixture;
    
    public TaskTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateTask()
    {
        // Arrange
        var title = _fixture.Create<string>();
        var description = _fixture.Create<string>();
        var priority = TaskPriority.High;
        
        // Act
        var task = new Task(title, description, priority);
        
        // Assert
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.Priority.Should().Be(priority);
        task.Status.Should().Be(TaskStatus.Todo);
        task.Id.Should().NotBeEmpty();
        task.DomainEvents.Should().HaveCount(1);
        task.DomainEvents.First().Should().BeOfType<TaskCreatedEvent>();
    }
}
```

### 2. Domain Entity Testing

#### Constructor Tests
```csharp
[Theory]
[InlineData("", "Valid description")]
[InlineData(null, "Valid description")]
[InlineData("   ", "Valid description")]
public void Constructor_WithInvalidTitle_ShouldThrowDomainValidationException(
    string invalidTitle, 
    string description)
{
    // Arrange & Act
    var action = () => new Task(invalidTitle, description, TaskPriority.Medium);
    
    // Assert
    action.Should().Throw<DomainValidationException>()
        .Which.Errors.Should().Contain("Task title cannot be empty");
}

[Fact]
public void Constructor_WithTitleTooLong_ShouldThrowDomainValidationException()
{
    // Arrange
    var longTitle = new string('x', 201);
    var description = _fixture.Create<string>();
    
    // Act
    var action = () => new Task(longTitle, description, TaskPriority.Low);
    
    // Assert
    action.Should().Throw<DomainValidationException>()
        .Which.Errors.Should().Contain("Task title cannot exceed 200 characters");
}
```

#### Business Method Tests
```csharp
[Fact]
public void AssignToDeveloper_WithValidSkills_ShouldAssignSuccessfully()
{
    // Arrange
    var task = CreateValidTask();
    var developerId = Guid.NewGuid();
    var skill = new Skill(SkillCategory.Backend, "C#", "Programming language");
    var developerSkills = new List<DeveloperSkill>
    {
        new(skill, SkillLevel.Advanced, DateTime.UtcNow.AddDays(-30))
    };
    
    task.AddSkillRequirement(SkillCategory.Backend, SkillLevel.Intermediate);
    task.ClearDomainEvents(); // Clear creation event for clean test
    
    // Act
    task.AssignToDeveloper(developerId, developerSkills);
    
    // Assert
    task.AssignedDeveloperId.Should().Be(developerId);
    task.Status.Should().Be(TaskStatus.InProgress);
    task.DomainEvents.Should().HaveCount(1);
    task.DomainEvents.First().Should().BeOfType<TaskAssignedEvent>();
    
    var assignedEvent = (TaskAssignedEvent)task.DomainEvents.First();
    assignedEvent.TaskId.Should().Be(task.Id);
    assignedEvent.DeveloperId.Should().Be(developerId);
}

[Fact]
public void AssignToDeveloper_WithInsufficientSkills_ShouldThrowBusinessRuleException()
{
    // Arrange
    var task = CreateValidTask();
    var developerId = Guid.NewGuid();
    var skill = new Skill(SkillCategory.Frontend, "JavaScript", "Programming language");
    var developerSkills = new List<DeveloperSkill>
    {
        new(skill, SkillLevel.Beginner, DateTime.UtcNow.AddDays(-30))
    };
    
    task.AddSkillRequirement(SkillCategory.Backend, SkillLevel.Intermediate);
    
    // Act
    var action = () => task.AssignToDeveloper(developerId, developerSkills);
    
    // Assert
    action.Should().Throw<BusinessRuleException>()
        .WithMessage("Developer does not have required skills for this task");
    
    task.AssignedDeveloperId.Should().BeNull();
    task.Status.Should().Be(TaskStatus.Todo);
}

[Fact]
public void AssignToDeveloper_WhenAlreadyAssignedToSameDeveloper_ShouldNotChangeState()
{
    // Arrange
    var task = CreateValidTask();
    var developerId = Guid.NewGuid();
    var developerSkills = CreateValidDeveloperSkills();
    
    task.AddSkillRequirement(SkillCategory.Backend, SkillLevel.Intermediate);
    task.AssignToDeveloper(developerId, developerSkills);
    task.ClearDomainEvents();
    
    // Act
    task.AssignToDeveloper(developerId, developerSkills);
    
    // Assert
    task.AssignedDeveloperId.Should().Be(developerId);
    task.DomainEvents.Should().BeEmpty(); // No new events
}
```

#### Domain Event Tests
```csharp
[Fact]
public void AddSkillRequirement_ShouldRaiseTaskSkillRequirementAddedEvent()
{
    // Arrange
    var task = CreateValidTask();
    task.ClearDomainEvents();
    var category = SkillCategory.Backend;
    var level = SkillLevel.Advanced;
    
    // Act
    task.AddSkillRequirement(category, level);
    
    // Assert
    task.SkillRequirements.Should().HaveCount(1);
    task.DomainEvents.Should().HaveCount(1);
    task.DomainEvents.First().Should().BeOfType<TaskSkillRequirementAddedEvent>();
    
    var skillEvent = (TaskSkillRequirementAddedEvent)task.DomainEvents.First();
    skillEvent.TaskId.Should().Be(task.Id);
    skillEvent.Category.Should().Be(category);
    skillEvent.MinimumLevel.Should().Be(level);
}
```

### 3. Value Object Testing

```csharp
public class DeveloperSkillTests
{
    private readonly Fixture _fixture;
    
    public DeveloperSkillTests()
    {
        _fixture = new Fixture();
    }
    
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateDeveloperSkill()
    {
        // Arrange
        var skill = new Skill(SkillCategory.Backend, "C#", "Programming language");
        var level = SkillLevel.Advanced;
        var acquiredDate = DateTime.UtcNow.AddDays(-30);
        
        // Act
        var developerSkill = new DeveloperSkill(skill, level, acquiredDate);
        
        // Assert
        developerSkill.Skill.Should().Be(skill);
        developerSkill.Level.Should().Be(level);
        developerSkill.AcquiredDate.Should().Be(acquiredDate);
    }
    
    [Fact]
    public void MeetsRequirement_WithMatchingCategoryAndSufficientLevel_ShouldReturnTrue()
    {
        // Arrange
        var skill = new Skill(SkillCategory.Backend, "C#", "Programming language");
        var developerSkill = new DeveloperSkill(skill, SkillLevel.Advanced, DateTime.UtcNow.AddDays(-30));
        var requirement = new TaskSkillRequirement(SkillCategory.Backend, SkillLevel.Intermediate, true);
        
        // Act
        var result = developerSkill.MeetsRequirement(requirement);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var skill = new Skill(SkillCategory.Backend, "C#", "Programming language");
        var acquiredDate = DateTime.UtcNow.AddDays(-30);
        var developerSkill1 = new DeveloperSkill(skill, SkillLevel.Advanced, acquiredDate);
        var developerSkill2 = new DeveloperSkill(skill, SkillLevel.Advanced, acquiredDate);
        
        // Act & Assert
        developerSkill1.Should().Be(developerSkill2);
        developerSkill1.GetHashCode().Should().Be(developerSkill2.GetHashCode());
    }
}
```

### 4. Command Handler Testing

```csharp
public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTaskCommandHandler _handler;
    private readonly Fixture _fixture;
    
    public CreateTaskCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _fixture = new Fixture();
        
        _unitOfWorkMock.Setup(uow => uow.Tasks).Returns(_taskRepositoryMock.Object);
        
        _handler = new CreateTaskCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTaskSuccessfully()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Implement authentication",
            Description = "Add JWT authentication to the API",
            Priority = TaskPriority.High,
            SkillRequirements = new List<TaskSkillRequirementDto>
            {
                new() { Category = SkillCategory.Backend, MinimumLevel = SkillLevel.Intermediate }
            }
        };
        
        var expectedTaskDto = new TaskDto 
        { 
            Id = Guid.NewGuid(), 
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            Status = TaskStatus.Todo
        };
        
        _mapperMock
            .Setup(m => m.Map<TaskDto>(It.IsAny<Task>()))
            .Returns(expectedTaskDto);
            
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedTaskDto);
        
        _taskRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Task>(t => 
                t.Title == command.Title && 
                t.Description == command.Description &&
                t.Priority == command.Priority), 
                It.IsAny<CancellationToken>()),
            Times.Once);
            
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_WhenBusinessRuleExceptionOccurs_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "",  // Invalid title
            Description = "Valid description",
            Priority = TaskPriority.Medium
        };
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        
        _taskRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Task>(), It.IsAny<CancellationToken>()),
            Times.Never);
            
        _unitOfWorkMock.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
```

### 5. Query Handler Testing

```csharp
public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<IReadRepository<Task>> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTaskByIdQueryHandler _handler;
    private readonly Fixture _fixture;
    
    public GetTaskByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IReadRepository<Task>>();
        _mapperMock = new Mock<IMapper>();
        _fixture = new Fixture();
        
        _handler = new GetTaskByIdQueryHandler(
            _repositoryMock.Object,
            _mapperMock.Object);
    }
    
    [Fact]
    public async Task Handle_WithExistingTask_ShouldReturnTaskDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(taskId);
        
        var task = new Task("Test Task", "Test Description", TaskPriority.Medium);
        var taskDto = new TaskDto 
        { 
            Id = task.Id, 
            Title = task.Title,
            Description = task.Description
        };
        
        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
            
        _mapperMock
            .Setup(m => m.Map<TaskDto>(task))
            .Returns(taskDto);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(taskDto);
        
        _repositoryMock.Verify(
            r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_WithNonExistentTask_ShouldReturnFailure()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(taskId);
        
        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Task?)null);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Task with ID {taskId} not found");
        
        _mapperMock.Verify(
            m => m.Map<TaskDto>(It.IsAny<Task>()),
            Times.Never);
    }
}
```

### 6. Test Helper Methods

```csharp
public static class TestHelpers
{
    public static Task CreateValidTask()
    {
        return new Task(
            "Test Task",
            "Test Description", 
            TaskPriority.Medium);
    }
    
    public static List<DeveloperSkill> CreateValidDeveloperSkills()
    {
        var backendSkill = new Skill(SkillCategory.Backend, "C#", "Programming language");
        var frontendSkill = new Skill(SkillCategory.Frontend, "React", "UI Framework");
        
        return new List<DeveloperSkill>
        {
            new(backendSkill, SkillLevel.Advanced, DateTime.UtcNow.AddDays(-365)),
            new(frontendSkill, SkillLevel.Intermediate, DateTime.UtcNow.AddDays(-180))
        };
    }
    
    public static Developer CreateValidDeveloper()
    {
        return new Developer(
            "John",
            "Doe",
            "john.doe@company.com",
            "Senior Developer",
            DateTime.UtcNow.AddYears(-3));
    }
}
```

## Test Categories

### Domain Layer Tests
- **Entity Tests**: Constructor validation, business methods, invariants
- **Value Object Tests**: Equality, immutability, business logic
- **Domain Service Tests**: Cross-aggregate operations
- **Domain Event Tests**: Event generation and data

### Application Layer Tests
- **Command Handler Tests**: Business logic execution, error handling
- **Query Handler Tests**: Data retrieval, filtering, mapping
- **Validator Tests**: Input validation rules
- **Domain Event Handler Tests**: Event processing logic

### Infrastructure Layer Tests
- **Repository Tests**: Data access patterns
- **Configuration Tests**: EF Core mappings
- **External Service Tests**: Integration patterns

## What to Ask Me

To create comprehensive tests, ask me for:

### Test Scope
1. **What to Test**
   - Which class/method needs testing?
   - What are the critical business scenarios?
   - What edge cases should be covered?

2. **Test Strategy**
   - Unit tests for individual components?
   - Integration tests for workflows?
   - What level of code coverage is expected?

### Test Scenarios
1. **Happy Path Tests**
   - What are the normal usage scenarios?
   - What valid inputs should be tested?

2. **Error Cases**
   - What validation errors can occur?
   - What business rule violations are possible?
   - How should exceptions be handled?

3. **Edge Cases**
   - Boundary conditions?
   - Null/empty input handling?
   - Concurrent access scenarios?

## Quality Checklist

Before completing the test creation, ensure:

✅ **AAA Pattern**: Clear Arrange, Act, Assert sections  
✅ **Single Responsibility**: Each test validates one thing  
✅ **Descriptive Names**: Test names explain the scenario  
✅ **Independent Tests**: No dependencies between tests  
✅ **Mock Dependencies**: External dependencies are mocked  
✅ **Error Testing**: Both success and failure scenarios  
✅ **Edge Cases**: Boundary conditions covered  

## Ready to Create Tests

**What unit tests would you like me to create?**

Specify:
- Class/method to test
- Test scenarios needed (happy path, errors, edge cases)
- Coverage requirements
- Any specific constraints or considerations
