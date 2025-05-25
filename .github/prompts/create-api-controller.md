---
mode: 'agent'
tools: ['codebase']
description: 'Create API controllers with proper HTTP semantics and documentation'
applyTo: '**/API/**/*.cs'
---

# Create API Controller

Create RESTful API controllers following enterprise patterns for the SoftwareEngineerSkills task board system.

## Context

You are implementing the API Layer with:
- **ASP.NET Core** controllers with proper HTTP semantics
- **API Versioning** for backward compatibility
- **OpenAPI/Swagger** documentation
- **MediatR** for CQRS command/query dispatch
- **Result Pattern** for consistent error handling

## Instructions

### 1. Controller Structure

Follow patterns from [Code Generation Standards](../instructions/code-generation-standards.instructions.md):

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Gets a task by its unique identifier
    /// </summary>
    /// <param name="id">The task unique identifier</param>
    /// <returns>The task details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id));
        
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new ProblemDetails
            {
                Title = "Task Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            });
    }
}
```

### 2. HTTP Method Patterns

#### GET Endpoints
```csharp
/// <summary>
/// Gets all tasks with optional filtering and pagination
/// </summary>
/// <param name="query">Filter and pagination parameters</param>
/// <returns>Paginated list of tasks</returns>
[HttpGet]
[ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery query)
{
    var result = await _mediator.Send(query);
    
    return result.IsSuccess
        ? Ok(result.Value)
        : BadRequest(new ProblemDetails
        {
            Title = "Invalid Request",
            Detail = result.Error,
            Status = StatusCodes.Status400BadRequest
        });
}

/// <summary>
/// Gets tasks assigned to a specific developer
/// </summary>
/// <param name="developerId">The developer's unique identifier</param>
/// <param name="status">Optional status filter</param>
/// <returns>List of assigned tasks</returns>
[HttpGet("developer/{developerId}")]
[ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetTasksByDeveloper(
    Guid developerId, 
    [FromQuery] TaskStatus? status = null)
{
    var query = new GetTasksByDeveloperQuery(developerId, status);
    var result = await _mediator.Send(query);
    
    return result.IsSuccess
        ? Ok(result.Value)
        : NotFound(new ProblemDetails
        {
            Title = "Developer Not Found",
            Detail = result.Error,
            Status = StatusCodes.Status404NotFound
        });
}
```

#### POST Endpoints
```csharp
/// <summary>
/// Creates a new task
/// </summary>
/// <param name="command">Task creation details</param>
/// <returns>The created task</returns>
[HttpPost]
[ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreateTask(CreateTaskCommand command)
{
    var result = await _mediator.Send(command);
    
    return result.IsSuccess
        ? CreatedAtAction(
            nameof(GetTaskById), 
            new { id = result.Value.Id }, 
            result.Value)
        : BadRequest(CreateValidationProblemDetails("Task Creation Failed", result.Error));
}
```

#### PUT Endpoints
```csharp
/// <summary>
/// Updates an existing task
/// </summary>
/// <param name="id">Task unique identifier</param>
/// <param name="command">Updated task details</param>
/// <returns>The updated task</returns>
[HttpPut("{id}")]
[ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskCommand command)
{
    if (id != command.Id)
        return BadRequest(new ProblemDetails
        {
            Title = "ID Mismatch",
            Detail = "The ID in the URL does not match the ID in the request body",
            Status = StatusCodes.Status400BadRequest
        });
    
    var result = await _mediator.Send(command);
    
    return result.IsSuccess
        ? Ok(result.Value)
        : result.Error.Contains("not found")
            ? NotFound(new ProblemDetails
            {
                Title = "Task Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            })
            : BadRequest(CreateValidationProblemDetails("Update Failed", result.Error));
}
```

#### PATCH Endpoints (for specific operations)
```csharp
/// <summary>
/// Assigns a task to a developer
/// </summary>
/// <param name="id">Task unique identifier</param>
/// <param name="command">Assignment details</param>
/// <returns>The updated task</returns>
[HttpPatch("{id}/assign")]
[ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> AssignTask(Guid id, AssignTaskCommand command)
{
    if (id != command.TaskId)
        return BadRequest(new ProblemDetails
        {
            Title = "ID Mismatch",
            Detail = "Task ID mismatch between URL and request body",
            Status = StatusCodes.Status400BadRequest
        });
    
    var result = await _mediator.Send(command);
    
    return result.IsSuccess
        ? Ok(result.Value)
        : HandleCommandError(result.Error);
}

/// <summary>
/// Updates task status
/// </summary>
/// <param name="id">Task unique identifier</param>
/// <param name="command">Status update details</param>
/// <returns>The updated task</returns>
[HttpPatch("{id}/status")]
[ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> UpdateTaskStatus(Guid id, UpdateTaskStatusCommand command)
{
    if (id != command.TaskId)
        return BadRequest(new ProblemDetails
        {
            Title = "ID Mismatch",
            Detail = "Task ID mismatch between URL and request body",
            Status = StatusCodes.Status400BadRequest
        });
    
    var result = await _mediator.Send(command);
    
    return result.IsSuccess
        ? Ok(result.Value)
        : HandleCommandError(result.Error);
}
```

#### DELETE Endpoints
```csharp
/// <summary>
/// Deletes a task (soft delete)
/// </summary>
/// <param name="id">Task unique identifier</param>
/// <returns>No content on success</returns>
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> DeleteTask(Guid id)
{
    var result = await _mediator.Send(new DeleteTaskCommand(id));
    
    return result.IsSuccess
        ? NoContent()
        : result.Error.Contains("not found")
            ? NotFound(new ProblemDetails
            {
                Title = "Task Not Found",
                Detail = result.Error,
                Status = StatusCodes.Status404NotFound
            })
            : BadRequest(new ProblemDetails
            {
                Title = "Delete Failed",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
}
```

### 3. Helper Methods

```csharp
private ValidationProblemDetails CreateValidationProblemDetails(string title, string error)
{
    return new ValidationProblemDetails
    {
        Title = title,
        Detail = error,
        Status = StatusCodes.Status400BadRequest
    };
}

private IActionResult HandleCommandError(string error)
{
    return error.Contains("not found")
        ? NotFound(new ProblemDetails
        {
            Title = "Resource Not Found",
            Detail = error,
            Status = StatusCodes.Status404NotFound
        })
        : BadRequest(new ProblemDetails
        {
            Title = "Operation Failed",
            Detail = error,
            Status = StatusCodes.Status400BadRequest
        });
}
```

### 4. Advanced Patterns

#### Bulk Operations
```csharp
/// <summary>
/// Bulk assigns multiple tasks to developers
/// </summary>
/// <param name="command">Bulk assignment details</param>
/// <returns>Assignment results</returns>
[HttpPost("bulk-assign")]
[ProducesResponseType(typeof(BulkAssignmentResult), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> BulkAssignTasks(BulkAssignTasksCommand command)
{
    var result = await _mediator.Send(command);
    
    return result.IsSuccess
        ? Ok(result.Value)
        : BadRequest(CreateValidationProblemDetails("Bulk Assignment Failed", result.Error));
}
```

#### File Operations
```csharp
/// <summary>
/// Exports tasks to CSV format
/// </summary>
/// <param name="query">Export parameters</param>
/// <returns>CSV file containing tasks</returns>
[HttpGet("export")]
[ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public async Task<IActionResult> ExportTasks([FromQuery] ExportTasksQuery query)
{
    var result = await _mediator.Send(query);
    
    if (!result.IsSuccess)
        return BadRequest(new ProblemDetails
        {
            Title = "Export Failed",
            Detail = result.Error,
            Status = StatusCodes.Status400BadRequest
        });
    
    return File(
        result.Value.Content,
        "text/csv",
        $"tasks-export-{DateTime.UtcNow:yyyy-MM-dd}.csv");
}
```

## Common Controller Types

### Task Management Controllers
- `TasksController` - CRUD operations for tasks
- `TaskAssignmentController` - Task assignment operations
- `TaskWorkflowController` - Status transitions and workflow

### Developer Management Controllers  
- `DevelopersController` - Developer profile management
- `DeveloperSkillsController` - Skill portfolio management
- `DeveloperWorkloadController` - Assignment and capacity tracking

### Project Management Controllers
- `ProjectsController` - Project lifecycle management
- `ProjectTasksController` - Task organization within projects
- `ProjectReportsController` - Project analytics and reporting

### Reporting Controllers
- `ReportsController` - General reporting endpoints
- `AnalyticsController` - Business intelligence queries
- `MetricsController` - Performance and KPI tracking

## What to Ask Me

To create the perfect controller, ask me for:

### Basic Information
1. **Controller Purpose**
   - What resource/domain area does this manage?
   - What are the main operations needed?
   - Who are the primary consumers?

2. **Endpoint Requirements**
   - What CRUD operations are needed?
   - Are there special business operations?
   - What query/filtering capabilities are required?

3. **API Design**
   - What HTTP methods should be supported?
   - What URL structure makes sense?
   - Are there any bulk operations needed?

### Advanced Features
1. **Security Requirements**
   - What authorization is needed?
   - Are there role-based restrictions?
   - Any rate limiting requirements?

2. **Performance Considerations**
   - Are there caching opportunities?
   - What pagination is needed?
   - Any async processing requirements?

3. **Integration Needs**
   - File upload/download capabilities?
   - External service integrations?
   - Real-time notifications?

## Quality Checklist

Before completing the controller creation, ensure:

✅ **RESTful Design**: Proper HTTP methods and status codes  
✅ **API Versioning**: Version attribute and routing  
✅ **Documentation**: XML comments and OpenAPI attributes  
✅ **Error Handling**: Consistent ProblemDetails responses  
✅ **Validation**: Input validation through commands/queries  
✅ **Security**: Appropriate authorization attributes  
✅ **Performance**: Async patterns and efficient queries  

## Ready to Create

**What API controller would you like me to create?**

Specify:
- Controller name and purpose
- Required CRUD operations
- Special business operations
- Query and filtering needs
- Any specific constraints or requirements
