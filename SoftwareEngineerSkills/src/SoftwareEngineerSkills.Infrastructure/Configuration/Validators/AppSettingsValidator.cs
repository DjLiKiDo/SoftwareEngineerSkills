using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Configuration.Validators;

public class AppSettingsValidator : IValidateOptions<AppSettings>
{
    public ValidateOptionsResult Validate(string? name, AppSettings options)
    {       
        if (options.Environment == Domain.Common.Enums.EnvironmentType.Production)
        {
            if (string.IsNullOrEmpty(options.ApplicationName))
            {
                return ValidateOptionsResult.Fail("ApplicationName is required in Production environment");
            }
        }
        
        return ValidateOptionsResult.Success;
    }
}
