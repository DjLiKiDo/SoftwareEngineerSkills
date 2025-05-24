using FluentAssertions;
using SoftwareEngineerSkills.Domain.DomainServices.Interfaces;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.DomainServices.Interfaces;

public class IEmailServiceTests
{
    [Fact]
    public void IEmailService_ShouldBeInterfaceWithExpectedMembers()
    {
        // Arrange
        var type = typeof(IEmailService);

        // Act & Assert
        type.IsInterface.Should().BeTrue();
        // Add further checks for expected methods as needed
    }

    [Fact]
    public void IEmailService_ShouldDeclareExpectedMethods()
    {
        // Arrange
        var type = typeof(IEmailService);

        // Act
        var sendEmailAsync = type.GetMethod("SendEmailAsync");
        var sendWithAttachmentsAsync = type.GetMethod("SendEmailWithAttachmentsAsync");

        // Assert
        sendEmailAsync.Should().NotBeNull();
        sendWithAttachmentsAsync.Should().NotBeNull();
        sendEmailAsync!.ReturnType.Should().Be(typeof(Task));
        sendWithAttachmentsAsync!.ReturnType.Should().Be(typeof(Task));
        sendEmailAsync.GetParameters().Should().Contain(p => p.Name == "to" && p.ParameterType == typeof(string));
        sendEmailAsync.GetParameters().Should().Contain(p => p.Name == "subject" && p.ParameterType == typeof(string));
        sendEmailAsync.GetParameters().Should().Contain(p => p.Name == "body" && p.ParameterType == typeof(string));
        sendWithAttachmentsAsync.GetParameters().Should().Contain(p => p.Name == "attachments");
    }
}
