using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Application.Configuration;

namespace SoftwareEngineerSkills.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ApiControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IOptionsSnapshot<EmailOptions> _emailOptions;
    private readonly ILogger<EmailController> _logger;

    public EmailController(
        IEmailService emailService,
        IOptionsSnapshot<EmailOptions> emailOptions,
        ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _emailOptions = emailOptions;
        _logger = logger;
    }

    [HttpGet("config")]
    public IActionResult GetConfiguration()
    {
        // Note: In a real application, you would not expose sensitive configuration like this
        // This is for demonstration purposes only
        var config = new
        {
            SmtpServer = _emailOptions.Value.SmtpServer,
            Port = _emailOptions.Value.Port,
            SenderEmail = _emailOptions.Value.SenderEmail,
            UseSsl = _emailOptions.Value.UseSsl,
            // Don't include sensitive fields like Username and Password
        };

        return Ok(config);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
        _logger.LogInformation("Sending email to {Recipient}", request.Recipient);

        await _emailService.SendEmailAsync(
            request.Recipient,
            request.Subject,
            request.Body);

        return Ok(new { Message = "Email sent successfully" });
    }
}

public class SendEmailRequest
{
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
