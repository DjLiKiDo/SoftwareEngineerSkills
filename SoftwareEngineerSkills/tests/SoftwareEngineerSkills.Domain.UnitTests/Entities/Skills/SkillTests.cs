using FluentAssertions;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using SoftwareEngineerSkills.Domain.Exceptions;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities.Skills;

public class SkillTests
{
    // Constants for test data
    private const string ValidName = "C#";
    private const string ValidDescription = "A versatile programming language.";
    private const string EmptyName = "";
    private const string WhitespaceName = "   ";
    private const string LongName = "ThisIsAVeryLongSkillNameThatExceedsTheMaximumAllowedLengthOfOneHundredCharactersAndThereforeShouldFailValidation";
    private const string EmptyDescription = "";
    private const string WhitespaceDescription = "   ";
    private const string LongDescription = "This is a very long skill description that exceeds the maximum allowed length of one thousand characters. This description is intentionally made excessively long to test the validation logic within the Skill entity. It should trigger a BusinessRuleException because the length constraint is violated. We need to ensure that our domain model correctly enforces these kinds of business rules to maintain data integrity and prevent unexpected issues in the application. This detailed description serves no other purpose than to be exceptionally long for this specific test case. Adding more text to make this description exceed one thousand characters. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Curabitur pretium tincidunt lacus. Nulla gravida orci a odio. Nullam varius, turpis et commodo pharetra, est eros bibendum elit, nec luctus magna felis sollicitudin mauris. Integer in mauris eu nibh euismod gravida. Duis ac tellus et risus vulputate vehicula. Donec lobortis risus a elit. Etiam tempor. Ut ullamcorper, ligula eu tempor congue, eros est euismod turpis, id tincidunt sapien risus a quam.";


    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateSkill()
    {
        // Arrange
        var name = ValidName;
        var category = SkillCategory.ProgrammingLanguage;
        var description = ValidDescription;
        var difficultyLevel = SkillLevel.Intermediate;
        var isInDemand = true;

        // Act
        var skill = new Skill(name, category, description, difficultyLevel, isInDemand);

        // Assert
        skill.Should().NotBeNull();
        skill.Name.Should().Be(name);
        skill.Category.Should().Be(category);
        skill.Description.Should().Be(description);
        skill.DifficultyLevel.Should().Be(difficultyLevel);
        skill.IsInDemand.Should().Be(isInDemand);
        skill.DomainEvents.Should().ContainSingle(e => e is SkillCreatedEvent);
        var skillCreatedEvent = skill.DomainEvents.First(e => e is SkillCreatedEvent) as SkillCreatedEvent;
        skillCreatedEvent.Should().NotBeNull();
        skillCreatedEvent?.SkillId.Should().Be(skill.Id);
        skillCreatedEvent?.SkillName.Should().Be(name);
        skillCreatedEvent?.SkillCategory.Should().Be(category); // Corrected assertion
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyName)]
    [InlineData(WhitespaceName)]
    public void Constructor_WithInvalidName_ShouldThrowBusinessRuleException(string? invalidName) // Made string nullable
    {
        // Arrange
        var category = SkillCategory.ProgrammingLanguage;
        var description = ValidDescription;
        var difficultyLevel = SkillLevel.Intermediate;

        // Act
        Action act = () => new Skill(invalidName, category, description, difficultyLevel);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill name cannot be empty");
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var name = LongName;
        var category = SkillCategory.ProgrammingLanguage;
        var description = ValidDescription;
        var difficultyLevel = SkillLevel.Intermediate;

        // Act
        Action act = () => new Skill(name, category, description, difficultyLevel);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyDescription)]
    [InlineData(WhitespaceDescription)]
    public void Constructor_WithInvalidDescription_ShouldThrowBusinessRuleException(string? invalidDescription) // Made string nullable
    {
        // Arrange
        var name = ValidName;
        var category = SkillCategory.ProgrammingLanguage;
        var difficultyLevel = SkillLevel.Intermediate;

        // Act
        Action act = () => new Skill(name, category, invalidDescription, difficultyLevel);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill description cannot be empty");
    }

    [Fact]
    public void Constructor_WithDescriptionTooLong_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var name = ValidName;
        var category = SkillCategory.ProgrammingLanguage;
        var description = LongDescription;
        var difficultyLevel = SkillLevel.Intermediate;

        // Act
        Action act = () => new Skill(name, category, description, difficultyLevel);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill description cannot exceed 1000 characters");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateSkillProperties()
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);
        var updatedName = "Updated Skill Name";
        var updatedCategory = SkillCategory.Framework;
        var updatedDescription = "Updated skill description.";
        var updatedDifficultyLevel = SkillLevel.Advanced;
        var updatedIsInDemand = true;
        skill.ClearDomainEvents(); // Clear initial creation event

        // Act
        skill.Update(updatedName, updatedCategory, updatedDescription, updatedDifficultyLevel, updatedIsInDemand);

        // Assert
        skill.Name.Should().Be(updatedName);
        skill.Category.Should().Be(updatedCategory);
        skill.Description.Should().Be(updatedDescription);
        skill.DifficultyLevel.Should().Be(updatedDifficultyLevel);
        skill.IsInDemand.Should().Be(updatedIsInDemand);
        skill.DomainEvents.Should().ContainSingle(e => e is SkillUpdatedEvent);
        var skillUpdatedEvent = skill.DomainEvents.First(e => e is SkillUpdatedEvent) as SkillUpdatedEvent;
        skillUpdatedEvent.Should().NotBeNull();
        skillUpdatedEvent?.SkillId.Should().Be(skill.Id);
        skillUpdatedEvent?.NewName.Should().Be(updatedName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyName)]
    [InlineData(WhitespaceName)]
    public void Update_WithInvalidName_ShouldThrowBusinessRuleException(string? invalidName) // Made string nullable
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);

        // Act
        Action act = () => skill.Update(invalidName, SkillCategory.Tool, "New Desc", SkillLevel.Expert, true);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill name cannot be empty");
    }
    
    [Fact]
    public void Update_WithNameTooLong_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);

        // Act
        Action act = () => skill.Update(LongName, SkillCategory.Tool, "New Desc", SkillLevel.Expert, true);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(EmptyDescription)]
    [InlineData(WhitespaceDescription)]
    public void Update_WithInvalidDescription_ShouldThrowBusinessRuleException(string? invalidDescription) // Made string nullable
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);

        // Act
        Action act = () => skill.Update("New Name", SkillCategory.Tool, invalidDescription, SkillLevel.Expert, true);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill description cannot be empty");
    }

    [Fact]
    public void Update_WithDescriptionTooLong_ShouldThrowBusinessRuleException()
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);

        // Act
        Action act = () => skill.Update("New Name", SkillCategory.Tool, LongDescription, SkillLevel.Expert, true);

        // Assert
        act.Should().Throw<BusinessRuleException>().WithMessage("Skill description cannot exceed 1000 characters");
    }

    [Fact]
    public void SetDemandStatus_WhenStatusChanges_ShouldUpdateIsInDemandAndAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);
        skill.ClearDomainEvents(); // Clear initial creation event

        // Act
        skill.SetDemandStatus(true);

        // Assert
        skill.IsInDemand.Should().BeTrue();
        skill.DomainEvents.Should().ContainSingle(e => e is SkillDemandChangedEvent);
        var demandChangedEvent = skill.DomainEvents.First(e => e is SkillDemandChangedEvent) as SkillDemandChangedEvent;
        demandChangedEvent.Should().NotBeNull();
        demandChangedEvent?.SkillId.Should().Be(skill.Id);
        demandChangedEvent?.SkillName.Should().Be(skill.Name);
        demandChangedEvent?.IsInDemand.Should().BeTrue();
    }

    [Fact]
    public void SetDemandStatus_WhenStatusIsSame_ShouldNotChangeIsInDemandAndNotAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, true);
        skill.ClearDomainEvents(); // Clear initial creation event

        // Act
        skill.SetDemandStatus(true);

        // Assert
        skill.IsInDemand.Should().BeTrue();
        skill.DomainEvents.Should().BeEmpty();
    }
    
    [Fact]
    public void UpdateDifficultyLevel_WhenLevelChanges_ShouldUpdateDifficultyLevelAndAddDomainEvent()
    {
        // Arrange
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, SkillLevel.Beginner, false);
        var newLevel = SkillLevel.Advanced;
        skill.ClearDomainEvents(); // Clear initial creation event

        // Act
        skill.UpdateDifficultyLevel(newLevel);

        // Assert
        skill.DifficultyLevel.Should().Be(newLevel);
        skill.DomainEvents.Should().ContainSingle(e => e is SkillUpdatedEvent); // Assuming SkillUpdatedEvent is generic enough
        var skillUpdatedEvent = skill.DomainEvents.First(e => e is SkillUpdatedEvent) as SkillUpdatedEvent;
        skillUpdatedEvent.Should().NotBeNull();
        skillUpdatedEvent?.SkillId.Should().Be(skill.Id);
        skillUpdatedEvent?.NewName.Should().Be(skill.Name); // Or a more specific event if available
    }

    [Fact]
    public void UpdateDifficultyLevel_WhenLevelIsSame_ShouldNotChangeDifficultyLevelAndNotAddDomainEvent()
    {
        // Arrange
        var initialLevel = SkillLevel.Intermediate;
        var skill = new Skill(ValidName, SkillCategory.ProgrammingLanguage, ValidDescription, initialLevel, false);
        skill.ClearDomainEvents(); // Clear initial creation event

        // Act
        skill.UpdateDifficultyLevel(initialLevel);

        // Assert
        skill.DifficultyLevel.Should().Be(initialLevel);
        skill.DomainEvents.Should().BeEmpty();
    }
}
