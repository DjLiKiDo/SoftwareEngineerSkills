using FluentAssertions;
using SoftwareEngineerSkills.Domain.Aggregates.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;
using System;
using System.Linq;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Aggregates.Skills;

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
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_ShouldThrowBusinessRuleException(string invalidName)
    {
        // Arrange & Act
        Action action = () => new Skill(invalidName, _validCategory, _validDescription, _validLevel);

        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill name cannot be empty");
    }
    
    [Fact]
    public void Constructor_NullName_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        Action action = () => new Skill(null!, _validCategory, _validDescription, _validLevel);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("name");
    }
    
    [Fact]
    public void Constructor_NameTooLong_ShouldThrowDomainValidationException()
    {
        // Arrange
        string nameTooLong = new string('x', 101); // 101 characters
        
        // Act
        Action action = () => new Skill(nameTooLong, _validCategory, _validDescription, _validLevel);

        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill name cannot exceed 100 characters");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidDescription_ShouldThrowBusinessRuleException(string invalidDescription)
    {
        // Arrange & Act
        Action action = () => new Skill(_validName, _validCategory, invalidDescription, _validLevel);

        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill description cannot be empty");
    }
    
    [Fact]
    public void Constructor_NullDescription_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        Action action = () => new Skill(_validName, _validCategory, null!, _validLevel);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("description");
    }
    
    [Fact]
    public void Constructor_DescriptionTooLong_ShouldThrowDomainValidationException()
    {
        // Arrange
        string descriptionTooLong = new string('x', 1001); // 1001 characters
        
        // Act
        Action action = () => new Skill(_validName, _validCategory, descriptionTooLong, _validLevel);

        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill description cannot exceed 1000 characters");
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
        skillUpdatedEvent.OldName.Should().Be(_validName); // Original name
        skillUpdatedEvent.NewName.Should().Be(newName);
    }
    
    [Fact]
    public void Update_WithCategoryChange_ShouldAddCategoryChangedEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        var newCategory = SkillCategory.Framework; // Different from _validCategory (ProgrammingLanguage)
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.Update(_validName, newCategory, _validDescription, _validLevel, false);
        
        // Assert
        skill.DomainEvents.Should().HaveCount(2); // SkillUpdatedEvent + SkillCategoryChangedEvent
        skill.DomainEvents.Should().ContainSingle(e => e is SkillCategoryChangedEvent);
        
        var categoryChangedEvent = skill.DomainEvents.OfType<SkillCategoryChangedEvent>().First();
        categoryChangedEvent.SkillId.Should().Be(skill.Id);
        categoryChangedEvent.SkillName.Should().Be(_validName);
        categoryChangedEvent.OldCategory.Should().Be(_validCategory);
        categoryChangedEvent.NewCategory.Should().Be(newCategory);
    }
    
    [Fact]
    public void Update_WithDemandStatusChange_ShouldAddDemandChangedEvent()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel, false);
        var newDemandStatus = true; // Different from initial value (false)
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.Update(_validName, _validCategory, _validDescription, _validLevel, newDemandStatus);
        
        // Assert
        skill.DomainEvents.Should().HaveCount(2); // SkillUpdatedEvent + SkillDemandChangedEvent
        skill.DomainEvents.Should().ContainSingle(e => e is SkillDemandChangedEvent);
        
        var demandChangedEvent = skill.DomainEvents.OfType<SkillDemandChangedEvent>().First();
        demandChangedEvent.SkillId.Should().Be(skill.Id);
        demandChangedEvent.SkillName.Should().Be(_validName);
        demandChangedEvent.IsInDemand.Should().BeTrue();
    }
    
    [Fact]
    public void Update_WithMultipleChanges_ShouldAddAllRelevantEvents()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel, false);
        var newName = "JavaScript";
        var newCategory = SkillCategory.Framework;
        var newDescription = "A high-level programming language for the web";
        var newLevel = SkillLevel.Expert;
        var newDemandStatus = true;
        
        // Clear domain events from construction
        skill.ClearDomainEvents();
        
        // Act
        skill.Update(newName, newCategory, newDescription, newLevel, newDemandStatus);
        
        // Assert
        skill.Name.Should().Be(newName);
        skill.Category.Should().Be(newCategory);
        skill.Description.Should().Be(newDescription);
        skill.DifficultyLevel.Should().Be(newLevel);
        skill.IsInDemand.Should().Be(newDemandStatus);
        
        // Check that all three events were raised
        skill.DomainEvents.Should().HaveCount(3);
        skill.DomainEvents.Should().ContainSingle(e => e is SkillUpdatedEvent);
        skill.DomainEvents.Should().ContainSingle(e => e is SkillCategoryChangedEvent);
        skill.DomainEvents.Should().ContainSingle(e => e is SkillDemandChangedEvent);
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
        domainEvent.Should().BeOfType<SkillDifficultyChangedEvent>();
        
        var difficultyChangedEvent = (SkillDifficultyChangedEvent)domainEvent;
        difficultyChangedEvent.SkillId.Should().Be(skill.Id);
        difficultyChangedEvent.SkillName.Should().Be(_validName);
        difficultyChangedEvent.OldLevel.Should().Be(_validLevel);
        difficultyChangedEvent.NewLevel.Should().Be(newLevel);
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
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithInvalidName_ShouldThrowDomainValidationException(string invalidName)
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        
        // Act
        Action action = () => skill.Update(invalidName, _validCategory, _validDescription, _validLevel, false);
        
        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill name cannot be empty");
    }
    
    [Fact]
    public void Update_WithNameTooLong_ShouldThrowDomainValidationException()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        string nameTooLong = new string('x', 101); // 101 characters
        
        // Act
        Action action = () => skill.Update(nameTooLong, _validCategory, _validDescription, _validLevel, false);
        
        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill name cannot exceed 100 characters");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithInvalidDescription_ShouldThrowDomainValidationException(string invalidDescription)
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        
        // Act
        Action action = () => skill.Update(_validName, _validCategory, invalidDescription, _validLevel, false);
        
        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill description cannot be empty");
    }
    
    [Fact]
    public void Update_WithDescriptionTooLong_ShouldThrowDomainValidationException()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        string descriptionTooLong = new string('x', 1001); // 1001 characters
        
        // Act
        Action action = () => skill.Update(_validName, _validCategory, descriptionTooLong, _validLevel, false);
        
        // Assert
        action.Should().Throw<DomainValidationException>()
            .WithMessage("One or more domain validation errors occurred.")
            .Which.Errors.Should().Contain("Skill description cannot exceed 1000 characters");
    }
    
    [Fact]
    public void Update_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        
        // Act
        Action action = () => skill.Update(null!, _validCategory, _validDescription, _validLevel, false);
        
        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("name");
    }
    
    [Fact]
    public void Update_WithNullDescription_ShouldThrowArgumentNullException()
    {
        // Arrange
        var skill = new Skill(_validName, _validCategory, _validDescription, _validLevel);
        
        // Act
        Action action = () => skill.Update(_validName, _validCategory, null!, _validLevel, false);
        
        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("description");
    }
}
