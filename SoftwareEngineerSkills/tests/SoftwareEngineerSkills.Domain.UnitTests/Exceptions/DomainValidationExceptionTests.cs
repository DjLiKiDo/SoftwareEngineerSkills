using FluentAssertions;
using SoftwareEngineerSkills.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Exceptions;

public class DomainValidationExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateInstanceWithDefaultMessage()
    {
        // Arrange & Act
        var exception = new DomainValidationException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("One or more domain validation errors occurred.");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MessageConstructor_ShouldCreateInstanceWithMessageAndAddToErrors()
    {
        // Arrange
        var errorMessage = "Name is required";

        // Act
        var exception = new DomainValidationException(errorMessage);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.Errors.Should().HaveCount(1);
        exception.Errors[0].Should().Be(errorMessage);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void ErrorsConstructor_ShouldCreateInstanceWithMultipleErrors()
    {
        // Arrange
        var errors = new List<string>
        {
            "Name is required",
            "Email is invalid",
            "Age must be positive"
        };

        // Act
        var exception = new DomainValidationException(errors);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("One or more domain validation errors occurred.");
        exception.Errors.Should().HaveCount(3);
        exception.Errors.Should().BeEquivalentTo(errors);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ShouldCreateInstanceWithMessageAndInnerException()
    {
        // Arrange
        var errorMessage = "Validation failed";
        var innerException = new ArgumentException("Inner exception");

        // Act
        var exception = new DomainValidationException(errorMessage, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(errorMessage);
        exception.Errors.Should().HaveCount(1);
        exception.Errors[0].Should().Be(errorMessage);
        exception.InnerException.Should().BeEquivalentTo(innerException);
    }

    [Fact]
    public void DomainValidationException_ShouldInheritFromBusinessRuleException()
    {
        // Act
        var exception = new DomainValidationException();

        // Assert
        exception.Should().BeAssignableTo<BusinessRuleException>();
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact] 
    public void ErrorsConstructor_WithEmptyList_ShouldCreateInstanceWithNoErrors()
    {
        // Arrange
        var errors = new List<string>();

        // Act
        var exception = new DomainValidationException(errors);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("One or more domain validation errors occurred.");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
    }
}
