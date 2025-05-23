using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Base;

public class BaseEntityTests
{
    [Fact]
    public void Constructor_ShouldGenerateId()
    {
        // Arrange & Act
        var entity = new TestBaseEntity();

        // Assert
        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEventToCollection()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var domainEvent = new TestDomainEvent();

        // Act
        entity.AddTestDomainEvent(domainEvent);

        // Assert
        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents.First().Should().Be(domainEvent);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveEventFromCollection()
    {
        // Arrange
        var entity = new TestBaseEntity();
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
        var entity = new TestBaseEntity();
        entity.AddTestDomainEvent(new TestDomainEvent());
        entity.AddTestDomainEvent(new TestDomainEvent());

        // Act
        entity.ClearDomainEvents();

        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AuditableProperties_ShouldBeSettable()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var now = DateTime.UtcNow;
        var user = "TestUser";

        // Act
        entity.Created = now;
        entity.CreatedBy = user;
        entity.LastModified = now;
        entity.LastModifiedBy = user;

        // Assert
        entity.Created.Should().Be(now);
        entity.CreatedBy.Should().Be(user);
        entity.LastModified.Should().Be(now);
        entity.LastModifiedBy.Should().Be(user);
    }

    #region Helper Classes

    private class TestBaseEntity : BaseEntity
    {
        public void AddTestDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }

    private class TestDomainEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    #endregion
}
