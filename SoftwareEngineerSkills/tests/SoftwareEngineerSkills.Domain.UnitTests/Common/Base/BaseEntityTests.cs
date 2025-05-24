using SoftwareEngineerSkills.Domain.Common.Base;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Exceptions;
using System.Threading.Tasks;

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

    [Fact]
    public void IncrementVersion_ShouldIncreaseVersionByOne()
    {
        // Arrange
        var entity = new TestBaseEntity();
        var initialVersion = entity.Version;

        // Act
        entity.IncrementVersion();

        // Assert
        entity.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void EnforceInvariants_WithNoViolations_ShouldNotThrowException()
    {
        // Arrange
        var entity = new TestBaseEntity();

        // Act & Assert
        entity.Invoking(e => e.EnforceInvariants())
              .Should().NotThrow();
    }

    [Fact]
    public void EnforceInvariants_WithViolations_ShouldThrowDomainValidationException()
    {
        // Arrange
        var entity = new TestBaseEntityWithInvariants { ShouldViolateInvariants = true };

        // Act & Assert
        entity.Invoking(e => e.EnforceInvariants())
              .Should().Throw<DomainValidationException>()
              .Which.Errors.Should().Contain("Test invariant violated");
    }

    [Fact]
    public async Task EnforceInvariantsAsync_WithNoViolations_ShouldNotThrowException()
    {
        // Arrange
        var entity = new TestBaseEntity();

        // Act & Assert
        await entity.Invoking(e => e.EnforceInvariantsAsync())
                   .Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnforceInvariantsAsync_WithViolations_ShouldThrowDomainValidationException()
    {
        // Arrange
        var entity = new TestBaseEntityWithInvariants { ShouldViolateInvariants = true };

        // Act & Assert
        await entity.Invoking(e => e.EnforceInvariantsAsync())
                  .Should().ThrowAsync<DomainValidationException>()
                  .WithMessage("*Test invariant violated*");
    }

    [Fact]
    public async Task EnforceInvariantsAsync_WithAsyncViolations_ShouldThrowDomainValidationException()
    {
        // Arrange
        var entity = new TestBaseEntityWithAsyncInvariants { ShouldViolateAsyncInvariants = true };

        // Act & Assert
        await entity.Invoking(e => e.EnforceInvariantsAsync())
                  .Should().ThrowAsync<DomainValidationException>()
                  .WithMessage("*Async invariant violated*");
    }

    [Fact]
    public void Equals_SameReference_ShouldReturnTrue()
    {
        // Arrange
        var entity = new TestBaseEntity();

        // Act
        var result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentTypes_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestBaseEntity();
        var entity2 = new DifferentTestEntity();

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SameIdDifferentTypes_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestBaseEntity();
        var entity2 = new DifferentTestEntity();

        // Use reflection to set the same ID on both entities
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity1, id);
        typeof(DifferentTestEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity2, id);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SameIdSameType_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestBaseEntity();
        var entity2 = new TestBaseEntity();

        // Use reflection to set the same ID on both entities
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity1, id);
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity2, id);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_EmptyId_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestBaseEntity();
        var entity2 = new TestBaseEntity();

        // Use reflection to set empty IDs
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity1, Guid.Empty);
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity2, Guid.Empty);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void OperatorEquals_NullInstances_ShouldReturnTrue()
    {
        // Arrange
        TestBaseEntity? entity1 = null;
        TestBaseEntity? entity2 = null;

        // Act
        var result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void OperatorEquals_OneNullInstance_ShouldReturnFalse()
    {
        // Arrange
        TestBaseEntity? entity1 = new TestBaseEntity();
        TestBaseEntity? entity2 = null;

        // Act
        var result1 = entity1 == entity2;
        var result2 = entity2 == entity1;

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Fact]
    public void OperatorNotEquals_ShouldReturnOppositeOfEquals()
    {
        // Arrange
        var entity1 = new TestBaseEntity();
        var entity2 = new TestBaseEntity();
        var entity3 = new TestBaseEntity();

        // Set the same ID for entity1 and entity2
        var id = Guid.NewGuid();
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity1, id);
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity2, id);

        // Act & Assert
        (entity1 != entity2).Should().BeFalse();
        (entity1 != entity3).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameId_ShouldReturnSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestBaseEntity();
        var entity2 = new TestBaseEntity();

        // Use reflection to set the same ID on both entities
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity1, id);
        typeof(TestBaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity2, id);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentIds_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var entity1 = new TestBaseEntity();
        var entity2 = new TestBaseEntity();

        // IDs are auto-generated and should be different

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    #region Helper Classes

    private class TestBaseEntity : BaseEntity
    {
        public void AddTestDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }

    private class DifferentTestEntity : BaseEntity 
    {
        // Different entity type for testing equality
    }
    
    private class TestBaseEntityWithInvariants : BaseEntity
    {
        public bool ShouldViolateInvariants { get; set; }
          protected override IEnumerable<string> CheckInvariants()
        {
            if (ShouldViolateInvariants)
            {
                yield return "Test invariant violated";
            }
        }
    }
    
    private class TestBaseEntityWithAsyncInvariants : BaseEntity
    {
        public bool ShouldViolateAsyncInvariants { get; set; }
        
        protected override Task<IEnumerable<string>> CheckInvariantsAsync(CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();
            
            if (ShouldViolateAsyncInvariants)
            {
                errors.Add("Async invariant violated");
            }
            
            return Task.FromResult<IEnumerable<string>>(errors);
        }
    }
    
    private class TestDomainEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    #endregion
}
