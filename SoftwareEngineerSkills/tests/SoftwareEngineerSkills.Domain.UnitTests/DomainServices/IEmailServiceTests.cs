using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SoftwareEngineerSkills.Domain.DomainServices.Interfaces;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.DomainServices;

/// <summary>
/// Unit tests for IEmailService interface
/// </summary>
public class IEmailServiceTests
{
    [Fact]
    public void IEmailService_ShouldHaveCorrectMethods()
    {
        // Arrange & Act
        var interfaceType = typeof(IEmailService);
        var methods = interfaceType.GetMethods();
        
        // Assert
        methods.Should().Contain(m => m.Name == "SendEmailAsync" && 
                                 m.GetParameters().Length >= 3);
        
        methods.Should().Contain(m => m.Name == "SendEmailWithAttachmentsAsync" && 
                                 m.GetParameters().Length >= 4);
    }
    
    [Fact]
    public async Task SendEmailAsync_ShouldBeCallable()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        
        mockEmailService
            .Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await mockEmailService.Object.SendEmailAsync(
            "test@example.com",
            "Test Subject",
            "Test Body",
            true,
            CancellationToken.None);
        
        // Assert
        mockEmailService.Verify(
            s => s.SendEmailAsync(
                "test@example.com",
                "Test Subject",
                "Test Body",
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SendEmailWithAttachmentsAsync_ShouldBeCallable()
    {
        // Arrange
        var mockEmailService = new Mock<IEmailService>();
        var attachments = new List<Attachment>();
        
        mockEmailService
            .Setup(s => s.SendEmailWithAttachmentsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<Attachment>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Act
        await mockEmailService.Object.SendEmailWithAttachmentsAsync(
            "test@example.com",
            "Test Subject",
            "Test Body",
            attachments,
            true,
            CancellationToken.None);
        
        // Assert
        mockEmailService.Verify(
            s => s.SendEmailWithAttachmentsAsync(
                "test@example.com",
                "Test Subject",
                "Test Body",
                attachments,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public void IEmailService_ShouldBePublicInterface()
    {
        // Arrange
        var interfaceType = typeof(IEmailService);
        
        // Act & Assert
        interfaceType.IsInterface.Should().BeTrue();
        interfaceType.IsPublic.Should().BeTrue();
        interfaceType.IsAbstract.Should().BeTrue();
    }
    
    [Fact]
    public void IEmailService_MethodsReturnType_ShouldBeTask()
    {
        // Arrange
        var interfaceType = typeof(IEmailService);
        var sendEmailMethod = interfaceType.GetMethod("SendEmailAsync");
        var sendWithAttachmentsMethod = interfaceType.GetMethod("SendEmailWithAttachmentsAsync");
        
        // Act & Assert
        sendEmailMethod.Should().NotBeNull();
        sendWithAttachmentsMethod.Should().NotBeNull();
        
        sendEmailMethod!.ReturnType.Should().Be(typeof(Task));
        sendWithAttachmentsMethod!.ReturnType.Should().Be(typeof(Task));
    }
}
