using SoftwareEngineerSkills.Domain.Common.Base;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Base;

public class SoftDeleteEntityTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var entity = new TestSoftDeleteEntity();

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
        entity.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void MarkAsDeleted_WhenNotDeleted_ShouldSetDeletionProperties()
    {
        // Arrange
        var entity = new TestSoftDeleteEntity();
        var deletedBy = "TestUser";

        // Act
        entity.MarkAsDeleted(deletedBy);

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        entity.DeletedBy.Should().Be(deletedBy);
    }

    [Fact]
    public void MarkAsDeleted_WhenAlreadyDeleted_ShouldNotChangeProperties()
    {
        // Arrange
        var entity = new TestSoftDeleteEntity();
        var initialDeletedBy = "InitialUser";
        entity.MarkAsDeleted(initialDeletedBy);
        
        var initialDeletedAt = entity.DeletedAt;
        
        // Wait briefly to ensure time would change if updated
        System.Threading.Thread.Sleep(10);

        // Act
        entity.MarkAsDeleted("NewUser");

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().Be(initialDeletedAt);
        entity.DeletedBy.Should().Be(initialDeletedBy);
    }

    [Fact]
    public void Restore_WhenDeleted_ShouldClearDeletionProperties()
    {
        // Arrange
        var entity = new TestSoftDeleteEntity();
        entity.MarkAsDeleted("TestUser");

        // Act
        entity.Restore();

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
        entity.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void Restore_WhenNotDeleted_ShouldNotChangeProperties()
    {
        // Arrange
        var entity = new TestSoftDeleteEntity();

        // Act
        entity.Restore();

        // Assert
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
        entity.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void SoftDeleteEntity_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var entity = new TestSoftDeleteEntity();

        // Assert
        entity.Should().BeAssignableTo<BaseEntity>();
    }

    // Helper class for testing
    private class TestSoftDeleteEntity : SoftDeleteEntity
    {
    }
}
