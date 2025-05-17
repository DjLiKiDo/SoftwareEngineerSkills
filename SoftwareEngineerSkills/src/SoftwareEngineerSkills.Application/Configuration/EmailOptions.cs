using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Application.Configuration;

/// <summary>
/// Configuration options for the email notification system
/// </summary>
public class EmailOptions : BaseOptions
{
    /// <summary>
    /// Section name in configuration
    /// </summary>
    public const string SectionNameConst = "EmailSettings";

    /// <inheritdoc />
    public override string SectionName => SectionNameConst;
    
    /// <summary>
    /// Gets or sets the SMTP server address
    /// </summary>
    [Required]
    [Url(ErrorMessage = "SMTP server address must be a valid URL")]
    public required string SmtpServer { get; set; }
    
    /// <summary>
    /// Gets or sets the SMTP port
    /// </summary>
    [Required]
    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535")]
    public int Port { get; set; }
    
    /// <summary>
    /// Gets or sets the sender email address
    /// </summary>
    [Required]
    [EmailAddress(ErrorMessage = "Sender must be a valid email address")]
    public required string SenderEmail { get; set; }
    
    /// <summary>
    /// Gets or sets whether to use SSL for SMTP connections
    /// </summary>
    public bool UseSsl { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the username for SMTP authentication
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Gets or sets the password for SMTP authentication
    /// </summary>
    public string? Password { get; set; }
}
