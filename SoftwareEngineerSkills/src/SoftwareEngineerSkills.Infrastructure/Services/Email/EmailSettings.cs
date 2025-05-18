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
