using FluentAssertions;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;
using System;
using System.Linq;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities.Skills;

/// <summary>
/// Unit tests for Skill entity
/// </summary>
public class SkillTests
{
    #region Constructor Tests
    
    [Fact]
    public void Given_ValidParameters_When_SkillCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        const string name = "C#";
        const SkillCategory category = SkillCategory.Programming;
        const string description = "Object-oriented programming language";
        const SkillLevel difficultyLevel = SkillLevel.Intermediate;
        const bool isInDemand = true;
        
        // Act
        var skill = new Skill(name, category, description, difficultyLevel, isInDemand);
        
        // Assert
        skill.Id.Should().NotBe(Guid.Empty);
        skill.Name.Should().Be(name);
        skill.Category.Should().Be(category);
        skill.Description.Should().Be(description);
        skill.DifficultyLevel.Should().Be(difficultyLevel);
        skill.IsInDemand.Should().BeTrue();
    }
    
    [Fact]
    public void Given_ValidParameters_When_SkillCreated_Then_ShouldRaiseSkillCreatedEvent()
    {
        // Arrange
        const string name = "Docker";
        const SkillCategory category = SkillCategory.DevOps;
        const string description = "Containerization platform";
        const SkillLevel difficultyLevel = SkillLevel.Intermediate;
        
        // Act
        var skill = new Skill(name, category, description, difficultyLevel);
        
        // Assert
        skill.DomainEvents.Should().HaveCount(1);
        var domainEvent = skill.DomainEvents.First().Should().BeOfType<SkillCreatedEvent>().Subject;
        domainEvent.SkillId.Should().Be(skill.Id);
        domainEvent.SkillName.Should().Be(name);
        domainEvent.SkillCategory.Should().Be(category);
        domainEvent.Category.Should().Be(category.ToString());
    }
    
    [Fact]
    public void Given_NullName_When_SkillCreated_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? name = null;
        const SkillCategory category = SkillCategory.Programming;
        const string description = "Object-oriented programming language";
        const SkillLevel difficultyLevel = SkillLevel.Intermediate;
        
        // Act & Assert
        var action = () => new Skill(name!, category, description, difficultyLevel);
        action.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("name");
    }
    
    [Fact]
    public void Given_NullDescription_When_SkillCreated_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        const string name = "C#";
        const SkillCategory category = SkillCategory.Programming;
        string? description = null;
        const SkillLevel difficultyLevel = SkillLevel.Intermediate;
        
        // Act & Assert
        var action = () => new Skill(name, category, description!, difficultyLevel);
        action.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("description");
    }
    
    [Fact]
    public void Given_EmptyName_When_EnforceInvariantsCalled_Then_ShouldThrowDomainValidationException()
    {
        // Arrange
        var skill = CreateValidSkill();
        
        // Using reflection to set name to empty string bypassing private setter
        var nameProperty = typeof(Skill).GetProperty("Name");
        nameProperty?.SetValue(skill, string.Empty);
        
        // Act & Assert
        var action = () => skill.EnforceInvariants();
        action.Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain("Skill name cannot be empty");
    }
    
    [Fact]
    public void Given_TooLongName_When_EnforceInvariantsCalled_Then_ShouldThrowDomainValidationException()
    {
        // Arrange
        var skill = CreateValidSkill();
        
        // Using reflection to set name to a string longer than 100 characters
        var nameProperty = typeof(Skill).GetProperty("Name");
        nameProperty?.SetValue(skill, new string('A', 101));
        
        // Act & Assert
        var action = () => skill.EnforceInvariants();
        action.Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain("Skill name cannot exceed 100 characters");
    }
    
    [Fact]
    public void Given_EmptyDescription_When_EnforceInvariantsCalled_Then_ShouldThrowDomainValidationException()
    {
        // Arrange
        var skill = CreateValidSkill();
        
        // Using reflection to set description to empty string bypassing private setter
        var descriptionProperty = typeof(Skill).GetProperty("Description");
        descriptionProperty?.SetValue(skill, string.Empty);
        
        // Act & Assert
        var action = () => skill.EnforceInvariants();
        action.Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain("Skill description cannot be empty");
    }
    
    [Fact]
    public void Given_TooLongDescription_When_EnforceInvariantsCalled_Then_ShouldThrowDomainValidationException()
    {
        // Arrange
        var skill = CreateValidSkill();
        
        // Using reflection to set description to a string longer than 1000 characters
        var descriptionProperty = typeof(Skill).GetProperty("Description");
        descriptionProperty?.SetValue(skill, new string('A', 1001));
        
        // Act & Assert
        var action = () => skill.EnforceInvariants();
        action.Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain("Skill description cannot exceed 1000 characters");
    }
    
    #endregion
    
    #region Update Tests
    
    [Fact]
    public void Given_ValidParameters_When_Update_Then_ShouldUpdateProperties()
    {
        // Arrange
        var skill = CreateValidSkill();
        skill.ClearDomainEvents(); // Clear creation events
        
        const string newName = "TypeScript";
        const SkillCategory newCategory = SkillCategory.WebDevelopment;
        const string newDescription = "JavaScript with types";
        const SkillLevel newLevel = SkillLevel.Advanced;
        const bool newIsInDemand = true;
        
        // Act
        skill.Update(newName, newCategory, newDescription, newLevel, newIsInDemand);
        
        // Assert
        skill.Name.Should().Be(newName);
        skill.Category.Should().Be(newCategory);
        skill.Description.Should().Be(newDescription);
        skill.DifficultyLevel.Should().Be(newLevel);
        skill.IsInDemand.Should().Be(newIsInDemand);
    }
    
    [Fact]
    public void Given_CategoryChange_When_Update_Then_ShouldRaiseSkillCategoryChangedEvent()
    {
        // Arrange
        var skill = CreateValidSkill();
        skill.ClearDomainEvents(); // Clear creation events
        
        const SkillCategory originalCategory = SkillCategory.ProgrammingLanguage;
        const SkillCategory newCategory = SkillCategory.Framework;
        
        // Act
        skill.Update(skill.Name, newCategory, skill.Description, skill.DifficultyLevel, skill.IsInDemand);
        
        // Assert
        skill.DomainEvents.Should().Contain(e => e is SkillCategoryChangedEvent);
        var categoryChangedEvent = skill.DomainEvents.OfType<SkillCategoryChangedEvent>().FirstOrDefault();
        categoryChangedEvent.Should().NotBeNull();
        categoryChangedEvent!.OldCategory.Should().Be(originalCategory);
        categoryChangedEvent.NewCategory.Should().Be(newCategory);
    }
    
    [Fact]
    public void Given_DemandStatusChange_When_Update_Then_ShouldRaiseSkillDemandChangedEvent()
    {
        // Arrange
        var skill = CreateValidSkill(isInDemand: false);
        skill.ClearDomainEvents(); // Clear creation events
        
        const bool newIsInDemand = true;
        
        // Act
        skill.Update(skill.Name, skill.Category, skill.Description, skill.DifficultyLevel, newIsInDemand);
        
        // Assert
        skill.DomainEvents.Should().Contain(e => e is SkillDemandChangedEvent);
        var demandChangedEvent = skill.DomainEvents.OfType<SkillDemandChangedEvent>().FirstOrDefault();
        demandChangedEvent.Should().NotBeNull();
        demandChangedEvent!.IsInDemand.Should().BeTrue();
    }
    
    [Fact]
    public void Given_NullName_When_Update_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var skill = CreateValidSkill();
        string? newName = null;
        
        // Act & Assert
        var action = () => skill.Update(newName!, skill.Category, skill.Description, skill.DifficultyLevel, skill.IsInDemand);
        action.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("name");
    }
    
    [Fact]
    public void Given_NullDescription_When_Update_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var skill = CreateValidSkill();
        string? newDescription = null;
        
        // Act & Assert
        var action = () => skill.Update(skill.Name, skill.Category, newDescription!, skill.DifficultyLevel, skill.IsInDemand);
        action.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("description");
    }
    
    #endregion
    
    #region SetDemandStatus Tests
    
    [Fact]
    public void Given_DifferentDemandStatus_When_SetDemandStatus_Then_ShouldUpdateAndRaiseEvent()
    {
        // Arrange
        var skill = CreateValidSkill(isInDemand: false);
        skill.ClearDomainEvents(); // Clear creation events
        
        // Act
        skill.SetDemandStatus(true);
        
        // Assert
        skill.IsInDemand.Should().BeTrue();
        skill.DomainEvents.Should().HaveCount(1);
        var demandChangedEvent = skill.DomainEvents.First().Should().BeOfType<SkillDemandChangedEvent>().Subject;
        demandChangedEvent.IsInDemand.Should().BeTrue();
    }
    
    [Fact]
    public void Given_SameDemandStatus_When_SetDemandStatus_Then_ShouldNotUpdateOrRaiseEvent()
    {
        // Arrange
        var skill = CreateValidSkill(isInDemand: true);
        skill.ClearDomainEvents(); // Clear creation events
        
        // Act
        skill.SetDemandStatus(true);
        
        // Assert
        skill.IsInDemand.Should().BeTrue();
        skill.DomainEvents.Should().BeEmpty();
    }
    
    #endregion
    
    #region UpdateDifficultyLevel Tests
    
    [Fact]
    public void Given_DifferentLevel_When_UpdateDifficultyLevel_Then_ShouldUpdateAndRaiseEvent()
    {
        // Arrange
        var skill = CreateValidSkill(difficultyLevel: SkillLevel.Beginner);
        skill.ClearDomainEvents(); // Clear creation events
        
        // Act
        skill.UpdateDifficultyLevel(SkillLevel.Advanced);
        
        // Assert
        skill.DifficultyLevel.Should().Be(SkillLevel.Advanced);
        skill.DomainEvents.Should().HaveCount(1);
        var difficultyChangedEvent = skill.DomainEvents.First().Should().BeOfType<SkillDifficultyChangedEvent>().Subject;
        difficultyChangedEvent.OldLevel.Should().Be(SkillLevel.Beginner);
        difficultyChangedEvent.NewLevel.Should().Be(SkillLevel.Advanced);
    }
    
    [Fact]
    public void Given_SameLevel_When_UpdateDifficultyLevel_Then_ShouldNotUpdateOrRaiseEvent()
    {
        // Arrange
        var skill = CreateValidSkill(difficultyLevel: SkillLevel.Intermediate);
        skill.ClearDomainEvents(); // Clear creation events
        
        // Act
        skill.UpdateDifficultyLevel(SkillLevel.Intermediate);
        
        // Assert
        skill.DifficultyLevel.Should().Be(SkillLevel.Intermediate);
        skill.DomainEvents.Should().BeEmpty();
    }
    
    #endregion
    
    #region Test Helpers
    
    private static Skill CreateValidSkill(
        string name = "C#",
        SkillCategory category = SkillCategory.ProgrammingLanguage, 
        string description = "Object-oriented programming language", 
        SkillLevel difficultyLevel = SkillLevel.Intermediate,
        bool isInDemand = false)
    {
        return new Skill(name, category, description, difficultyLevel, isInDemand);
    }
    
    #endregion
}
