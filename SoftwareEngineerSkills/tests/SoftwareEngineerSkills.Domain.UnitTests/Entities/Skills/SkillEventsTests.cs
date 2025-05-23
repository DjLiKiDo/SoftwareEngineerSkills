using FluentAssertions;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities.Skills;

/// <summary>
/// Unit tests for Skill domain events
/// </summary>
public class SkillEventsTests
{
    #region SkillCreatedEvent Tests
    
    [Fact]
    public void Given_ValidParameters_When_SkillCreatedEventConstructed_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string skillName = "C#";
        const SkillCategory skillCategory = SkillCategory.Programming;
        var category = skillCategory.ToString();
        
        // Act
        var evt = new SkillCreatedEvent(skillId, skillName, skillCategory, category);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.SkillName.Should().Be(skillName);
        evt.SkillCategory.Should().Be(skillCategory);
        evt.Category.Should().Be(category);
        evt.EventId.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Given_ValidParameters_When_SkillCreatedEventConstructed_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillCreatedEvent(Guid.NewGuid(), "C#", SkillCategory.Programming, "Programming");
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
    
    #region SkillUpdatedEvent Tests
    
    [Fact]
    public void Given_ValidParameters_When_SkillUpdatedEventConstructed_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string oldName = "JavaScript";
        const string newName = "TypeScript";
        
        // Act
        var evt = new SkillUpdatedEvent(skillId, oldName, newName);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.OldName.Should().Be(oldName);
        evt.NewName.Should().Be(newName);
        evt.EventId.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Given_ValidParameters_When_SkillUpdatedEventConstructed_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillUpdatedEvent(Guid.NewGuid(), "Old", "New");
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
    
    #region SkillDemandChangedEvent Tests
    
    [Fact]
    public void Given_ValidParameters_When_SkillDemandChangedEventConstructed_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string skillName = "React";
        const bool isInDemand = true;
        
        // Act
        var evt = new SkillDemandChangedEvent(skillId, skillName, isInDemand);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.SkillName.Should().Be(skillName);
        evt.IsInDemand.Should().Be(isInDemand);
        evt.EventId.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Given_ValidParameters_When_SkillDemandChangedEventConstructed_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillDemandChangedEvent(Guid.NewGuid(), "React", true);
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
    
    #region SkillCategoryChangedEvent Tests
    
    [Fact]
    public void Given_ValidParameters_When_SkillCategoryChangedEventConstructed_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string skillName = "Docker";
        const SkillCategory oldCategory = SkillCategory.DevOps;
        const SkillCategory newCategory = SkillCategory.CloudComputing;
        
        // Act
        var evt = new SkillCategoryChangedEvent(skillId, skillName, oldCategory, newCategory);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.SkillName.Should().Be(skillName);
        evt.OldCategory.Should().Be(oldCategory);
        evt.NewCategory.Should().Be(newCategory);
        evt.EventId.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Given_ValidParameters_When_SkillCategoryChangedEventConstructed_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillCategoryChangedEvent(
            Guid.NewGuid(), 
            "Docker", 
            SkillCategory.DevOps, 
            SkillCategory.CloudComputing);
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
    
    #region SkillDifficultyChangedEvent Tests
    
    [Fact]
    public void Given_ValidParameters_When_SkillDifficultyChangedEventConstructed_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string skillName = "C#";
        const SkillLevel oldLevel = SkillLevel.Beginner;
        const SkillLevel newLevel = SkillLevel.Intermediate;
        
        // Act
        var evt = new SkillDifficultyChangedEvent(skillId, skillName, oldLevel, newLevel);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.SkillName.Should().Be(skillName);
        evt.OldLevel.Should().Be(oldLevel);
        evt.NewLevel.Should().Be(newLevel);
        evt.EventId.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Given_ValidParameters_When_SkillDifficultyChangedEventConstructed_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillDifficultyChangedEvent(
            Guid.NewGuid(),
            "C#",
            SkillLevel.Beginner,
            SkillLevel.Intermediate);
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
}
