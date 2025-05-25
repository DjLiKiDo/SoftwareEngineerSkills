---
mode: 'agent'
tools: ['codebase']
description: 'Create CQRS command and query handlers with proper validation'
applyTo: '**/Application/**/*.cs'
---

# Create CQRS Handler

Create CQRS command or query handlers following the established patterns in the SoftwareEngineerSkills task board system.

## Context

You are implementing the Application Layer using:
- **CQRS Pattern** with clear command/query separation
- **MediatR** for request handling
- **FluentValidation** for input validation
- **Result Pattern** for error handling without exceptions
- **Unit of Work** for data persistence

## Instructions

### 1. Handler Type Selection

**Commands** (Write Operations):
- Create, update, delete operations
- Business process execution
- State changes that generate domain events

**Queries** (Read Operations):
- Data retrieval operations
- Reporting and analytics
- Read-only projections

### 2. Implementation Patterns

Follow patterns from [Code Generation Standards](../instructions/code-generation-standards.instructions.md):

#### Command Pattern
```csharp
// Command
public record CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public string Title { get; init; }
    public string Description { get; init; }
    public TaskPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public List<TaskSkillRequirementDto> SkillRequirements { get; init; } = new();
}

// Command Handler
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public CreateTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var task = new Task(request.Title, request.Description, request.Priority);
            
            if (request.DueDate.HasValue)
                task.SetDueDate(request.DueDate.Value);
            
            foreach (var skillReq in request.SkillRequirements)
            {
                task.AddSkillRequirement(skillReq.Category, skillReq.MinimumLevel);
            }
            
            await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(_mapper.Map<TaskDto>(task));
        }
        catch (BusinessRuleException ex)
        {
            return Result.Failure<TaskDto>(ex.Message);
        }
    }
}

// Validator
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Task description is required")
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters");
            
        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid task priority");
            
        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future");
    }
}
```

#### Query Pattern
```csharp
// Query
public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskDto>>;

// Query Handler
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IReadRepository<Task> _repository;
    private readonly IMapper _mapper;
    
    public GetTaskByIdQueryHandler(IReadRepository<Task> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (task == null)
            return Result.Failure<TaskDto>($"Task with ID {request.Id} not found.");
            
        return Result.Success(_mapper.Map<TaskDto>(task));
    }
}

// Validator (if needed for complex queries)
public class GetTaskByIdQueryValidator : AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");
    }
}
```

### 3. Advanced Patterns

#### Complex Command with Business Logic
```csharp
public record AssignTaskCommand : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }
    public Guid DeveloperId { get; init; }
}

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public AssignTaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<Result<TaskDto>> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Load task and developer
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
            if (task == null)
                return Result.Failure<TaskDto>($"Task with ID {request.TaskId} not found");
                
            var developer = await _unitOfWork.Developers.GetByIdAsync(request.DeveloperId, cancellationToken);
            if (developer == null)
                return Result.Failure<TaskDto>($"Developer with ID {request.DeveloperId} not found");
            
            // Business logic
            task.AssignToDeveloper(request.DeveloperId, developer.Skills);
            developer.AssignTask(request.TaskId);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success(_mapper.Map<TaskDto>(task));
        }
        catch (BusinessRuleException ex)
        {
            return Result.Failure<TaskDto>(ex.Message);
        }
    }
}
```

#### Query with Complex Filtering
```csharp
public record GetTasksQuery : IRequest<Result<PagedResult<TaskDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public TaskStatus? Status { get; init; }
    public Guid? AssignedDeveloperId { get; init; }
    public TaskPriority? Priority { get; init; }
    public string? SearchTerm { get; init; }
}

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>
{
    private readonly IReadRepository<Task> _repository;
    private readonly IMapper _mapper;
    
    public GetTasksQueryHandler(IReadRepository<Task> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<PagedResult<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var spec = new TaskFilterSpecification(
            request.Status,
            request.AssignedDeveloperId,
            request.Priority,
            request.SearchTerm);
            
        var tasks = await _repository.GetPagedAsync(
            spec,
            request.Page,
            request.PageSize,
            cancellationToken);
            
        var taskDtos = _mapper.Map<List<TaskDto>>(tasks.Items);
        
        var result = new PagedResult<TaskDto>
        {
            Items = taskDtos,
            TotalCount = tasks.TotalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
        
        return Result.Success(result);
    }
}
```

## What to Ask Me

To create the perfect handler, ask me for:

### Command Handlers
1. **Operation Details**
   - What business operation are we implementing?
   - What domain entities are involved?
   - What business rules need to be enforced?

2. **Input Requirements**
   - What parameters are needed?
   - What validation rules apply?
   - Are there any conditional requirements?

3. **Output Expectations**
   - What should be returned on success?
   - What error scenarios need handling?
   - Should we return the created/updated entity?

### Query Handlers
1. **Data Requirements**
   - What data needs to be retrieved?
   - What filtering/sorting options are needed?
   - Is pagination required?

2. **Performance Considerations**
   - What related data should be included?
   - Are there caching opportunities?
   - What projections would be optimal?

3. **Access Patterns**
   - Who will consume this query?
   - What authorization checks are needed?
   - How frequently will this be called?

## Common Scenarios

### Task Management Commands
- `CreateTaskCommand` - Create new tasks with skill requirements
- `AssignTaskCommand` - Assign tasks to developers
- `UpdateTaskStatusCommand` - Change task status through workflow
- `AddTaskSkillRequirementCommand` - Add skill requirements to existing tasks

### Developer Management Commands
- `CreateDeveloperCommand` - Onboard new developers
- `AddDeveloperSkillCommand` - Add skills to developer profiles
- `UpdateSkillLevelCommand` - Upgrade skill proficiency levels
- `DeactivateDeveloperCommand` - Handle developer departures

### Project Management Commands
- `CreateProjectCommand` - Start new projects
- `AddTaskToProjectCommand` - Organize tasks under projects
- `UpdateProjectStatusCommand` - Track project lifecycle

### Reporting Queries
- `GetTasksByDeveloperQuery` - Developer workload reports
- `GetProjectProgressQuery` - Project status reporting
- `GetSkillGapAnalysisQuery` - Team capability analysis
- `GetTaskMetricsQuery` - Performance analytics

## Quality Checklist

Before completing the handler creation, ensure:

✅ **Proper Pattern**: Command for writes, Query for reads  
✅ **Validation**: FluentValidation rules for all inputs  
✅ **Error Handling**: Result pattern with meaningful error messages  
✅ **Business Logic**: Domain methods called, not direct property manipulation  
✅ **Transactions**: Unit of Work used for consistency  
✅ **Performance**: Appropriate includes and projections for queries  
✅ **Testing**: Handler is easily testable with mocked dependencies  

## Ready to Create

**What CQRS handler would you like me to create?**

Specify:
- Handler type (Command or Query)
- Business operation or data need
- Input parameters and validation requirements
- Expected output format
- Any special business rules or constraints
