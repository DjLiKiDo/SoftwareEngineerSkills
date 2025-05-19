using FluentAssertions;
using SoftwareEngineerSkills.Domain.Exceptions;
using SoftwareEngineerSkills.Domain.ValueObjects;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Constructor_ValidEmail_ShouldCreateInstance()
    {
        // Arrange
        var validEmail = "test@example.com";

        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NullOrEmptyEmail_ShouldThrowBusinessRuleException(string invalidEmail)
    {
        // Arrange & Act
        Action action = () => new Email(invalidEmail);

        // Assert
        action.Should().Throw<BusinessRuleException>()
            .WithMessage("Email cannot be empty");
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid@com")]
    [InlineData("invalid@.com")]
    [InlineData("invalid@domain.")]
    [InlineData("invalid@domain.c")] // TLD too short
    public void Constructor_InvalidEmailFormat_ShouldThrowBusinessRuleException(string invalidEmail)
    {
        // Arrange & Act
        Action action = () => new Email(invalidEmail);

        // Assert
        action.Should().Throw<BusinessRuleException>()
            .WithMessage("Email has an invalid format");
    }

    [Fact]
    public void ImplicitConversion_FromEmail_ShouldReturnString()
    {
        // Arrange
        var emailString = "test@example.com";
        var email = new Email(emailString);

        // Act
        string result = email;

        // Assert
        result.Should().Be(emailString);
    }

    [Fact]
    public void ExplicitConversion_FromString_ShouldReturnEmail()
    {
        // Arrange
        var emailString = "test@example.com";

        // Act
        var email = (Email)emailString;

        // Assert
        email.Value.Should().Be(emailString);
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var emailString = "test@example.com";
        var email = new Email(emailString);

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be(emailString);
    }

    [Fact]
    public void Equals_SameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
        (email1 == email2).Should().BeFalse();
    }

    [Fact]
    public void Equals_CaseInsensitive_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("TEST@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Should().Be(email2);
    }

    [Fact]
    public void GetHashCode_SameEmail_ShouldReturnSameHashCode()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentEmail_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
