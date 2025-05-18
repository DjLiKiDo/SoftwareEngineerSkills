using SoftwareEngineerSkills.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Infrastructure.Services.Email;

/// <summary>
/// Configuration for email service
/// </summary>
public class EmailSettings : IValidatableSettings
{
    /// <summary>
    /// The configuration section name
    /// </summary>
    public const string SectionName = "Email";

    /// <summary>
    /// SMTP server host
    /// </summary>
    [Required(ErrorMessage = "SMTP server is required")]
    public string SmtpServer { get; set; } = string.Empty;
    
    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort { get; set; } = 25;
    
    /// <summary>
    /// Whether to use SSL
    /// </summary>
    public bool EnableSsl { get; set; } = true;
    
    /// <summary>
    /// Default sender email address
    /// </summary>
    public string DefaultFrom { get; set; } = string.Empty;
    
    /// <summary>
    /// Username for SMTP authentication
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Password for SMTP authentication
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Validates the settings
    /// </summary>
    /// <returns>True if settings are valid, otherwise false</returns>
    public bool Validate(out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();

        // Validate SMTP server
        if (string.IsNullOrWhiteSpace(SmtpServer))
        {
            validationResults.Add(new ValidationResult(
                "SMTP server cannot be empty",
                new[] { nameof(SmtpServer) }));
        }

        // Validate SMTP port
        if (SmtpPort <= 0 || SmtpPort > 65535)
        {
            validationResults.Add(new ValidationResult(
                "SMTP port must be between 1 and 65535",
                new[] { nameof(SmtpPort) }));
        }

        // Validate DefaultFrom email format
        if (!string.IsNullOrEmpty(DefaultFrom) && 
            !EmailSettings.IsValidEmailFormat(DefaultFrom))
        {
            validationResults.Add(new ValidationResult(
                "Default from address must be a valid email format",
                new[] { nameof(DefaultFrom) }));
        }

        // If username is provided, password should also be provided
        if (!string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password))
        {
            validationResults.Add(new ValidationResult(
                "Password must be provided when username is specified",
                new[] { nameof(Password) }));
        }

        return validationResults.Count == 0;
    }

    /// <summary>
    /// Helper method to validate email format
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>True if valid format, otherwise false</returns>
    private static bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
