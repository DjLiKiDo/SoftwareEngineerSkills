using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace SoftwareEngineerSkills.Infrastructure.Services.Email;

/// <summary>
/// Configuration for email service
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// SMTP server host
    /// </summary>
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
}

/// <summary>
/// Interface for email service
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="isHtml">Whether the body is HTML</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends an email with attachments
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="attachments">Email attachments</param>
    /// <param name="isHtml">Whether the body is HTML</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<Attachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of email service using SmtpClient
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;
    
    /// <summary>
    /// Creates a new instance of the SmtpEmailService class
    /// </summary>
    /// <param name="options">Email settings options</param>
    /// <param name="logger">Logger</param>
    public SmtpEmailService(IOptions<EmailSettings> options, ILogger<SmtpEmailService> logger)
    {
        _emailSettings = options.Value;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        await SendEmailWithAttachmentsAsync(to, subject, body, Enumerable.Empty<Attachment>(), isHtml, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<Attachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = CreateMessage(to, subject, body, attachments, isHtml);
            
            using var client = CreateSmtpClient();
            await client.SendMailAsync(message, cancellationToken);
            
            _logger.LogInformation("Email sent successfully to {EmailAddress} with subject {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {EmailAddress} with subject {Subject}", to, subject);
            throw;
        }
    }
    
    private MailMessage CreateMessage(string to, string subject, string body, IEnumerable<Attachment> attachments, bool isHtml)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.DefaultFrom),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };
        
        message.To.Add(to);
        
        foreach (var attachment in attachments)
        {
            message.Attachments.Add(attachment);
        }
        
        return message;
    }
    
    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = _emailSettings.EnableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
        };
        
        return client;
    }
}
