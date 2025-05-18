// filepath: /Users/marquez/Downloads/Pablo/Repos/SoftwareEngineerSkills/SoftwareEngineerSkills/tests/SoftwareEngineerSkills.Domain.UnitTests/ValueObjects/EmailTests.cs
using FluentAssertions;
using SoftwareEngineerSkills.Domain.Exceptions;
using SoftwareEngineerSkills.Domain.ValueObjects;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.ValueObjects;

public class EmailTests
{
    private const string ValidEmail = "test@example.com";
    private const string ValidEmailWithSubdomain = "test@sub.example.com";
    private const string ValidEmailWithPlusAlias = "test+alias@example.com";
    private const string InvalidEmailNoAt = "testexample.com";
    private const string InvalidEmailNoDomain = "test@";
    private const string InvalidEmailNoUser = "@example.com";
    private const string InvalidEmailWithSpaces = "test @example.com";
    private const string EmptyEmail = "";
    private const string WhitespaceEmail = "   ";

    [Theory]
    [InlineData(ValidEmail)]
    [InlineData(ValidEmailWithSubdomain)]
    [InlineData(ValidEmailWithPlusAlias)]
    public void Constructor_WithValidEmail_ShouldCreateEmail(string validEmailValue)
    {
        // Arrange & Act
        var email = new Email(validEmailValue);

        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(validEmailValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyEmail)]
    [InlineData(WhitespaceEmail)]
    public void Constructor_WithNullOrEmptyOrWhitespaceEmail_ShouldThrowBusinessRuleException(string? invalidEmailValue) // Made string nullable
    {
        // Arrange & Act
        Action act = () => new Email(invalidEmailValue!);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Email cannot be empty");
    }

    [Theory]
    [InlineData(InvalidEmailNoAt)]
    [InlineData(InvalidEmailNoDomain)]
    [InlineData(InvalidEmailNoUser)]
    [InlineData(InvalidEmailWithSpaces)]
    [InlineData("test@.com")]
    [InlineData("test@com.")]
    [InlineData("test@com.c")] // Invalid TLD
    public void Constructor_WithInvalidEmailFormat_ShouldThrowBusinessRuleException(string invalidEmailValue)
    {
        // Arrange & Act
        Action act = () => new Email(invalidEmailValue);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Email has an invalid format");
    }

    [Fact]
    public void GetEqualityComponents_ShouldReturnCorrectValue()
    {
        // Arrange
        var email = new Email(ValidEmail);
        var emailUpper = new Email("TEST@EXAMPLE.COM");

        // Act & Assert
        email.Should().Be(emailUpper); // Equality should be case-insensitive for the value part
        email.GetHashCode().Should().Be(emailUpper.GetHashCode());
    }

    [Fact]
    public void ImplicitOperatorToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email(ValidEmail);

        // Act
        string emailString = email;

        // Assert
        emailString.Should().Be(ValidEmail);
    }

    [Fact]
    public void ExplicitOperatorToEmail_ShouldCreateEmailFromString()
    {
        // Arrange
        var emailString = ValidEmail;

        // Act
        var email = (Email)emailString;

        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(ValidEmail);
    }

    [Fact]
    public void ExplicitOperatorToEmail_WithInvalidString_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var invalidEmailString = InvalidEmailNoAt;

        // Act
        Action act = () => { var email = (Email)invalidEmailString; };

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Email has an invalid format");
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email(ValidEmail);

        // Act
        var emailString = email.ToString();

        // Assert
        emailString.Should().Be(ValidEmail);
    }

    [Fact]
    public void Equals_WithDifferentTypes_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email(ValidEmail);
        var otherObject = new object();

        // Act
        var result = email.Equals(otherObject);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email(ValidEmail);

        // Act
        var result = email.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // Arrange
        var email = new Email(ValidEmail);

        // Act
        var result = email.Equals(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentEmailValue_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email(ValidEmail);
        var email2 = new Email("another@example.com");

        // Act
        var result = email1.Equals(email2);

        // Assert
        result.Should().BeFalse();
    }
}
