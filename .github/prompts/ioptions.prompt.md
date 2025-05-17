# Task: Implement IOptions Pattern for [ConfigurationSectionName]

I need assistance with implementing the ASP.NET Core `IOptions` pattern for managing settings related to `[ConfigurationSectionName]`. This will involve creating a strongly-typed configuration class and registering it with the dependency injection container.

Please adhere to all guidelines, coding standards, and architectural patterns detailed in the project's Copilot instructions:
`@workspace /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/.github/prompts/project_copilot_guidelines.instructions.md`

Specifically, for this task, please help me with the following:

1.  **Create the Options Class:**
    *   Define a new C# class named `[ConfigurationSectionName]Options.cs` (e.g., `JwtOptions.cs`, `EmailServiceOptions.cs`) in the `SoftwareEngineerSkills.Application/Configuration` directory (or `SoftwareEngineerSkills.Infrastructure/Configuration` if it's more infrastructure-specific).
    *   This class should be a POCO with properties matching the keys in the `appsettings.json` for the `[ConfigurationSectionName]` section.
    *   Include a `public const string Section = "[ConfigurationSectionName]";` field in the options class.
    *   Add XML documentation comments to the class and its properties.
    *   Example `appsettings.json` structure:
        ```json
        {
          "[ConfigurationSectionName]": {
            "Property1": "value1",
            "Property2": 123,
            "NestedObject": {
              "NestedProperty": true
            }
          }
        }
        ```

2.  **Register the Options:**
    *   In the appropriate `DependencyInjection.cs` file (e.g., `SoftwareEngineerSkills.Application/DependencyInjection.cs` or `SoftwareEngineerSkills.Infrastructure/DependencyInjection.cs`) or directly in `SoftwareEngineerSkills.API/Program.cs` if it's a root-level configuration:
        *   Register the options class using `services.Configure<[ConfigurationSectionName]Options>(configuration.GetSection([ConfigurationSectionName]Options.Section));`
        *   Ensure `IConfiguration configuration` is available in the scope where you're registering.

3.  **Implement Validation (Optional but Recommended):**
    *   Add data annotations (e.g., `[Required]`, `[Url]`, `[Range]`) to the properties of the `[ConfigurationSectionName]Options` class for basic validation.
    *   Alternatively, or for more complex validation, show how to implement `IValidateOptions<[ConfigurationSectionName]Options>` or use the `.Validate()` extension method during registration. For example:
        ```csharp
        // In DependencyInjection.cs or Program.cs
        services.AddOptions<[ConfigurationSectionName]Options>()
            .Bind(configuration.GetSection([ConfigurationSectionName]Options.Section))
            .ValidateDataAnnotations() // For data annotations
            .Validate(options =>
            {
                // Custom validation logic here
                if (string.IsNullOrEmpty(options.Property1))
                {
                    return false;
                }
                return true;
            }, "Custom validation failed for Property1.");
        ```

4.  **Usage Example:**
    *   Show how to inject and use `IOptions<[ConfigurationSectionName]Options>` (or `IOptionsSnapshot` / `IOptionsMonitor` if appropriate, explaining the choice based on the guidelines in `@attachment IOptions pattern.md`) into a sample service or controller.
    *   For example, in a service within `SoftwareEngineerSkills.Application/Features/[SomeFeature]/[SomeService].cs`:
        ```csharp
        // private readonly [ConfigurationSectionName]Options _options;

        // public [SomeService](IOptions<[ConfigurationSectionName]Options> options)
        // {
        //     _options = options.Value;
        // }

        // // ... use _options.Property1 ...
        ```
    *   Remember to use C# 12 primary constructors where appropriate for DI.

The relevant files for this task might be:
`@file /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/src/SoftwareEngineerSkills.API/appsettings.json`
`@file /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/src/SoftwareEngineerSkills.Application/DependencyInjection.cs` (or other relevant DI setup file)
`@file /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/src/SoftwareEngineerSkills.Application/Configuration/[NewOptionsClass].cs` (to be created)
`@file /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/src/SoftwareEngineerSkills.Application/Features/[SomeFeature]/[SomeService].cs` (for usage example)

Refer to the official .NET 9 documentation via `context7` if specific new API details are needed, and consult the attached `IOptions pattern.md` for best practices regarding which `IOptions` interface to use.