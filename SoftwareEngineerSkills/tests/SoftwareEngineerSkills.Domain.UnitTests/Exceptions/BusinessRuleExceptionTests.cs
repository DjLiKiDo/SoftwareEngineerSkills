using FluentAssertions;
using SoftwareEngineerSkills.Domain.Exceptions;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Exceptions;

public class BusinessRuleExceptionTests
{
    private const string DefaultMessage = "Exception of type 'SoftwareEngineerSkills.Domain.Exceptions.BusinessRuleException' was thrown."; // Or whatever the default is for .NET version
    private const string CustomMessage = "This is a custom business rule violation message.";

    [Fact]
    public void Constructor_Default_ShouldCreateExceptionWithDefaultMessage()
    {
        // Arrange & Act
        var exception = new BusinessRuleException();

        // Assert
        exception.Should().NotBeNull();
        // The default message can vary slightly based on .NET version and localization.
        // It's often better to check that it's not null or empty if the exact message isn't critical.
        // However, for a specific known default, you can assert it.
        // For .NET Core, the message for parameterless Exception constructor is typically like:
        // "Exception of type 'YourNamespace.YourException' was thrown."
        // We will check if the message contains the type name for robustness.
        exception.Message.Should().Contain(nameof(BusinessRuleException)); 
    }

    [Fact]
    public void Constructor_WithMessage_ShouldCreateExceptionWithGivenMessage()
    {
        // Arrange & Act
        var exception = new BusinessRuleException(CustomMessage);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(CustomMessage);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateExceptionWithGivenMessageAndInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception message.");

        // Act
        var exception = new BusinessRuleException(CustomMessage, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(CustomMessage);
        exception.InnerException.Should().BeSameAs(innerException);
        exception.InnerException?.Message.Should().Be("Inner exception message.");
    }
}
