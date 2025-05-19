using FluentAssertions;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Configuration;

public class ValidatableSettingsTests
{
    [Fact]
    public void Validate_ValidSettings_ShouldReturnTrue()
    {
        // Arrange
        var settings = new TestValidatableSettings
        {
            RequiredProperty = "Not Empty",
            RangeProperty = 50
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
    
    [Fact]
    public void Validate_MissingRequiredProperty_ShouldReturnFalse()
    {
        // Arrange
        var settings = new TestValidatableSettings
        {
            RequiredProperty = "", // Empty string violates [Required]
            RangeProperty = 50
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(TestValidatableSettings.RequiredProperty)));
    }
    
    [Fact]
    public void Validate_PropertyOutOfRange_ShouldReturnFalse()
    {
        // Arrange
        var settings = new TestValidatableSettings
        {
            RequiredProperty = "Not Empty",
            RangeProperty = 150 // Out of range (max 100)
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().ContainSingle();
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(TestValidatableSettings.RangeProperty)));
    }
    
    [Fact]
    public void Validate_MultipleValidationErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var settings = new TestValidatableSettings
        {
            RequiredProperty = "", // Empty string violates [Required]
            RangeProperty = 150 // Out of range (max 100)
        };
        
        // Act
        var isValid = settings.Validate(out var validationResults);
        
        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().HaveCount(2);
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(TestValidatableSettings.RequiredProperty)));
        validationResults.Should().Contain(vr => vr.MemberNames.Contains(nameof(TestValidatableSettings.RangeProperty)));
    }
    
    // Test implementation of IValidatableSettings
    private class TestValidatableSettings : IValidatableSettings
    {
        [Required(ErrorMessage = "RequiredProperty is required")]
        public string RequiredProperty { get; set; } = string.Empty;
        
        [Range(1, 100, ErrorMessage = "RangeProperty must be between 1 and 100")]
        public int RangeProperty { get; set; }
        
        public bool Validate(out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true);
        }
    }
}
