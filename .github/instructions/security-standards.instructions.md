---
applyTo: "**/*.cs"
---

# Security Standards for SoftwareEngineerSkills Project

This instruction file provides comprehensive security guidelines for implementing secure development practices in the Development Team Task Board system, covering authentication, authorization, data protection, and secure coding practices.

## Project Security Context

**Domain:** Development Team Task Board
**Sensitivity Level:** Internal corporate data with personal developer information
**Compliance:** GDPR, corporate security policies
**Authentication:** JWT-based with Azure AD integration
**Authorization:** Role-based with policy enforcement

## Authentication Implementation Guidelines

### JWT Configuration Standards
- Configure JWT authentication with proper token validation parameters
- Use secure secret keys with appropriate length and complexity
- Set reasonable token expiration times based on security requirements
- Validate issuer, audience, and lifetime claims consistently
- Enable HTTPS requirement for production environments

### Token Validation Parameters
- Enable all standard validations (issuer, audience, lifetime, signing key)
- Configure appropriate clock skew tolerance for time synchronization
- Set proper role and name claim types for authorization
- Use secure signing algorithms (RS256 or HS256 with strong keys)

### Authentication Events Handling
- Log authentication failures for security monitoring
- Track successful token validations for audit purposes
- Handle token expiration gracefully with proper error responses
- Monitor suspicious authentication patterns

### Configuration Management
- Store JWT settings in secure configuration sections
- Use environment variables for sensitive values in production
- Implement configuration validation at startup
- Support different settings per environment (dev, staging, prod)

### Token Service Implementation
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IOptions<JwtSettings> jwtSettings,
        IUserRepository userRepository,
        ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<TokenResponse> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var claims = await GenerateClaimsAsync(user, cancellationToken);
        
        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();
        
        // Store refresh token securely
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays));
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.ExpirationInMinutes * 60,
            TokenType = "Bearer"
        };
    }

    private async Task<List<Claim>> GenerateClaimsAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64)
        };

        // Add role claims
        var roles = await _userRepository.GetUserRolesAsync(user.Id, cancellationToken);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add custom claims for permissions
        var permissions = await _userRepository.GetUserPermissionsAsync(user.Id, cancellationToken);
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        return claims;
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
```

## Authorization Implementation

### Role-Based Authorization
```csharp
namespace SoftwareEngineerSkills.API.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Role-based policies
            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("RequireManagerRole", policy =>
                policy.RequireRole("Admin", "ProjectManager"));

            options.AddPolicy("RequireDeveloperRole", policy =>
                policy.RequireRole("Admin", "ProjectManager", "Developer"));

            // Permission-based policies
            options.AddPolicy("CanManageTasks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim("permission", "tasks.manage") ||
                    context.User.IsInRole("Admin") ||
                    context.User.IsInRole("ProjectManager")));

            options.AddPolicy("CanViewTasks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim("permission", "tasks.view") ||
                    context.User.IsInRole("Admin") ||
                    context.User.IsInRole("ProjectManager") ||
                    context.User.IsInRole("Developer")));

            options.AddPolicy("CanAssignTasks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim("permission", "tasks.assign") ||
                    context.User.IsInRole("Admin") ||
                    context.User.IsInRole("ProjectManager")));

            // Resource-based policies
            options.AddPolicy("CanEditOwnProfile", policy =>
                policy.Requirements.Add(new SameUserRequirement()));

            options.AddPolicy("CanViewAssignedTasks", policy =>
                policy.Requirements.Add(new TaskAssigneeRequirement()));
        });

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, SameUserAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, TaskAssigneeAuthorizationHandler>();

        return services;
    }
}
```

### Custom Authorization Requirements
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Authorization;

public class SameUserRequirement : IAuthorizationRequirement { }

public class SameUserAuthorizationHandler : AuthorizationHandler<SameUserRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameUserRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var requestedUserId = context.Resource as string;

        if (userId != null && 
            (userId == requestedUserId || context.User.IsInRole("Admin")))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class TaskAssigneeRequirement : IAuthorizationRequirement { }

public class TaskAssigneeAuthorizationHandler : AuthorizationHandler<TaskAssigneeRequirement>
{
    private readonly ITaskRepository _taskRepository;

    public TaskAssigneeAuthorizationHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TaskAssigneeRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var taskId = context.Resource as Guid?;

        if (userId != null && taskId.HasValue)
        {
            if (context.User.IsInRole("Admin") || context.User.IsInRole("ProjectManager"))
            {
                context.Succeed(requirement);
                return;
            }

            var task = await _taskRepository.GetByIdAsync(taskId.Value);
            if (task?.AssignedDeveloperId.ToString() == userId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
```

## Data Protection and Encryption

### Sensitive Data Handling
```csharp
namespace SoftwareEngineerSkills.Domain.ValueObjects;

public class EncryptedValue : ValueObject
{
    private readonly IDataProtector _dataProtector;
    
    public string EncryptedData { get; private set; }

    public EncryptedValue(string plainTextValue, IDataProtector dataProtector)
    {
        _dataProtector = dataProtector;
        EncryptedData = _dataProtector.Protect(plainTextValue);
    }

    private EncryptedValue(string encryptedData)
    {
        EncryptedData = encryptedData;
    }

    public string Decrypt(IDataProtector dataProtector)
    {
        return dataProtector.Unprotect(EncryptedData);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EncryptedData;
    }

    public static EncryptedValue FromEncryptedData(string encryptedData)
    {
        return new EncryptedValue(encryptedData);
    }
}
```

### Personal Data Protection
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public interface IPersonalDataService
{
    Task<string> ExportPersonalDataAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeletePersonalDataAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AnonymizePersonalDataAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class PersonalDataService : IPersonalDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PersonalDataService> _logger;

    public PersonalDataService(IUnitOfWork unitOfWork, ILogger<PersonalDataService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> ExportPersonalDataAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var developer = await _unitOfWork.Developers.GetByIdAsync(userId, cancellationToken);
        if (developer == null)
            throw new EntityNotFoundException($"Developer with ID {userId} not found");

        var personalData = new
        {
            developer.FirstName,
            developer.LastName,
            developer.Email,
            developer.Position,
            developer.HireDate,
            Skills = developer.Skills.Select(s => new { s.Skill.Name, s.Level, s.AcquiredDate }),
            Tasks = await GetDeveloperTasksAsync(userId, cancellationToken)
        };

        var json = JsonSerializer.Serialize(personalData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        _logger.LogInformation("Personal data exported for user {UserId}", userId);
        return json;
    }

    public async Task DeletePersonalDataAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var developer = await _unitOfWork.Developers.GetByIdAsync(userId, cancellationToken);
        if (developer == null)
            return;

        // Unassign all tasks
        var assignedTasks = await _unitOfWork.Tasks.GetByAssigneeAsync(userId, cancellationToken);
        foreach (var task in assignedTasks)
        {
            task.UnassignDeveloper();
        }

        // Delete developer record
        _unitOfWork.Developers.Remove(developer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Personal data deleted for user {UserId}", userId);
    }

    public async Task AnonymizePersonalDataAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var developer = await _unitOfWork.Developers.GetByIdAsync(userId, cancellationToken);
        if (developer == null)
            return;

        // Anonymize personal information
        developer.AnonymizePersonalData(); // Implement in Developer entity

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Personal data anonymized for user {UserId}", userId);
    }

    private async Task<object> GetDeveloperTasksAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tasks = await _unitOfWork.Tasks.GetByAssigneeAsync(userId, cancellationToken);
        return tasks.Select(t => new
        {
            t.Title,
            t.Description,
            t.Status,
            t.Priority,
            t.Created,
            t.DueDate
        });
    }
}
```

## Input Validation and Sanitization

### Input Validation Attributes
```csharp
namespace SoftwareEngineerSkills.Application.Common.Validation;

public class NoScriptInjectionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            var dangerousPatterns = new[]
            {
                "<script",
                "</script>",
                "javascript:",
                "vbscript:",
                "onload=",
                "onerror=",
                "onclick="
            };

            if (dangerousPatterns.Any(pattern => 
                stringValue.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult("Input contains potentially dangerous content.");
            }
        }

        return ValidationResult.Success;
    }
}

public class SqlInjectionProtectionAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            var dangerousPatterns = new[]
            {
                "';",
                "--",
                "/*",
                "*/",
                "@@",
                "sp_",
                "xp_",
                "UNION",
                "SELECT",
                "INSERT",
                "UPDATE",
                "DELETE",
                "DROP",
                "CREATE",
                "ALTER",
                "EXEC"
            };

            if (dangerousPatterns.Any(pattern => 
                stringValue.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult("Input contains potentially dangerous SQL content.");
            }
        }

        return ValidationResult.Success;
    }
}
```

### Input Sanitization Service
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public interface IInputSanitizationService
{
    string SanitizeHtml(string input);
    string SanitizeForStorage(string input);
    bool IsValidEmail(string email);
    bool IsValidUrl(string url);
}

public class InputSanitizationService : IInputSanitizationService
{
    private readonly HtmlSanitizer _htmlSanitizer;

    public InputSanitizationService()
    {
        _htmlSanitizer = new HtmlSanitizer();
        _htmlSanitizer.AllowedTags.Clear();
        _htmlSanitizer.AllowedAttributes.Clear();
    }

    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return _htmlSanitizer.Sanitize(input);
    }

    public string SanitizeForStorage(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove null characters, control characters
        input = Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", string.Empty);
        
        // Trim whitespace
        input = input.Trim();
        
        // Limit length
        if (input.Length > 10000)
        {
            input = input[..10000];
        }

        return input;
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
```

## Security Headers and Middleware

### Security Headers Middleware
```csharp
namespace SoftwareEngineerSkills.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; connect-src 'self'; frame-ancestors 'none';");

        // X-Frame-Options
        context.Response.Headers.Add("X-Frame-Options", "DENY");

        // X-Content-Type-Options
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

        // Referrer Policy
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

        // Permissions Policy
        context.Response.Headers.Add("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

        // Strict Transport Security (HTTPS only)
        if (context.Request.IsHttps)
        {
            context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        }

        await _next(context);
    }
}
```

### Rate Limiting Configuration
```csharp
namespace SoftwareEngineerSkills.API.Configuration;

public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Global rate limit
            options.AddFixedWindowLimiter("GlobalPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // API-specific rate limits
            options.AddFixedWindowLimiter("ApiPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 50;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
            });

            // Authentication endpoint rate limit
            options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
            });

            // Per-user rate limit
            options.AddTokenBucketLimiter("UserPolicy", limiterOptions =>
            {
                limiterOptions.TokenLimit = 20;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
                limiterOptions.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                limiterOptions.TokensPerPeriod = 10;
                limiterOptions.AutoReplenishment = true;
            });
        });

        return services;
    }
}
```

## Audit Logging

### Security Event Logging
```csharp
namespace SoftwareEngineerSkills.Infrastructure.Services;

public interface ISecurityAuditService
{
    Task LogAuthenticationAttemptAsync(string username, bool success, string ipAddress, string userAgent);
    Task LogAuthorizationFailureAsync(string username, string resource, string action, string ipAddress);
    Task LogDataAccessAsync(string username, string entityType, Guid entityId, string action);
    Task LogSecurityEventAsync(SecurityEventType eventType, string description, string? username = null, string? ipAddress = null);
}

public class SecurityAuditService : ISecurityAuditService
{
    private readonly ILogger<SecurityAuditService> _logger;
    private readonly IAuditRepository _auditRepository;

    public SecurityAuditService(
        ILogger<SecurityAuditService> logger,
        IAuditRepository auditRepository)
    {
        _logger = logger;
        _auditRepository = auditRepository;
    }

    public async Task LogAuthenticationAttemptAsync(
        string username, 
        bool success, 
        string ipAddress, 
        string userAgent)
    {
        var auditEvent = new SecurityAuditEvent
        {
            EventType = success ? SecurityEventType.LoginSuccess : SecurityEventType.LoginFailure,
            Username = username,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow,
            Description = success ? "User login successful" : "User login failed"
        };

        await _auditRepository.AddAsync(auditEvent);

        _logger.LogInformation("Authentication attempt: {Username} from {IpAddress} - {Result}", 
            username, ipAddress, success ? "Success" : "Failure");
    }

    public async Task LogAuthorizationFailureAsync(
        string username, 
        string resource, 
        string action, 
        string ipAddress)
    {
        var auditEvent = new SecurityAuditEvent
        {
            EventType = SecurityEventType.AuthorizationFailure,
            Username = username,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow,
            Description = $"Authorization failed for {action} on {resource}"
        };

        await _auditRepository.AddAsync(auditEvent);

        _logger.LogWarning("Authorization failure: {Username} attempted {Action} on {Resource} from {IpAddress}", 
            username, action, resource, ipAddress);
    }

    public async Task LogDataAccessAsync(
        string username, 
        string entityType, 
        Guid entityId, 
        string action)
    {
        var auditEvent = new DataAccessAuditEvent
        {
            Username = username,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Timestamp = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditEvent);

        _logger.LogInformation("Data access: {Username} performed {Action} on {EntityType} {EntityId}", 
            username, action, entityType, entityId);
    }

    public async Task LogSecurityEventAsync(
        SecurityEventType eventType, 
        string description, 
        string? username = null, 
        string? ipAddress = null)
    {
        var auditEvent = new SecurityAuditEvent
        {
            EventType = eventType,
            Username = username,
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow,
            Description = description
        };

        await _auditRepository.AddAsync(auditEvent);

        _logger.LogWarning("Security event: {EventType} - {Description} - User: {Username}, IP: {IpAddress}", 
            eventType, description, username, ipAddress);
    }
}

public enum SecurityEventType
{
    LoginSuccess,
    LoginFailure,
    LogoutSuccess,
    AuthorizationFailure,
    PasswordChange,
    AccountLocked,
    SuspiciousActivity,
    DataExport,
    DataDeletion,
    PrivilegeEscalation
}
```

## Secrets Management

### Configuration Security
```csharp
namespace SoftwareEngineerSkills.API.Configuration;

public static class SecretsConfiguration
{
    public static IConfigurationBuilder AddSecureConfiguration(
        this IConfigurationBuilder builder, 
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            builder.AddUserSecrets<Program>();
        }
        else
        {
            // Production: Use Azure Key Vault or similar
            builder.AddAzureKeyVault(new Uri("https://your-keyvault.vault.azure.net/"),
                new DefaultAzureCredential());
        }

        return builder;
    }
}

// Secure configuration model
public class DatabaseSettings
{
    [Required]
    public string ConnectionString { get; set; } = null!;
    
    [Range(1, 3600)]
    public int CommandTimeout { get; set; } = 30;
    
    public bool EnableSensitiveDataLogging { get; set; } = false;
}

public class SecuritySettings
{
    [Required]
    public JwtSettings Jwt { get; set; } = null!;
    
    [Required]
    public EncryptionSettings Encryption { get; set; } = null!;
    
    public RateLimitSettings RateLimit { get; set; } = new();
}
```

## Security Testing

### Security-Focused Tests
```csharp
[Collection("Security")]
public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SecurityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Api_WithoutAuthentication_ShouldReturn401()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Api_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await _client.GetAsync("/api/v1/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("'; DROP TABLE Tasks; --")]
    [InlineData("javascript:alert('xss')")]
    public async Task CreateTask_WithMaliciousInput_ShouldRejectRequest(string maliciousInput)
    {
        // Arrange
        var token = await GetValidTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateTaskCommand
        {
            Title = maliciousInput,
            Description = "Test description",
            Priority = TaskPriority.Medium
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Api_ShouldIncludeSecurityHeaders()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/health");

        // Assert
        response.Headers.Should().ContainKey("X-Frame-Options");
        response.Headers.Should().ContainKey("X-Content-Type-Options");
        response.Headers.GetValues("X-Frame-Options").Should().Contain("DENY");
        response.Headers.GetValues("X-Content-Type-Options").Should().Contain("nosniff");
    }

    private async Task<string> GetValidTokenAsync()
    {
        // Implementation to get valid JWT token for testing
        return "valid-test-token";
    }
}
```

## Security Checklist

### Development Security Checklist
- [ ] **Authentication**: JWT tokens properly validated with secure signing keys
- [ ] **Authorization**: Role and resource-based authorization implemented
- [ ] **Input Validation**: All inputs validated and sanitized
- [ ] **Output Encoding**: All outputs properly encoded to prevent XSS
- [ ] **SQL Injection**: Parameterized queries used exclusively
- [ ] **CSRF Protection**: Anti-forgery tokens implemented for state-changing operations
- [ ] **HTTPS**: All communication encrypted with TLS 1.2+
- [ ] **Security Headers**: All recommended security headers implemented
- [ ] **Rate Limiting**: API endpoints protected against abuse
- [ ] **Audit Logging**: Security events properly logged
- [ ] **Data Protection**: Sensitive data encrypted at rest and in transit
- [ ] **Secrets Management**: No secrets in source code, secure storage used
- [ ] **Error Handling**: No sensitive information exposed in error messages
- [ ] **Dependencies**: All dependencies regularly updated and scanned for vulnerabilities

Remember: Security is not a feature but a fundamental requirement. Implement security controls from the beginning and maintain them throughout the application lifecycle.
