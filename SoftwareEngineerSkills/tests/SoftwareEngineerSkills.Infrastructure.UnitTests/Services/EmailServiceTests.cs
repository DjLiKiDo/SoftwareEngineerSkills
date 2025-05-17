using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SoftwareEngineerSkills.Application.Configuration;
using SoftwareEngineerSkills.Infrastructure.Services;
using Xunit;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Services;

public class EmailServiceTests
{
    private readonly Mock<IOptionsMonitor<EmailOptions>> _optionsMonitorMock;
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _optionsMonitorMock = new Mock<IOptionsMonitor<EmailOptions>>();
        _loggerMock = new Mock<ILogger<EmailService>>();
        
        // Setup default options
        var emailOptions = new EmailOptions
        {
            SmtpServer = "smtp.test.com",
            Port = 587,
            SenderEmail = "test@example.com",
            UseSsl = true,
            Username = "testuser",
            Password = "testpass"
        };
        
        _optionsMonitorMock
            .Setup(monitor => monitor.CurrentValue)
            .Returns(emailOptions);
        
        _emailService = new EmailService(_optionsMonitorMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task SendEmailAsync_ShouldUseConfiguredOptions()
    {
        // Arrange
        var recipient = "recipient@example.com";
        var subject = "Test Subject";
        var body = "Test Body";
        
        // Act
        await _emailService.SendEmailAsync(recipient, subject, body);
        
        // Assert
        // Verify that the CurrentValue property was accessed
        _optionsMonitorMock.Verify(monitor => monitor.CurrentValue, Times.Once);
        
        // Verify that the logger was called with expected parameters
        _loggerMock.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Sending email")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    [Fact]
    public void OnEmailSettingsChange_ShouldRegisterCallback()
    {
        // Arrange
        Action<EmailOptions> callback = _ => { };
        
        // Setup the OnChange method to return a disposable
        _optionsMonitorMock
            .Setup(monitor => monitor.OnChange(It.IsAny<Action<EmailOptions, string?>>()))
            .Returns(Mock.Of<IDisposable>());
        
        // Act
        var result = _emailService.OnEmailSettingsChange(callback);
        
        // Assert
        Assert.NotNull(result);
        _optionsMonitorMock.Verify(
            monitor => monitor.OnChange(It.IsAny<Action<EmailOptions, string?>>()),
            Times.Once);
    }
}
