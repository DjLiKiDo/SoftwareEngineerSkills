using FluentAssertions;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;
using System;
using System.Linq;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities.Skills;

public class SkillTests
{
    private readonly string _validName = "C#";
    private readonly string _validDescription = "Object-oriented programming language developed by Microsoft";
    private readonly SkillCategory _validCategory = SkillCategory.ProgrammingLanguage;
    private readonly SkillLevel _validLevel = SkillLevel.Advanced;
    
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);

        // Assert
        skill.Name.Should().Be(_validName);
        skill.Category.Should().Be(_validCategory);
        skill.Description.Should().Be(_validDescription);
        skill.DifficultyLevel.Should().Be(_validLevel);
        skill.IsInDemand.Should().BeFalse(); // Default value
    }
    
    [Fact]
    public void Constructor_ValidParametersWithInDemand_ShouldCreateInstance()
    {
        // Arrange & Act
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel, true);

        // Assert
        skill.Name.Should().Be(_validName);
        skill.Category.Should().Be(_validCategory);
        skill.Description.Should().Be(_validDescription);
        skill.DifficultyLevel.Should().Be(_validLevel);
        skill.IsInDemand.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_ShouldThrowBusinessRuleException(string invalidName)
    {
        // Arrange & Act
        Action action = () => new Skill(invalidName, _validCategory, _validDescription, _validLevel);

        // Assert
        action.Should().Throw<BusinessRuleException>()
            .WithMessage("Skill name cannot be empty");
    }
    
    [Fact]
    public void Constructor_NameTooLong_ShouldThrowBusinessRuleException()
    {
        // Arrange
        string nameTooLong = new string('x', 101); // 101 characters
        
        // Act
        Action action = () => new Skill(nameTooLong, _validCategory, _validDescription, _validLevel);

        // Assert
        action.Should().Throw<BusinessRuleException>()
            .WithMessage("Skill name cannot exceed 100 characters");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidDescription_ShouldThrowBusinessRuleException(string invalidDescription)
    {
        // Arrange & Act
        Action action = () => new Skill(_validName, _validCategory, invalidDescription, _validLevel);

        // Assert
        action.Should().Throw<BusinessRuleException>()
            .WithMessage("Skill description cannot be empty");
    }
    
    [Fact]
    public void Constructor_DescriptionTooLong_ShouldThrowBusinessRuleException()
    {
        // Arrange
        string descriptionTooLong = new string('x', 1001); // 1001 characters
        
        // Act
        Action action = () => new Skill(_validName, _validCategory, descriptionTooLong, _validLevel);

        // Assert
        action.Should().Throw<BusinessRuleException>()
            .WithMessage("Skill description cannot exceed 1000 characters");
    }
    
    [Fact]
    public void Constructor_ShouldAddSkillCreatedDomainEvent()
    {
        // Arrange & Act
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        
        // Assert
        skill.DomainEvents.Should().NotBeEmpty();
        var domainEvent = skill.DomainEvents.First();
        domainEvent.Should().BeOfType<SkillCreatedEvent>();
        
        var skillCreatedEvent = (SkillCreatedEvent)domainEvent;
        skillCreatedEvent.SkillId.Should().Be(skill.Id);
        skillCreatedEvent.SkillName.Should().Be(_validName);
        skillCreatedEvent.SkillCategory.Should().Be(_validCategory);
        skillCreatedEvent.Category.Should().Be(_validCategory.ToString());
    }
    
    [Fact]
    public void Update_ValidParameters_ShouldUpdateSkillProperties()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        var newName = "JavaScript";
        var newCategory = SkillCategory.ProgrammingLanguage;
        var newDescription = "High-level programming language for web development";
        var newLevel = SkillLevel.Intermediate;
        var newIsInDemand = true;
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
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
    public void Update_ShouldAddSkillUpdatedDomainEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        var newName = "JavaScript";
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.Update(newName, _validCategory, _validDescription, _validLevel, false);
        
        // Assert
        skill.DomainEvents.Should().NotBeEmpty();
        var domainEvent = skill.DomainEvents.First();
        domainEvent.Should().BeOfType<SkillUpdatedEvent>();
        
        var skillUpdatedEvent = (SkillUpdatedEvent)domainEvent;
        skillUpdatedEvent.SkillId.Should().Be(skill.Id);
        skillUpdatedEvent.SkillName.Should().Be(_validName); // Original name
        skillUpdatedEvent.NewName.Should().Be(newName);
    }
    
    [Fact]
    public void SetDemandStatus_ChangedStatus_ShouldUpdateAndAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel, false);
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.SetDemandStatus(true);
        
        // Assert
        skill.IsInDemand.Should().BeTrue();
        skill.DomainEvents.Should().NotBeEmpty();
        var domainEvent = skill.DomainEvents.First();
        domainEvent.Should().BeOfType<SkillDemandChangedEvent>();
        
        var skillDemandChangedEvent = (SkillDemandChangedEvent)domainEvent;
        skillDemandChangedEvent.SkillId.Should().Be(skill.Id);
        skillDemandChangedEvent.SkillName.Should().Be(_validName);
        skillDemandChangedEvent.IsInDemand.Should().BeTrue();
    }
    
    [Fact]
    public void SetDemandStatus_SameStatus_ShouldNotAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel, true);
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.SetDemandStatus(true);
        
        // Assert
        skill.IsInDemand.Should().BeTrue();
        skill.DomainEvents.Should().BeEmpty();
    }
    
    [Fact]
    public void UpdateDifficultyLevel_ChangedLevel_ShouldUpdateAndAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        var newLevel = SkillLevel.Beginner;
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.UpdateDifficultyLevel(newLevel);
        
        // Assert
        skill.DifficultyLevel.Should().Be(newLevel);
        skill.DomainEvents.Should().NotBeEmpty();
        var domainEvent = skill.DomainEvents.First();
        domainEvent.Should().BeOfType<SkillUpdatedEvent>();
        
        var skillUpdatedEvent = (SkillUpdatedEvent)domainEvent;
        skillUpdatedEvent.SkillId.Should().Be(skill.Id);
        skillUpdatedEvent.SkillName.Should().Be(_validName);
        skillUpdatedEvent.NewName.Should().Be(_validName);
    }
    
    [Fact]
    public void UpdateDifficultyLevel_SameLevel_ShouldNotAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.UpdateDifficultyLevel(_validLevel);
        
        // Assert
        skill.DifficultyLevel.Should().Be(_validLevel);
        skill.DomainEvents.Should().BeEmpty();
    }
}
