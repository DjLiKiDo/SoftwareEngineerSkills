using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;

namespace SoftwareEngineerSkills.Infrastructure.Services;

/// <summary>
/// Implementation of the email service using IOptionsMonitor
/// </summary>
public class EmailService : IEmailService
{
    private readonly IOptionsMonitor<EmailOptions> _optionsMonitor;
    private readonly ILogger<EmailService> _logger;

    public ApplicationOptions CurrentSettings => throw new NotImplementedException();

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="optionsMonitor">The options monitor for email settings</param>
    /// <param name="logger">The logger</param>
    public EmailService(IOptionsMonitor<EmailOptions> optionsMonitor, ILogger<EmailService> logger)
    {
        _optionsMonitor = optionsMonitor;
        _logger = logger;

        // Log when configuration changes
        _optionsMonitor.OnChange(options =>
        {
            _logger.LogInformation("Email configuration has changed");
        });
    }

    /// <inheritdoc />
    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var options = _optionsMonitor.CurrentValue;

        // In a real implementation, this would use SmtpClient or a third-party library
        // to send emails using the configuration from options
        _logger.LogInformation("Sending email to {Recipient} using SMTP server {Server}:{Port}",
            recipient, options.SmtpServer, options.Port);

        // This is a placeholder for the actual email sending implementation
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public IDisposable OnEmailSettingsChange(Action<EmailOptions> listener)
    {
        return _optionsMonitor.OnChange(listener)!;
    }

    public IDisposable OnChange(Action<EmailOptions> listener)
    {
        throw new NotImplementedException();
    }
}
