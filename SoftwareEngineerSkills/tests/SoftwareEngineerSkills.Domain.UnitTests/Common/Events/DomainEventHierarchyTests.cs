using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Events;

public class DomainEventHierarchyTests
{
    [Fact]
    public void DomainEvent_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();
        
        // Assert
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void EntitySoftDeletedEvent_ShouldBeADomainEvent()
    {
        // Arrange & Act
        var softDeletedEvent = new EntitySoftDeletedEvent(
            System.Guid.NewGuid(), 
            "TestEntity", 
            "TestUser");
        
        // Assert
        softDeletedEvent.Should().BeAssignableTo<DomainEvent>();
        softDeletedEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void EntityRestoredEvent_ShouldBeADomainEvent()
    {
        // Arrange & Act
        var restoredEvent = new EntityRestoredEvent(
            System.Guid.NewGuid(), 
            "TestEntity", 
            "TestUser");
        
        // Assert
        restoredEvent.Should().BeAssignableTo<DomainEvent>();
        restoredEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void IDomainEvent_Interface_ShouldHaveRequiredMembers()
    {
        // Arrange
        var type = typeof(IDomainEvent);
        var property = type.GetProperty("OccurredOn");
        
        // Act & Assert
        property.Should().NotBeNull();
        property.PropertyType.Should().Be(typeof(DateTime));
    }

    [Fact]
    public void DomainEvent_Base_ShouldHaveRequiredMembers()
    {
        // Arrange
        var type = typeof(DomainEvent);
        var occurredOnProperty = type.GetProperty("OccurredOn");
        var idProperty = type.GetProperty("Id");
        
        // Act & Assert
        occurredOnProperty.Should().NotBeNull();
        occurredOnProperty.PropertyType.Should().Be(typeof(DateTime));
        
        idProperty.Should().NotBeNull();
        idProperty.PropertyType.Should().Be(typeof(Guid));
    }

    private class TestDomainEvent : DomainEvent
    {
    }
}
