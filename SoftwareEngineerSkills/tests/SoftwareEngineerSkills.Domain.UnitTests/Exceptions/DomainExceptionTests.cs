using FluentAssertions;
using SoftwareEngineerSkills.Domain.Exceptions;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Exceptions;

public class DomainExceptionTests
{
    private class ConcreteDomainException : DomainException
    {
        public ConcreteDomainException() : base() { }
        public ConcreteDomainException(string message) : base(message) { }
        public ConcreteDomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var exception = new ConcreteDomainException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MessageConstructor_ShouldCreateInstanceWithMessage()
    {
        // Arrange
        var errorMessage = "Domain rule violated";

        // Act
        var exception = new ConcreteDomainException(errorMessage);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ShouldCreateInstanceWithMessageAndInnerException()
    {
        // Arrange
        var errorMessage = "Domain rule violated";
        var innerException = new ArgumentException("Inner exception");

        // Act
        var exception = new ConcreteDomainException(errorMessage, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }

    [Fact]
    public void DomainException_ShouldInheritFromException()
    {
        // Act
        var exception = new ConcreteDomainException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
