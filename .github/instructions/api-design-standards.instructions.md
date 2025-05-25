---
applyTo: "**/*Controller.cs"
---

# API Design Standards for SoftwareEngineerSkills Project

This instruction file provides comprehensive guidelines for creating RESTful API controllers that follow enterprise standards and best practices for the Development Team Task Board system.

## Project Context

**Domain:** Development Team Task Board
**Primary Resources:** Tasks, Developers, Projects, Skills
**Authentication:** JWT-based with role-based authorization
**API Versioning:** URI versioning (api/v1, api/v2)

## RESTful API Design Principles

### Resource Naming Conventions
- **Collections:** Plural nouns (`/api/v1/tasks`, `/api/v1/developers`)
- **Individual Resources:** Singular identifier (`/api/v1/tasks/{id}`, `/api/v1/developers/{id}`)
- **Sub-resources:** Nested when appropriate (`/api/v1/tasks/{id}/comments`, `/api/v1/projects/{id}/tasks`)
- **Actions:** Use HTTP verbs, avoid action names in URLs
- **Filtering/Searching:** Use query parameters (`/api/v1/tasks?status=inprogress&assignee={developerId}`)

### HTTP Status Codes
- **200 OK:** Successful GET, PUT, PATCH operations
- **201 Created:** Successful POST operations with resource creation
- **204 No Content:** Successful DELETE operations
- **400 Bad Request:** Invalid request data or business rule violations
- **401 Unauthorized:** Authentication required
- **403 Forbidden:** Insufficient permissions
- **404 Not Found:** Resource not found
- **409 Conflict:** Resource state conflicts
- **422 Unprocessable Entity:** Validation errors
- **500 Internal Server Error:** Unexpected server errors

## Controller Implementation Guidelines

### Structural Standards
- Apply `[ApiController]` attribute for automatic model validation and error handling
- Use proper routing conventions with versioning support
- Implement dependency injection for `IMediator` and logging services
- Apply authentication by default with explicit `[AllowAnonymous]` for public endpoints

### Method Implementation Standards
- Use descriptive XML documentation for all public endpoints
- Include appropriate `[ProducesResponseType]` attributes for OpenAPI documentation
- Implement proper HTTP verb mapping following RESTful conventions
- Return consistent response patterns using the Result pattern
- Handle both success and failure scenarios appropriately

### Parameter Handling
- Use `[FromQuery]` for optional filtering and pagination parameters
- Use `[FromRoute]` for resource identifiers in URL path
- Use `[FromBody]` for complex request objects in POST/PUT operations
- Validate all input parameters using model validation attributes
    /// Creates a new task
    /// </summary>
    /// <param name="command">Task creation data</param>
    /// <returns>Created task</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return result.Error.Contains("validation") 
                ? UnprocessableEntity(result.Error)
                : BadRequest(result.Error);
        }
        
        return CreatedAtAction(
            nameof(GetTaskById), 
            new { id = result.Value.Id }, 
            result.Value);
    }
    
    /// <summary>
    /// Updates an existing task
    /// </summary>
    /// <param name="id">Task identifier</param>
    /// <param name="command">Task update data</param>
    /// <returns>Updated task</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch between route and request body");
            
        var result = await _mediator.Send(command);
        
        return result.IsSuccess
            ? Ok(result.Value)
            ## Request/Response Standards

### Request Body Validation
- Use data annotations and FluentValidation for input validation
- Perform validation in Application layer using command/query validators
- Return 422 for validation errors with detailed error information
- Return 400 for malformed requests or syntax errors

### Response Headers
- Include `Location` header for 201 Created responses pointing to new resource
- Set `Content-Type: application/json` for all API responses
- Apply appropriate `Cache-Control` headers based on data sensitivity
- Use `ETag` headers for optimistic concurrency control when applicable

### Error Response Format
- Implement consistent error response structure across all endpoints
- Include error codes, messages, and validation details in error responses
- Use problem details format (RFC 7807) for structured error information
- Log errors appropriately while avoiding sensitive data exposure

### Response Headers
- **Location:** Include for 201 Created responses
- **Content-Type:** Always application/json for API responses
- **Cache-Control:** Set appropriate caching headers
- **ETag:** For optimistic concurrency control

### Error Response Format
```csharp
public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Detail { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

## Security Standards

### Authentication & Authorization
```csharp
[Authorize] // Require authentication by default
[Authorize(Roles = "Admin,ProjectManager")] // Role-based authorization
[Authorize(Policy = "CanManageTasks")] // Policy-based authorization
[AllowAnonymous] // Explicitly allow anonymous access
```

### Input Validation
### Input Validation
- Validate all external inputs using model binding and validation attributes
- Sanitize user inputs to prevent XSS attacks and injection vulnerabilities
- Use parameterized queries through Entity Framework Core
- Apply rate limiting policies to prevent abuse

## Performance Considerations

### Async Operations
- Implement all controller actions as async methods
- Accept and pass through CancellationToken for long-running operations
- Configure appropriate timeout policies for external service calls

### Caching
- Implement response caching for appropriate GET endpoints
- Use cache headers to control client and proxy caching behavior
- Consider ETags for conditional requests and cache validation

### Response Optimization
- Enable response compression for large payloads
- Use appropriate serialization settings for JSON responses
- Implement pagination for large collections

## Documentation Standards

### XML Documentation
- Document all public action methods with comprehensive descriptions
- Include detailed parameter descriptions and expected values
- Document all possible HTTP status codes and their scenarios
- Provide usage examples for complex operations

### OpenAPI/Swagger
- Use `ProducesResponseType` attributes for all endpoints
- Include example values in request/response DTOs
- Group related endpoints with proper tags
- Provide clear operation summaries and descriptions

## Versioning Strategy

### URI Versioning
- Use version-specific routing with API version attributes
- Support multiple API versions simultaneously when needed
- Implement proper version discovery and negotiation

### Backwards Compatibility
- Maintain backwards compatibility within major versions
- Use optional parameters for new features
- Deprecate features gracefully with appropriate warning headers
- Provide migration guides for breaking changes

## Testing Guidelines

### Controller Testing
- Mock all dependencies including IMediator
- Test HTTP status codes for all scenarios
- Verify correct commands/queries are dispatched to mediator
- Test model binding and validation behavior
- Test authorization and authentication scenarios
- Verify response content and headers are correct

## Common Implementation Patterns

### Pagination Implementation
- Implement consistent pagination across all collection endpoints
- Limit maximum page size to prevent resource exhaustion
- Include pagination metadata in response headers or response body
- Support cursor-based pagination for large datasets when appropriate
    {
        Response.Headers.Add("X-Pagination-TotalCount", result.Value.TotalCount.ToString());
        Response.Headers.Add("X-Pagination-PageSize", result.Value.PageSize.ToString());
        Response.Headers.Add("X-Pagination-CurrentPage", result.Value.CurrentPage.ToString());
    }
    
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

### Filtering Pattern
```csharp
public async Task<IActionResult> GetTasks(
    [FromQuery] TaskStatus? status = null,
    [FromQuery] Guid? assigneeId = null,
    [FromQuery] DateTime? createdAfter = null,
    [FromQuery] string? searchTerm = null)
{
    var query = new GetTasksQuery
    {
        Status = status,
        AssigneeId = assigneeId,
        CreatedAfter = createdAfter,
        SearchTerm = searchTerm
    };
    
    var result = await _mediator.Send(query);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

## Error Handling

### Error Handling Implementation
- Use middleware for unhandled exceptions to ensure consistent error responses
- Log all errors with correlation IDs for traceability and debugging
- Return structured error responses that don't expose internal system details
- Map domain exceptions to appropriate HTTP status codes

### Business Rule Violations
- Handle business rule violations by examining Result object error information
- Map specific error types to appropriate HTTP status codes (400, 404, 409, etc.)
- Provide meaningful error messages that help API consumers understand the issue
- Maintain consistency in error response format across all endpoints

## Best Practices Summary

- Keep controllers thin and focused only on HTTP concerns
- Delegate all business logic to Application layer through CQRS pattern
- Use dependency injection for all external dependencies
- Implement proper logging and monitoring for all operations
- Follow RESTful conventions consistently across all endpoints
- Prioritize security, performance, and maintainability in all implementations
