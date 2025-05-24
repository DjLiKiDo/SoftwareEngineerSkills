using SoftwareEngineerSkills.Infrastructure.Services.Email;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Services.Email;

public class EmailSettingsTests
{
    [Fact]
    public void Validate_ValidSettings_ShouldReturnTrue()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            EnableSsl = true,
            DefaultFrom = "test@example.com",
            Username = "user",
            Password = "password"
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
    
    [Fact]
    public void Validate_EmptySmtpServer_ShouldReturnFalse()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "",
            SmtpPort = 587,
            EnableSsl = true,
            DefaultFrom = "test@example.com"
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle();
        
        var validationResult = validationResults.First();
        validationResult.ErrorMessage.Should().Contain("SMTP server cannot be empty");
        validationResult.MemberNames.Should().Contain(nameof(EmailSettings.SmtpServer));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(70000)]
    public void Validate_InvalidPort_ShouldReturnFalse(int invalidPort)
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = invalidPort,
            EnableSsl = true,
            DefaultFrom = "test@example.com"
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle();
        
        var validationResult = validationResults.First();
        validationResult.ErrorMessage.Should().Contain("SMTP port must be between 1 and 65535");
        validationResult.MemberNames.Should().Contain(nameof(EmailSettings.SmtpPort));
    }
    
    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    public void Validate_InvalidDefaultFromEmail_ShouldReturnFalse(string invalidEmail)
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            EnableSsl = true,
            DefaultFrom = invalidEmail
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle();
        
        var validationResult = validationResults.First();
        validationResult.ErrorMessage.Should().Contain("Default from address must be a valid email format");
        validationResult.MemberNames.Should().Contain(nameof(EmailSettings.DefaultFrom));
    }
    
    [Fact]
    public void Validate_EmptyDefaultFromEmail_ShouldBeValid()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            EnableSsl = true,
            DefaultFrom = "" // Empty is allowed (might be configured elsewhere)
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
    
    [Fact]
    public void Validate_UsernameWithoutPassword_ShouldReturnFalse()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            EnableSsl = true,
            DefaultFrom = "test@example.com",
            Username = "user",
            Password = "" // Missing password
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle();
        
        var validationResult = validationResults.First();
        validationResult.ErrorMessage.Should().Contain("Password must be provided when username is specified");
        validationResult.MemberNames.Should().Contain(nameof(EmailSettings.Password));
    }
    
    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var settings = new EmailSettings
        {
            SmtpServer = "",
            SmtpPort = 0,
            EnableSsl = true,
            DefaultFrom = "invalid-email",
            Username = "user",
            Password = "" // Missing password
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().HaveCount(4); // Four errors: empty server, invalid port, invalid email, missing password
    }
}
