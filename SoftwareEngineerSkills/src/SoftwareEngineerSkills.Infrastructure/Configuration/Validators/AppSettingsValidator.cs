using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Domain.Common.Enums;
using System.Collections.Generic;

namespace SoftwareEngineerSkills.Infrastructure.Configuration.Validators;

/// <summary>
/// Validates AppSettings configuration using IValidateOptions pattern
/// </summary>
public class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {
        var errors = new List<string>();
        
        // Validate ApplicationName
        if (string.IsNullOrEmpty(options.ApplicationName))
        {
            errors.Add("Application name is required");
        }
        else if (options.ApplicationName.Length > 100)
        {
            errors.Add("Application name cannot exceed 100 characters");
        }
        
        // Validate Environment
        if (!Enum.IsDefined(typeof(EnvironmentType), options.Environment))
        {
            errors.Add("Environment must be a valid environment type");
        }
        
        // Additional business rules
        if (options.Environment == EnvironmentType.Production)
        {
            // This validation is already covered by the general ApplicationName check above,
            // but keeping it as an example of environment-specific rules
            if (string.IsNullOrEmpty(options.ApplicationName))
            {
                errors.Add("ApplicationName is required in Production environment");
            }
        }
        
        // Return appropriate result based on validation
        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors) 
            : ValidateOptionsResult.Success;
    }
}
