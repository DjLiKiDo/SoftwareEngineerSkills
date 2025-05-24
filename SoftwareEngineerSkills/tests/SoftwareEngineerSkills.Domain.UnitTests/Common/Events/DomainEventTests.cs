using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Events;

public class DomainEventTests
{
    [Fact]
    public void Constructor_ShouldSetOccurredOnToCurrentTime()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();
        
        // Assert
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithCustomTime_ShouldSetSpecifiedTime()
    {
        // Arrange
        var customTime = DateTime.UtcNow.AddDays(-1);
        
        // Act
        var domainEvent = new TestDomainEvent(customTime);
        
        // Assert
        domainEvent.OccurredOn.Should().Be(customTime);
    }

    [Fact]
    public void Id_ShouldBeUniqueForEachEvent()
    {
        // Arrange & Act
        var domainEvent1 = new TestDomainEvent();
        var domainEvent2 = new TestDomainEvent();
        
        // Assert
        domainEvent1.Id.Should().NotBe(domainEvent2.Id);
    }

    // Test DomainEvent implementation
    private class TestDomainEvent : DomainEvent, IDomainEvent
    {
        public TestDomainEvent() : base() { }
        
        public TestDomainEvent(DateTime occurredOn) : base(occurredOn) { }
    }
}

public class DomainEntityTests
{
    [Fact]
    public void AddDomainEvent_ShouldAddEventToCollection()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();
        
        // Act
        entity.AddTestDomainEvent(domainEvent);
        
        // Assert
        entity.DomainEvents.Should().ContainSingle();
        entity.DomainEvents.First().Should().BeSameAs(domainEvent);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveEventFromCollection()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();
        entity.AddTestDomainEvent(domainEvent);
        
        // Act
        entity.RemoveDomainEvent(domainEvent);
        
        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var entity = new TestEntity();
        entity.AddTestDomainEvent(new TestDomainEvent());
        entity.AddTestDomainEvent(new TestDomainEvent());
        
        // Act
        entity.ClearDomainEvents();
        
        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    // Test classes for domain events
    private class TestDomainEvent : DomainEvent, IDomainEvent { }
    
    private class TestEntity : BaseEntity
    {
        public void AddTestDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }
}
