using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;
using System.Collections.Generic;

namespace SoftwareEngineerSkills.Infrastructure.Configuration.Validators;

/// <summary>
/// Validates EmailOptions configuration using IValidateOptions pattern
/// </summary>
public class EmailOptionsValidator : IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string? name, EmailOptions options)
    {
        var errors = new List<string>();
        
        // Custom validation: If username is provided, password must also be provided
        if (!string.IsNullOrEmpty(options.Username) && string.IsNullOrEmpty(options.Password))
        {
            errors.Add("Password must be provided when username is specified");
        }
        
        // Further validations could be added here
        if (options.Port == 0)
        {
            errors.Add("Port cannot be 0");
        }
        
        // Return appropriate result based on validation
        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors) 
            : ValidateOptionsResult.Success;
    }
}
