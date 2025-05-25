---
applyTo: "**/*.cs"
---

# Performance Optimization Standards for SoftwareEngineerSkills Project

This instruction file provides comprehensive guidelines for implementing high-performance code in the Development Team Task Board system, covering database optimization, caching strategies, async patterns, and resource management.

## Project Performance Context

**Domain:** Development Team Task Board
**Scale:** Enterprise-level with thousands of tasks and hundreds of developers
**Performance Targets:** 
- API response time: < 200ms (95th percentile)
- Database queries: < 50ms (average)
- Memory usage: < 2GB per instance
- Throughput: > 1000 requests/minute per instance

## Database Performance Optimization

### Query Optimization Patterns
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Repositories;

public class OptimizedTaskRepository : Repository<Task>, ITaskRepository
{
    public OptimizedTaskRepository(ApplicationDbContext context, ILogger<OptimizedTaskRepository> logger) 
        : base(context, logger)
    {
    }

    // Optimized query with projection to avoid loading full entities
    public async Task<PagedResult<TaskSummaryDto>> GetTaskSummariesAsync(
        TaskFilterCriteria criteria,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking() // Disable change tracking for read-only queries
            .Where(t => !t.IsDeleted);

        // Apply filters efficiently
        if (criteria.Status.HasValue)
            query = query.Where(t => t.Status == criteria.Status.Value);

        if (criteria.AssigneeId.HasValue)
            query = query.Where(t => t.AssignedDeveloperId == criteria.AssigneeId.Value);

        if (criteria.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == criteria.ProjectId.Value);

        if (criteria.DueDateFrom.HasValue)
            query = query.Where(t => t.DueDate >= criteria.DueDateFrom.Value);

        if (criteria.DueDateTo.HasValue)
            query = query.Where(t => t.DueDate <= criteria.DueDateTo.Value);

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(t => 
                EF.Functions.Contains(t.Title, criteria.SearchTerm) ||
                EF.Functions.Contains(t.Description, criteria.SearchTerm));
        }

        // Get count efficiently
        var totalCount = await query.CountAsync(cancellationToken);

        // Project to DTO to minimize data transfer
        var items = await query
            .Select(t => new TaskSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                AssignedDeveloperName = t.AssignedDeveloperId != null 
                    ? _context.Developers
                        .Where(d => d.Id == t.AssignedDeveloperId)
                        .Select(d => d.FirstName + " " + d.LastName)
                        .FirstOrDefault()
                    : null,
                DueDate = t.DueDate,
                Created = t.Created
            })
            .OrderByDescending(t => t.Created)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskSummaryDto>(items, totalCount, pageNumber, pageSize);
    }

    // Batch loading with includes to minimize database round trips
    public async Task<IEnumerable<Task>> GetTasksWithDetailsAsync(
        IEnumerable<Guid> taskIds,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.SkillRequirements)
            .Include(t => t.Project)
            .Where(t => taskIds.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    // Compiled query for frequently executed operations
    private static readonly Func<ApplicationDbContext, Guid, Task<Task?>> GetTaskByIdCompiled =
        EF.CompileQuery((ApplicationDbContext context, Guid id) =>
            context.Tasks
                .Include(t => t.SkillRequirements)
                .FirstOrDefault(t => t.Id == id));

    public async Task<Task?> GetTaskByIdOptimizedAsync(Guid id)
    {
        return await Task.FromResult(GetTaskByIdCompiled(_context, id));
    }

    // Bulk operations for better performance
    public async Task<int> BulkUpdateTaskStatusAsync(
        IEnumerable<Guid> taskIds,
        TaskStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => taskIds.Contains(t.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Status, newStatus)
                .SetProperty(t => t.LastModified, DateTime.UtcNow),
                cancellationToken);
    }
}
```

### Database Configuration Optimization
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Persistence.Configuration;

public static class DatabaseOptimizationConfiguration
{
    public static void ConfigureForPerformance(this ModelBuilder modelBuilder)
    {
        // Configure indexes for frequently queried columns
        modelBuilder.Entity<Task>(entity =>
        {
            // Composite index for common filter combinations
            entity.HasIndex(t => new { t.Status, t.AssignedDeveloperId, t.ProjectId })
                .HasDatabaseName("IX_Tasks_Status_Assignee_Project");

            // Index for text search
            entity.HasIndex(t => t.Title)
                .HasDatabaseName("IX_Tasks_Title");

            // Index for date range queries
            entity.HasIndex(t => t.DueDate)
                .HasDatabaseName("IX_Tasks_DueDate");

            // Index for audit queries
            entity.HasIndex(t => t.Created)
                .HasDatabaseName("IX_Tasks_Created");

            // Filtered index for active tasks
            entity.HasIndex(t => t.Status)
                .HasFilter("[Status] IN ('Todo', 'InProgress', 'InReview')")
                .HasDatabaseName("IX_Tasks_ActiveStatus");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasIndex(d => d.Email)
                .IsUnique()
                .HasDatabaseName("IX_Developers_Email");

            entity.HasIndex(d => new { d.IsActive, d.Position })
                .HasDatabaseName("IX_Developers_Active_Position");
        });

        // Configure value conversions for enums to use integers
        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Task>()
            .Property(t => t.Priority)
            .HasConversion<int>();
    }

    public static void ConfigureDbContextForPerformance(DbContextOptionsBuilder options, string connectionString)
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.CommandTimeout(30);
            sqlOptions.EnableRetryOnFailure(3);
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

        // Disable sensitive data logging in production
        options.EnableSensitiveDataLogging(false);
        options.EnableDetailedErrors(false);

        // Configure query behavior
        options.ConfigureWarnings(warnings =>
        {
            warnings.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
        });
    }
}
```

## Caching Strategies

### Multi-Level Caching Implementation
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class;
    Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

public class HybridCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<HybridCacheService> _logger;
    private readonly TimeSpan _memoryExpiration = TimeSpan.FromMinutes(5);

    public HybridCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<HybridCacheService> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        // Try L1 cache (memory) first
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        // Try L2 cache (distributed)
        var distributedValue = await GetFromDistributedCacheAsync<T>(key, cancellationToken);
        if (distributedValue != null)
        {
            // Promote to L1 cache
            _memoryCache.Set(key, distributedValue, _memoryExpiration);
            return distributedValue;
        }

        return null;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        // Set in both caches
        _memoryCache.Set(key, value, TimeSpan.FromMinutes(Math.Min(expiration.TotalMinutes, _memoryExpiration.TotalMinutes)));
        await SetDistributedCacheAsync(key, value, expiration, cancellationToken);
    }

    public async Task<T> GetOrSetAsync<T>(
        string key, 
        Func<CancellationToken, Task<T>> factory, 
        TimeSpan expiration, 
        CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var value = await factory(cancellationToken);
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    private async Task<T?> GetFromDistributedCacheAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var cachedBytes = await _distributedCache.GetAsync(key, cancellationToken);
            if (cachedBytes == null) return null;

            var cachedJson = Encoding.UTF8.GetString(cachedBytes);
            return JsonSerializer.Deserialize<T>(cachedJson);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get value from distributed cache for key {Key}", key);
            return null;
        }
    }

    private async Task SetDistributedCacheAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _distributedCache.SetAsync(key, bytes, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set value in distributed cache for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Implementation depends on cache provider
        // Redis supports pattern-based removal
        throw new NotImplementedException("Pattern-based cache removal requires specific cache provider support");
    }
}
```

### Response Caching Implementation
```csharp
namespace SoftwareEngineerSkills.Application.Tasks.Queries;

public class GetTaskByIdCachedQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetTaskByIdCachedQueryHandler> _logger;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(15);

    public GetTaskByIdCachedQueryHandler(
        ITaskRepository repository,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<GetTaskByIdCachedQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"task:{request.Id}";

        var cachedTask = await _cacheService.GetOrSetAsync(
            cacheKey,
            async ct =>
            {
                var task = await _repository.GetByIdAsync(request.Id, ct);
                return task != null ? _mapper.Map<TaskDto>(task) : null;
            },
            CacheExpiration,
            cancellationToken);

        return cachedTask != null
            ? Result.Success(cachedTask)
            : Result.Failure<TaskDto>($"Task with ID {request.Id} not found.");
    }
}

// Cache invalidation on updates
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public UpdateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
            return Result.Failure<TaskDto>($"Task with ID {request.Id} not found.");

        // Update task
        task.UpdateDetails(request.Title, request.Description);
        task.UpdatePriority(request.Priority);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await _cacheService.RemoveAsync($"task:{request.Id}", cancellationToken);

        return Result.Success(_mapper.Map<TaskDto>(task));
    }
}
```

## Async Programming Patterns

### Efficient Async Operations
```csharp
namespace SoftwareEngineerSkills.Application.Services;

public class TaskAssignmentService : ITaskAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<TaskAssignmentService> _logger;

    public TaskAssignmentService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<TaskAssignmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    // Parallel processing for bulk operations
    public async Task<Result<int>> AssignTasksInBulkAsync(
        IEnumerable<TaskAssignmentRequest> assignments,
        CancellationToken cancellationToken = default)
    {
        var assignmentList = assignments.ToList();
        var partitioner = Partitioner.Create(assignmentList, true);

        var assignedCount = 0;
        var options = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        await Parallel.ForEachAsync(partitioner, options, async (assignment, ct) =>
        {
            try
            {
                var task = await _unitOfWork.Tasks.GetByIdAsync(assignment.TaskId, ct);
                var developer = await _unitOfWork.Developers.GetByIdAsync(assignment.DeveloperId, ct);

                if (task != null && developer != null && task.CanBeAssignedTo(developer.Skills))
                {
                    task.AssignToDeveloper(assignment.DeveloperId, developer.Skills);
                    Interlocked.Increment(ref assignedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign task {TaskId} to developer {DeveloperId}", 
                    assignment.TaskId, assignment.DeveloperId);
            }
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(assignedCount);
    }

    // Background task processing with proper async patterns
    public async Task ProcessTaskNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var pendingNotifications = await _unitOfWork.Notifications
            .GetPendingNotificationsAsync(cancellationToken);

        var semaphore = new SemaphoreSlim(10, 10); // Limit concurrent operations
        var tasks = pendingNotifications.Select(async notification =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await ProcessSingleNotificationAsync(notification, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    private async Task ProcessSingleNotificationAsync(
        Notification notification, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendNotificationAsync(notification, cancellationToken);
            notification.MarkAsSent();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process notification {NotificationId}", notification.Id);
            notification.MarkAsFailed();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
```

### Resource Management and Disposal
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public class FileProcessingService : IFileProcessingService, IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ILogger<FileProcessingService> _logger;

    public FileProcessingService(ILogger<FileProcessingService> logger)
    {
        _semaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);
        _cancellationTokenSource = new CancellationTokenSource();
        _logger = logger;
    }

    public async Task<Result<FileProcessResult>> ProcessFileAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        using var combinedToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

        await _semaphore.WaitAsync(combinedToken.Token);
        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream, combinedToken.Token);
            
            // Process file content
            var result = await ProcessFileContentAsync(memoryStream.ToArray(), fileName, combinedToken.Token);
            return Result.Success(result);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<FileProcessResult>("File processing was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FileName}", fileName);
            return Result.Failure<FileProcessResult>($"Error processing file: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<FileProcessResult> ProcessFileContentAsync(
        byte[] content,
        string fileName,
        CancellationToken cancellationToken)
    {
        // Simulate processing with proper cancellation support
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(10, cancellationToken); // Simulate work
        }

        return new FileProcessResult { ProcessedBytes = content.Length, FileName = fileName };
    }

    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        
        try
        {
            await Task.Delay(1000, CancellationToken.None); // Grace period
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        _semaphore.Dispose();
        _cancellationTokenSource.Dispose();
    }
}
```

## Memory Management

### Object Pooling
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public class PooledStringBuilderService : IStringBuilderService
{
    private static readonly ObjectPool<StringBuilder> StringBuilderPool = 
        new DefaultObjectPoolProvider().CreateStringBuilderPool();

    public string BuildTaskSummary(Task task, IEnumerable<Developer> developers)
    {
        var sb = StringBuilderPool.Get();
        try
        {
            sb.AppendLine($"Task: {task.Title}");
            sb.AppendLine($"Status: {task.Status}");
            sb.AppendLine($"Priority: {task.Priority}");
            
            if (task.AssignedDeveloperId.HasValue)
            {
                var assignee = developers.FirstOrDefault(d => d.Id == task.AssignedDeveloperId);
                sb.AppendLine($"Assigned to: {assignee?.FirstName} {assignee?.LastName}");
            }

            sb.AppendLine($"Due Date: {task.DueDate:yyyy-MM-dd}");
            
            return sb.ToString();
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }
}

// Custom object pool for complex objects
public class TaskDtoPool : IObjectPool<TaskDto>
{
    private readonly ConcurrentBag<TaskDto> _pool = new();

    public TaskDto Get()
    {
        return _pool.TryTake(out var item) ? item : new TaskDto();
    }

    public void Return(TaskDto obj)
    {
        // Reset object state
        obj.Reset();
        _pool.Add(obj);
    }
}
```

### Memory-Efficient Data Processing
```csharp
namespace SoftwareEngineerSkills.Application.Services;

public class ReportGenerationService : IReportGenerationService
{
    private readonly ITaskRepository _taskRepository;

    public ReportGenerationService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    // Stream processing for large datasets
    public async IAsyncEnumerable<TaskReportItem> GenerateTaskReportAsync(
        DateTime fromDate,
        DateTime toDate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int batchSize = 1000;
        var offset = 0;

        while (true)
        {
            var batch = await _taskRepository.GetTasksForReportAsync(
                fromDate, toDate, offset, batchSize, cancellationToken);

            if (!batch.Any())
                yield break;

            foreach (var task in batch)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                yield return new TaskReportItem
                {
                    Id = task.Id,
                    Title = task.Title,
                    Status = task.Status,
                    CompletedDate = task.CompletedDate,
                    DurationInHours = task.CalculateDurationInHours()
                };
            }

            offset += batchSize;

            // Allow garbage collection
            if (offset % 10000 == 0)
            {
                GC.Collect(0, GCCollectionMode.Optimized);
                await Task.Yield();
            }
        }
    }

    // Memory-efficient CSV export
    public async Task ExportTasksToCsvAsync(
        Stream outputStream,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        using var writer = new StreamWriter(outputStream, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Write header
        csv.WriteHeader<TaskReportItem>();
        await csv.NextRecordAsync();

        // Stream data
        await foreach (var item in GenerateTaskReportAsync(fromDate, toDate, cancellationToken))
        {
            csv.WriteRecord(item);
            await csv.NextRecordAsync();

            // Flush periodically to manage memory
            if (csv.Row % 1000 == 0)
            {
                await csv.FlushAsync();
            }
        }
    }
}
```

## HTTP Client Optimization

### Efficient HTTP Client Usage
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiService> _logger;
    private readonly SemaphoreSlim _semaphore;

    public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _semaphore = new SemaphoreSlim(10, 10); // Limit concurrent requests
        
        // Configure HttpClient for performance
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "SoftwareEngineerSkills/1.0");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<Result<SkillValidationResponse>> ValidateSkillsAsync(
        IEnumerable<string> skillNames,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            var request = new SkillValidationRequest { Skills = skillNames.ToList() };
            
            using var response = await _httpClient.PostAsJsonAsync(
                "api/skills/validate", 
                request, 
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SkillValidationResponse>(
                    cancellationToken: cancellationToken);
                return Result.Success(result!);
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Skill validation failed with status {StatusCode}: {Content}", 
                response.StatusCode, errorContent);
            
            return Result.Failure<SkillValidationResponse>(
                $"Skill validation failed: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during skill validation");
            return Result.Failure<SkillValidationResponse>("Network error during skill validation");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning("Skill validation request timed out");
            return Result.Failure<SkillValidationResponse>("Skill validation request timed out");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

// HTTP client configuration for performance
public static class HttpClientConfiguration
{
    public static IServiceCollection AddOptimizedHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<ExternalApiService>(client =>
        {
            client.BaseAddress = new Uri("https://api.example.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            MaxConnectionsPerServer = 20,
            UseCookies = false
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}
```

## Performance Monitoring

### Performance Metrics Collection
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Monitoring;

public class PerformanceMetricsService : IPerformanceMetricsService
{
    private readonly ILogger<PerformanceMetricsService> _logger;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly Counter<int> _databaseQueryCounter;
    private readonly Histogram<double> _databaseQueryDuration;

    public PerformanceMetricsService(IMeterFactory meterFactory, ILogger<PerformanceMetricsService> logger)
    {
        _logger = logger;
        var meter = meterFactory.Create("SoftwareEngineerSkills.Performance");
        
        _requestCounter = meter.CreateCounter<int>(
            name: "http_requests_total",
            description: "Total number of HTTP requests");
            
        _requestDuration = meter.CreateHistogram<double>(
            name: "http_request_duration_seconds",
            description: "HTTP request duration in seconds");
            
        _databaseQueryCounter = meter.CreateCounter<int>(
            name: "database_queries_total",
            description: "Total number of database queries");
            
        _databaseQueryDuration = meter.CreateHistogram<double>(
            name: "database_query_duration_seconds",
            description: "Database query duration in seconds");
    }

    public IDisposable MeasureRequestDuration(string endpoint, string method)
    {
        var tags = new TagList
        {
            ["endpoint"] = endpoint,
            ["method"] = method
        };

        _requestCounter.Add(1, tags);
        return new TimingScope(_requestDuration, tags);
    }

    public IDisposable MeasureDatabaseQuery(string operation, string entityType)
    {
        var tags = new TagList
        {
            ["operation"] = operation,
            ["entity_type"] = entityType
        };

        _databaseQueryCounter.Add(1, tags);
        return new TimingScope(_databaseQueryDuration, tags);
    }

    private class TimingScope : IDisposable
    {
        private readonly Histogram<double> _histogram;
        private readonly TagList _tags;
        private readonly long _startTime;

        public TimingScope(Histogram<double> histogram, TagList tags)
        {
            _histogram = histogram;
            _tags = tags;
            _startTime = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            var elapsed = Stopwatch.GetElapsedTime(_startTime);
            _histogram.Record(elapsed.TotalSeconds, _tags);
        }
    }
}

// Performance monitoring middleware
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IPerformanceMetricsService _metricsService;

    public PerformanceMonitoringMiddleware(
        RequestDelegate next,
        IPerformanceMetricsService metricsService)
    {
        _next = next;
        _metricsService = metricsService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.Request.Path.Value ?? "unknown";
        var method = context.Request.Method;

        using var measurement = _metricsService.MeasureRequestDuration(endpoint, method);
        await _next(context);
    }
}
```

## Performance Testing

### Load Testing Scenarios
```csharp
[Collection("Performance")]
public class PerformanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PerformanceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ShouldHandleHighLoad()
    {
        // Arrange
        var concurrentRequests = 100;
        var tasks = new Task[concurrentRequests];

        // Act
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < concurrentRequests; i++)
        {
            tasks[i] = _client.GetAsync("/api/v1/tasks");
        }

        var responses = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        responses.Should().AllSatisfy(response => 
            response.StatusCode.Should().Be(HttpStatusCode.OK));
        
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5 seconds max
        
        var averageResponseTime = stopwatch.ElapsedMilliseconds / (double)concurrentRequests;
        averageResponseTime.Should().BeLessThan(200); // 200ms average
    }

    [Fact]
    public async Task CreateTask_ShouldMaintainPerformanceUnderLoad()
    {
        // Arrange
        var concurrentRequests = 50;
        var tasks = new Task[concurrentRequests];
        
        // Act
        for (int i = 0; i < concurrentRequests; i++)
        {
            var request = new CreateTaskCommand
            {
                Title = $"Performance Test Task {i}",
                Description = "Test task for performance testing",
                Priority = TaskPriority.Medium
            };
            
            tasks[i] = _client.PostAsJsonAsync("/api/v1/tasks", request);
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().AllSatisfy(response => 
            response.StatusCode.Should().Be(HttpStatusCode.Created));
    }
}
```

## Performance Optimization Checklist

### Database Performance
- [ ] **Indexes**: Appropriate indexes on frequently queried columns
- [ ] **Query Optimization**: Use projections, avoid N+1 queries, use compiled queries
- [ ] **Connection Pooling**: Properly configured connection pools
- [ ] **Async Operations**: All database operations are async
- [ ] **Batch Operations**: Use bulk operations for multiple entities
- [ ] **Query Splitting**: Configure EF Core query splitting behavior

### Caching Strategy
- [ ] **Multi-Level Caching**: Memory and distributed cache implementation
- [ ] **Cache Invalidation**: Proper cache invalidation strategies
- [ ] **Cache Keys**: Consistent and logical cache key patterns
- [ ] **Expiration Policies**: Appropriate cache expiration times
- [ ] **Cache Warming**: Pre-populate frequently accessed data

### Memory Management
- [ ] **Object Pooling**: Use object pools for frequently created objects
- [ ] **Disposal**: Proper disposal of resources and subscriptions
- [ ] **Stream Processing**: Use streaming for large data sets
- [ ] **Memory Profiling**: Regular memory usage monitoring

### HTTP Performance
- [ ] **HTTP Client Reuse**: Use HttpClientFactory and avoid socket exhaustion
- [ ] **Connection Limits**: Configure appropriate connection limits
- [ ] **Compression**: Enable response compression
- [ ] **Keep-Alive**: Use persistent connections
- [ ] **Request Batching**: Batch multiple requests where possible

### Async Programming
- [ ] **ConfigureAwait**: Use ConfigureAwait(false) in library code
- [ ] **Parallel Processing**: Use Task.Parallel for CPU-bound operations
- [ ] **Semaphores**: Control concurrency with semaphores
- [ ] **Cancellation**: Proper cancellation token support

Remember: Performance optimization should be driven by actual measurements and profiling data, not assumptions. Always measure before and after optimizations to verify improvements.
