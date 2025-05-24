using FluentAssertions;
using SoftwareEngineerSkills.Domain.Enums;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Enums;

public class SkillLevelTests
{
    [Fact]
    public void SkillLevel_ShouldHaveExpectedValues()
    {
        // Arrange & Act - Get all enum values
        var values = Enum.GetValues<SkillLevel>();
        
        // Assert
        values.Should().Contain(SkillLevel.Beginner);
        values.Should().Contain(SkillLevel.Elementary);
        values.Should().Contain(SkillLevel.Intermediate);
        values.Should().Contain(SkillLevel.Advanced);
        values.Should().Contain(SkillLevel.Expert);
    }

    [Fact]
    public void SkillLevel_ShouldHaveExpectedCount()
    {
        // Arrange & Act - Get all enum values
        var values = Enum.GetValues<SkillLevel>();
        
        // Assert
        values.Should().HaveCount(5); // Update this count if enum values change
    }

    [Fact]
    public void SkillLevel_ShouldHaveExpectedNumericValues()
    {
        // Assert
        ((int)SkillLevel.Beginner).Should().Be(1);
        ((int)SkillLevel.Elementary).Should().Be(2);
        ((int)SkillLevel.Intermediate).Should().Be(3);
        ((int)SkillLevel.Advanced).Should().Be(4);
        ((int)SkillLevel.Expert).Should().Be(5);
    }

    [Theory]
    [InlineData("Beginner")]
    [InlineData("Elementary")]
    [InlineData("Intermediate")]
    [InlineData("Advanced")]
    [InlineData("Expert")]
    public void SkillLevel_ShouldParse_FromValidString(string levelName)
    {
        // Act
        var result = Enum.TryParse<SkillLevel>(levelName, out var level);
        
        // Assert
        result.Should().BeTrue();
        Enum.IsDefined(typeof(SkillLevel), level).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Unknown")]
    [InlineData("beginner")]
    [InlineData("BEGINNER")]
    [InlineData("Basic")]
    public void SkillLevel_ShouldNotParse_FromInvalidString(string invalidLevelName)
    {
        // Act
        var result = Enum.TryParse<SkillLevel>(invalidLevelName, out var level);
        
        // Assert
        if (result)
        {
            // If it parsed successfully, make sure it's not a valid defined value
            Enum.IsDefined(typeof(SkillLevel), level).Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(1, SkillLevel.Beginner)]
    [InlineData(2, SkillLevel.Elementary)]
    [InlineData(3, SkillLevel.Intermediate)]
    [InlineData(4, SkillLevel.Advanced)]
    [InlineData(5, SkillLevel.Expert)]
    public void SkillLevel_ShouldCast_FromInt(int value, SkillLevel expected)
    {
        // Act
        var level = (SkillLevel)value;
        
        // Assert
        level.Should().Be(expected);
    }

    [Fact]
    public void SkillLevel_ShouldBeComparable()
    {
        // Assert
        (SkillLevel.Beginner < SkillLevel.Elementary).Should().BeTrue();
        (SkillLevel.Elementary < SkillLevel.Intermediate).Should().BeTrue();
        (SkillLevel.Intermediate < SkillLevel.Advanced).Should().BeTrue();
        (SkillLevel.Advanced < SkillLevel.Expert).Should().BeTrue();
        
        (SkillLevel.Expert > SkillLevel.Advanced).Should().BeTrue();
        (SkillLevel.Advanced > SkillLevel.Intermediate).Should().BeTrue();
        (SkillLevel.Intermediate > SkillLevel.Elementary).Should().BeTrue();
        (SkillLevel.Elementary > SkillLevel.Beginner).Should().BeTrue();
    }
}
