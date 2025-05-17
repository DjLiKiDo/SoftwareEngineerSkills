namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Service interface for sending emails
/// </summary>
public interface IEmailService : IOptionsService<EmailOptions>
{
    /// <summary>
    /// Sends an email to the specified recipient
    /// </summary>
    /// <param name="recipient">The email address of the recipient</param>
    /// <param name="subject">The subject of the email</param>
    /// <param name="body">The body of the email</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(string recipient, string subject, string body);
}
