using FluentAssertions;
using SoftwareEngineerSkills.Domain.Enums;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Enums;

public class SkillCategoryTests
{
    [Fact]
    public void SkillCategory_ShouldHaveExpectedValues()
    {
        // Arrange & Act - Get all enum values
        var values = Enum.GetValues<SkillCategory>();
        
        // Assert
        values.Should().Contain(SkillCategory.ProgrammingLanguage);
        values.Should().Contain(SkillCategory.Framework);
        values.Should().Contain(SkillCategory.Database);
        values.Should().Contain(SkillCategory.Cloud);
        values.Should().Contain(SkillCategory.DevOps);
        values.Should().Contain(SkillCategory.Testing);
        values.Should().Contain(SkillCategory.Architecture);
        values.Should().Contain(SkillCategory.Tool);
        values.Should().Contain(SkillCategory.SoftSkill);
        values.Should().Contain(SkillCategory.Other);
    }

    [Fact]
    public void SkillCategory_ShouldHaveExpectedCount()
    {
        // Arrange & Act - Get all enum values
        var values = Enum.GetValues<SkillCategory>();
        
        // Assert
        values.Should().HaveCount(10); // Update this count if enum values change
    }

    [Theory]
    [InlineData("ProgrammingLanguage")]
    [InlineData("Framework")]
    [InlineData("Database")]
    [InlineData("Cloud")]
    [InlineData("DevOps")]
    [InlineData("Testing")]
    [InlineData("Architecture")]
    [InlineData("Tool")]
    [InlineData("SoftSkill")]
    [InlineData("Other")]
    public void SkillCategory_ShouldParse_FromValidString(string categoryName)
    {
        // Act
        var result = Enum.TryParse<SkillCategory>(categoryName, out var category);
        
        // Assert
        result.Should().BeTrue();
        Enum.IsDefined(typeof(SkillCategory), category).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("programming-language")]
    [InlineData("PROGRAMMING_LANGUAGE")]
    [InlineData("123")]
    public void SkillCategory_ShouldNotParse_FromInvalidString(string invalidCategoryName)
    {
        // Act
        var result = Enum.TryParse<SkillCategory>(invalidCategoryName, out var category);
        
        // Assert
        if (result)
        {
            // If it parsed successfully, make sure it's not a valid defined value
            Enum.IsDefined(typeof(SkillCategory), category).Should().BeFalse();
        }
    }

    [Fact]
    public void SkillCategory_ToString_ShouldReturnExpectedString()
    {
        // Assert
        SkillCategory.ProgrammingLanguage.ToString().Should().Be("ProgrammingLanguage");
        SkillCategory.Framework.ToString().Should().Be("Framework");
        SkillCategory.Database.ToString().Should().Be("Database");
        SkillCategory.Cloud.ToString().Should().Be("Cloud");
        SkillCategory.DevOps.ToString().Should().Be("DevOps");
        SkillCategory.Testing.ToString().Should().Be("Testing");
        SkillCategory.Architecture.ToString().Should().Be("Architecture");
        SkillCategory.Tool.ToString().Should().Be("Tool");
        SkillCategory.SoftSkill.ToString().Should().Be("SoftSkill");
        SkillCategory.Other.ToString().Should().Be("Other");
    }
}
