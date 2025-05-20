using FluentAssertions;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Entities.Skills;
using SoftwareEngineerSkills.Domain.Enums;
using System;
using Xunit;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities.Skills;

public class SkillEventsTests
{
    [Fact]
    public void SkillCreatedEvent_Constructor_ShouldSetProperties()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillName = "C#";
        var skillCategory = SkillCategory.ProgrammingLanguage;
        var category = "ProgrammingLanguage";

        // Act
        var skillCreatedEvent = new SkillCreatedEvent(skillId, skillName, skillCategory, category);

        // Assert
        skillCreatedEvent.SkillId.Should().Be(skillId);
        skillCreatedEvent.SkillName.Should().Be(skillName);
        skillCreatedEvent.SkillCategory.Should().Be(skillCategory);
        skillCreatedEvent.Category.Should().Be(category);
        skillCreatedEvent.Should().BeAssignableTo<IDomainEvent>();
        skillCreatedEvent.Should().BeAssignableTo<DomainEvent>();
        skillCreatedEvent.Id.Should().NotBeEmpty();
        skillCreatedEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void SkillUpdatedEvent_Constructor_ShouldSetProperties()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var originalName = "C#";
        var newName = "C# 14";

        // Act
        var skillUpdatedEvent = new SkillUpdatedEvent(skillId, originalName, newName);

        // Assert
        skillUpdatedEvent.SkillId.Should().Be(skillId);
        skillUpdatedEvent.OldName.Should().Be(originalName);
        skillUpdatedEvent.NewName.Should().Be(newName);
        skillUpdatedEvent.Should().BeAssignableTo<IDomainEvent>();
        skillUpdatedEvent.Should().BeAssignableTo<DomainEvent>();
        skillUpdatedEvent.Id.Should().NotBeEmpty();
        skillUpdatedEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void SkillDemandChangedEvent_Constructor_ShouldSetProperties()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillName = "C#";
        var isInDemand = true;

        // Act
        var skillDemandChangedEvent = new SkillDemandChangedEvent(skillId, skillName, isInDemand);

        // Assert
        skillDemandChangedEvent.SkillId.Should().Be(skillId);
        skillDemandChangedEvent.SkillName.Should().Be(skillName);
        skillDemandChangedEvent.IsInDemand.Should().Be(isInDemand);
        skillDemandChangedEvent.Should().BeAssignableTo<IDomainEvent>();
        skillDemandChangedEvent.Should().BeAssignableTo<DomainEvent>();
        skillDemandChangedEvent.Id.Should().NotBeEmpty();
        skillDemandChangedEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
