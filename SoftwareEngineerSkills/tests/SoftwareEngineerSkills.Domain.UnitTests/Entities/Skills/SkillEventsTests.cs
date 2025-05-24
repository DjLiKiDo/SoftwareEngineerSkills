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
    public void SkillCreatedEvent_ConstructedWithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string skillName = "C#";
        const SkillCategory skillCategory = SkillCategory.ProgrammingLanguage;
        var category = skillCategory.ToString();
        
        // Act
        var evt = new SkillCreatedEvent(skillId, skillName, skillCategory, category);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.SkillName.Should().Be(skillName);
        evt.SkillCategory.Should().Be(skillCategory);
        evt.Category.Should().Be(category);
        evt.Id.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void SkillCreatedEvent_TypeChecked_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillCreatedEvent(Guid.NewGuid(), "C#", SkillCategory.ProgrammingLanguage, "ProgrammingLanguage");
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
    
    #region SkillUpdatedEvent Tests
    
    [Fact]
    public void SkillUpdatedEvent_ConstructedWithValidParameters_ShouldInitializeCorrectly()
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
        evt.Id.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void SkillUpdatedEvent_TypeChecked_ShouldInheritFromDomainEvent()
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
    public void SkillDemandChangedEvent_ConstructedWithValidParameters_ShouldInitializeCorrectly()
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
        evt.Id.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void SkillDemandChangedEvent_TypeChecked_ShouldInheritFromDomainEvent()
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
    public void SkillCategoryChangedEvent_ConstructedWithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        const string skillName = "Docker";
        const SkillCategory oldCategory = SkillCategory.DevOps;
        const SkillCategory newCategory = SkillCategory.Cloud;
        
        // Act
        var evt = new SkillCategoryChangedEvent(skillId, skillName, oldCategory, newCategory);
        
        // Assert
        evt.SkillId.Should().Be(skillId);
        evt.SkillName.Should().Be(skillName);
        evt.OldCategory.Should().Be(oldCategory);
        evt.NewCategory.Should().Be(newCategory);
        evt.Id.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void SkillCategoryChangedEvent_TypeChecked_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var evt = new SkillCategoryChangedEvent(
            Guid.NewGuid(), 
            "Docker", 
            SkillCategory.DevOps, 
            SkillCategory.Cloud);
        
        // Assert
        evt.Should().BeAssignableTo<DomainEvent>();
        evt.Should().BeAssignableTo<IDomainEvent>();
    }
    
    #endregion
    
    #region SkillDifficultyChangedEvent Tests
    
    [Fact]
    public void SkillDifficultyChangedEvent_ConstructedWithValidParameters_ShouldInitializeCorrectly()
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
        evt.Id.Should().NotBe(Guid.Empty);
        evt.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void SkillDifficultyChangedEvent_TypeChecked_ShouldInheritFromDomainEvent()
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
