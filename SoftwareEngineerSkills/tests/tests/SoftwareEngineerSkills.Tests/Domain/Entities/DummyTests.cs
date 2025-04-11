using System;
using FluentAssertions;
using SoftwareEngineerSkills.Domain.Entities;
using Xunit;

namespace SoftwareEngineerSkills.Tests.Domain.Entities;

public class DummyTests
{
    [Fact]
    public void Create_ThrowsArgumentOutOfRangeException_WhenPriorityIsInvalid()
    {
        // Act
        Action act = () => Dummy.Create("Test", "Description", -1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*Priority must be between 0 and 5*");
    }

    [Fact]
    public void Create_SetsDefaultValues_WhenCalled()
    {
        // Act
        var dummy = Dummy.Create("Test", "Description");

        // Assert
        dummy.IsActive.Should().BeTrue();
        dummy.Priority.Should().Be(0);
        dummy.CreatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ThrowsArgumentOutOfRangeException_WhenPriorityIsInvalid()
    {
        // Arrange
        var dummy = Dummy.Create("Test", "Description");

        // Act
        Action act = () => dummy.Update("Updated", "Updated Description", 6);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*Priority must be between 0 and 5*");
    }

    [Fact]
    public void Update_UpdatesValues_WhenCalled()
    {
        // Arrange
        var dummy = Dummy.Create("Test", "Description");

        // Act
        dummy.Update("Updated", "Updated Description", 3);

        // Assert
        dummy.Name.Should().Be("Updated");
        dummy.Description.Should().Be("Updated Description");
        dummy.Priority.Should().Be(3);
        dummy.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue_WhenCalled()
    {
        // Arrange
        var dummy = Dummy.Create("Test", "Description");
        dummy.Deactivate();

        // Act
        dummy.Activate();

        // Assert
        dummy.IsActive.Should().BeTrue();
        dummy.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse_WhenCalled()
    {
        // Arrange
        var dummy = Dummy.Create("Test", "Description");

        // Act
        dummy.Deactivate();

        // Assert
        dummy.IsActive.Should().BeFalse();
        dummy.UpdatedAt.Should().NotBeNull();
    }
}