# IOptions Pattern Implementation Plan

## Review and Improvement of IOptions Pattern Implementation

The existing implementation of the IOptions pattern in the SoftwareEngineerSkills project follows many best practices already, but this plan outlines specific improvements and recommendations based on the official ASP.NET Core documentation for .NET 9.

### Current Implementation Strengths

1.  **Use of `IOptionsMonitor` for singletons**: The `AppSettingsService` correctly uses `IOptionsMonitor<AppSettings>` since it's a singleton service, which is the recommended approach for accessing configuration that might change at runtime.
2.  **Custom section name constant**: The `AppSettings` class properly defines a constant `SectionName` to avoid magic strings.
3.  **Strong typing**: Configuration values are strongly typed with the `AppSettings` class.
4.  **Proper validation**: Using `IValidateOptions<AppSettings>` with custom `AppSettingsValidator` for complex validation rules.
5.  **Change notification**: The service properly exposes an `OnChange` method to register callbacks for configuration changes.

### Recommended Improvements

#### 1. Add Data Annotations for Basic Validation

While the project already has custom validation using `IValidateOptions`, adding data annotations can make basic validations clearer and provide automatic error messages:

```csharp
using System.ComponentModel.DataAnnotations;

public class AppSettings
{
    public const string SectionName = "AppSettings";

    [Required(ErrorMessage = "Application name is required")]
    [StringLength(100, ErrorMessage = "Application name cannot exceed 100 characters")]
    public string ApplicationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Environment type is required")]
    public EnvironmentType Environment { get; set; } = EnvironmentType.Unknown;
}
```

Then update the registration to use both validation methods:

```csharp
services
    .AddOptions<AppSettings>()
    .BindConfiguration(AppSettings.SectionName)
    .ValidateDataAnnotations() // Add this line
    .ValidateOnStart();
```

#### 2. Use Named Options Pattern for Multiple Configurations

If your application needs multiple configuration sections of the same type (e.g., multiple connections), add support for named options:

```csharp
// Registration
services.Configure<AppSettings>("Primary", configuration.GetSection("PrimaryAppSettings"));
services.Configure<AppSettings>("Secondary", configuration.GetSection("SecondaryAppSettings"));

// Usage
public class SomeService
{
    private readonly AppSettings _primarySettings;
    private readonly AppSettings _secondarySettings;
    
    public SomeService(IOptionsSnapshot<AppSettings> optionsAccessor)
    {
        _primarySettings = optionsAccessor.Get("Primary");
        _secondarySettings = optionsAccessor.Get("Secondary");
    }
}
```

#### 3. Use the Right Options Interface Based on Lifetime

The current implementation uses `IOptionsMonitor` correctly for singleton services, but make sure to follow these guidelines for different service lifetimes:

*   For **Singleton** services: Use `IOptionsMonitor<T>` (current approach is correct)
*   For **Scoped** services: Use `IOptionsSnapshot<T>` which is recomputed per request
*   For **Transient** services with static config: Use `IOptions<T>`

#### 4. Add Required Properties in C# 11+

Use the `required` keyword for required properties in the options class:

```csharp
public class AppSettings
{
    public const string SectionName = "AppSettings";

    [Required]
    public required string ApplicationName { get; set; }
    
    [Required]
    public required EnvironmentType Environment { get; set; }
}
```

This adds compile-time checking for required properties.

#### 5. Add Delegate-based Validation

For more complex validation rules that can't be expressed with data annotations, add delegate-based validation:

```csharp
services
    .AddOptions<AppSettings>()
    .BindConfiguration(AppSettings.SectionName)
    .ValidateDataAnnotations()
    .Validate(settings => 
    {
        if (settings.Environment == EnvironmentType.Production && 
            settings.ApplicationName.Length < 5)
        {
            return false;
        }
        return true;
    }, "Production application names must be at least 5 characters.")
    .ValidateOnStart();
```

#### 6. Use Options Builder for Complex Configuration Scenarios

For options that depend on other services during configuration:

```csharp
services.AddOptions<AppSettings>()
    .Configure<IExampleService>((settings, service) => 
    {
        settings.SomeProperty = service.GetConfigValue();
    });
```

#### 7. Use a Consistent Pattern for All Configuration Classes

Create a template for all configuration classes in your application:

```csharp
/// <summary>
/// Configuration options for [Feature/Component]
/// </summary>
public class ExampleOptions
{
    /// <summary>
    /// Section name in configuration
    /// </summary>
    public const string Section = "Example";
    
    /// <summary>
    /// Gets or sets the [property description]
    /// </summary>
    [Required]
    public required string SomeProperty { get; set; }
    
    // Other properties...
}
```

#### 8. Use Post-Configuration for Default Values

To ensure default values are applied after all other configuration:

```csharp
services.PostConfigure<AppSettings>(settings => 
{
    // Apply defaults to any null or empty values
    settings.ApplicationName = string.IsNullOrEmpty(settings.ApplicationName) 
        ? "Default App Name" 
        : settings.ApplicationName;
});
```

### Implementation Example

Let's implement a new options class for a hypothetical feature following all best practices:

**Options Class (`EmailOptions.cs`):**
```csharp
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Configuration options for the email notification system
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Section name in configuration
    /// </summary>
    public const string Section = "Email";
    
    /// <summary>
    /// Gets or sets the SMTP server address
    /// </summary>
    [Required]
    [Url(ErrorMessage = "SMTP server address must be a valid URL")]
    public required string SmtpServer { get; set; }
    
    /// <summary>
    /// Gets or sets the SMTP port
    /// </summary>
    [Required]
    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535")]
    public int Port { get; set; }
    
    /// <summary>
    /// Gets or sets the sender email address
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Sender must be a valid email address")]
    public required string SenderEmail { get; set; }
    
    /// <summary>
    /// Gets or sets whether to use SSL for SMTP connections
    /// </summary>
    public bool UseSsl { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the username for SMTP authentication
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password for SMTP authentication
    /// </summary>
    public string? Password { get; set; }
}
```

**Registration (in `DependencyInjection.cs` or `Program.cs`):**
```csharp
services
    .AddOptions<EmailOptions>()
    .BindConfiguration(EmailOptions.Section)
    .ValidateDataAnnotations()
    .Validate(options => {
        // Custom validation: if username is provided, password must also be provided
        if (!string.IsNullOrEmpty(options.Username) && string.IsNullOrEmpty(options.Password))
        {
            return false;
        }
        return true;
    }, "Password must be provided when username is specified.")
    .ValidateOnStart();

// Register a validator service if complex validation logic is in its own class
// services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();
```

**Service Example (`EmailService.cs`):**
```csharp
public interface IEmailService
{
    Task SendEmailAsync(string recipient, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IOptionsMonitor<EmailOptions> _optionsMonitor;
    
    public EmailService(IOptionsMonitor<EmailOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }
    
    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var options = _optionsMonitor.CurrentValue;
        // Implementation using options.SmtpServer, options.Port, etc.
        // ...
        await Task.CompletedTask; // Placeholder
    }
}
```

### Conclusion

The SoftwareEngineerSkills project already implements many best practices for the IOptions pattern. By applying the suggestions above, the configuration system can be further enhanced to fully align with ASP.NET Core 9 recommendations. The key improvements focus on:

1.  Adding data annotations for basic validation.
2.  Using the `required` keyword for non-nullable properties.
3.  Ensuring the appropriate options interface (`IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`) is used based on service lifetime and configuration reload needs.
4.  Adding delegate-based validation for more complex rules directly in the registration pipeline.
5.  Implementing consistent patterns and structure across all configuration classes.
6.  Using post-configuration for setting default values reliably.

Following these improvements will lead to a more robust, self-documenting, and maintainable configuration system.

## Tasks and Subtasks

*(Please add your specific tasks and subtasks here)*

### General Improvements
*   [ ] Task:
    *   [ ] Subtask:
    *   [ ] Subtask:

### New Configuration Sections
*   [ ] Task: Implement `[NewFeature]Options`
    *   [ ] Subtask: Define `[NewFeature]Options` class with properties and data annotations.
    *   [ ] Subtask: Add `[NewFeature]` section to `appsettings.json` and environment-specific files.
    *   [ ] Subtask: Register `[NewFeature]Options` in `DependencyInjection.cs` with validation.
    *   [ ] Subtask: Create or update service to consume `[NewFeature]Options`.
    *   [ ] Subtask: Add unit tests for the new options and consuming service.

---
*This document was prepared on 17 de mayo de 2025.*
