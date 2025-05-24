using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Events;

public class SoftDeleteEventsTests
{
    [Fact]
    public void EntitySoftDeletedEvent_Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entityType = "TestEntity";
        var deletedBy = "TestUser";

        // Act
        var @event = new EntitySoftDeletedEvent(entityId, entityType, deletedBy);

        // Assert
        @event.EntityId.Should().Be(entityId);
        @event.EntityType.Should().Be(entityType);
        @event.DeletedBy.Should().Be(deletedBy);
        @event.Id.Should().NotBe(Guid.Empty);
        @event.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EntityRestoredEvent_Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var entityType = "TestEntity";
        var previouslyDeletedBy = "TestUser";

        // Act
        var @event = new EntityRestoredEvent(entityId, entityType, previouslyDeletedBy);

        // Assert
        @event.EntityId.Should().Be(entityId);
        @event.EntityType.Should().Be(entityType);
        @event.PreviouslyDeletedBy.Should().Be(previouslyDeletedBy);
        @event.Id.Should().NotBe(Guid.Empty);
        @event.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void EntitySoftDeletedEvent_WithEmptyDeletedBy_ShouldAcceptEmptyValue()
    {
        // Arrange & Act
        var @event = new EntitySoftDeletedEvent(Guid.NewGuid(), "TestEntity", string.Empty);

        // Assert
        @event.DeletedBy.Should().BeEmpty();
    }

    [Fact]
    public void EntityRestoredEvent_WithEmptyPreviouslyDeletedBy_ShouldAcceptEmptyValue()
    {
        // Arrange & Act
        var @event = new EntityRestoredEvent(Guid.NewGuid(), "TestEntity", string.Empty);

        // Assert
        @event.PreviouslyDeletedBy.Should().BeEmpty();
    }

    [Fact]
    public void EntitySoftDeletedEvent_Constructor_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var @event = new EntitySoftDeletedEvent(Guid.NewGuid(), "TestEntity", "TestUser");

        // Assert
        @event.Should().BeAssignableTo<DomainEvent>();
        @event.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void EntityRestoredEvent_Constructor_ShouldInheritFromDomainEvent()
    {
        // Arrange & Act
        var @event = new EntityRestoredEvent(Guid.NewGuid(), "TestEntity", "TestUser");

        // Assert
        @event.Should().BeAssignableTo<DomainEvent>();
        @event.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void EntitySoftDeletedEvent_MultipleInstances_ShouldHaveDifferentIds()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        
        // Act
        var event1 = new EntitySoftDeletedEvent(entityId, "TestEntity", "TestUser");
        var event2 = new EntitySoftDeletedEvent(entityId, "TestEntity", "TestUser");

        // Assert
        event1.Id.Should().NotBe(event2.Id);
    }

    [Fact]
    public void EntityRestoredEvent_MultipleInstances_ShouldHaveDifferentIds()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        
        // Act
        var event1 = new EntityRestoredEvent(entityId, "TestEntity", "TestUser");
        var event2 = new EntityRestoredEvent(entityId, "TestEntity", "TestUser");

        // Assert
        event1.Id.Should().NotBe(event2.Id);
    }
}
