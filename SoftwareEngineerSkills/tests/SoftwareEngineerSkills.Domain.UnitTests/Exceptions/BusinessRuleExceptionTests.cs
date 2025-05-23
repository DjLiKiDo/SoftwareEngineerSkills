using SoftwareEngineerSkills.Domain.Exceptions;

namespace SoftwareEngineerSkills.Domain.UnitTests.Exceptions;

public class BusinessRuleExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var exception = new BusinessRuleException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MessageConstructor_ShouldCreateInstanceWithMessage()
    {
        // Arrange
        var errorMessage = "Business rule violated";

        // Act
        var exception = new BusinessRuleException(errorMessage);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ShouldCreateInstanceWithMessageAndInnerException()
    {
        // Arrange
        var errorMessage = "Business rule violated";
        var innerException = new ArgumentException("Inner exception");

        // Act
        var exception = new BusinessRuleException(errorMessage, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }

    [Fact]
    public void BusinessRuleException_ShouldInheritFromDomainException()
    {
        // Act
        var exception = new BusinessRuleException();

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }
}
