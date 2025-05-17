namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Service interface for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email to the specified recipient
    /// </summary>
    /// <param name="recipient">The email address of the recipient</param>
    /// <param name="subject">The subject of the email</param>
    /// <param name="body">The body of the email</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(string recipient, string subject, string body);
    
    /// <summary>
    /// Register a callback to be invoked when email settings change
    /// </summary>
    /// <param name="listener">The callback to invoke when settings change</param>
    /// <returns>A disposable that can be used to unregister the callback</returns>
    IDisposable OnEmailSettingsChange(Action<EmailOptions> listener);
}
