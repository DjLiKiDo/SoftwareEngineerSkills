using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Entities;

namespace SoftwareEngineerSkills.Domain.UnitTests.Entities;

// Concrete implementation of Entity for testing purposes
file class TestEntity : Entity
{
    public string Name { get; private set; }

    // Private parameterless constructor
    private TestEntity() 
    {
        Name = string.Empty; // Initialize to default
    }

    public TestEntity(Guid id, string name) 
    {
        Id = id; // Manually set Id for testing equality
        Name = name;
    }
    
    public TestEntity(string name)
    {
        Name = name;
    }
}

// Another concrete implementation for type checking in Equals
file class AnotherTestEntity : Entity
{
    public string Description { get; private set; }

    private AnotherTestEntity() 
    {
        Description = string.Empty; // Initialize to default
    }

    public AnotherTestEntity(Guid id, string description)
    {
        Id = id;
        Description = description;
    }
}

public class EntityTests
{
    [Fact]
    public void Equals_SameInstance_ShouldReturnTrue()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act
        var result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act
        var result = entity.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity("Test");
        var otherObject = new object();

        // Act
        var result = entity.Equals(otherObject);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentEntityType_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid(), "Test1");
        var entity2 = new AnotherTestEntity(entity1.Id, "DifferentTypeSameId");

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SameTypeAndId_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id, "Test1");
        var entity2 = new TestEntity(id, "Test2"); // Name is different, but Id is the same

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_SameTypeDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid(), "Test");
        var entity2 = new TestEntity(Guid.NewGuid(), "Test");

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_OneEntityWithEmptyId_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.Empty, "Test1");
        var entity2 = new TestEntity(Guid.NewGuid(), "Test2");

        // Act
        var result1 = entity1.Equals(entity2);
        var result2 = entity2.Equals(entity1);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Fact]
    public void Equals_BothEntitiesWithEmptyId_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.Empty, "Test1");
        var entity2 = new TestEntity(Guid.Empty, "Test2");

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse(); 
    }

    [Fact]
    public void GetHashCode_SameIdAndType_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id, "Test1");
        var entity2 = new TestEntity(id, "Test2");

        // Act & Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentIdSameType_ShouldNotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid(), "Test");
        var entity2 = new TestEntity(Guid.NewGuid(), "Test");

        // Act & Assert
        if (entity1.Id != entity2.Id) 
        {
            entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
        }
    }
    
    [Fact]
    public void GetHashCode_DifferentTypeSameId_ShouldNotBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id, "Test1");
        var entity2 = new AnotherTestEntity(id, "DifferentTypeSameId");

        // Act & Assert
        entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_BothNull_ShouldReturnTrue()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act
        var result = (entity1 == entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_OneNull_ShouldReturnFalse()
    {
        // Arrange
        TestEntity? entity1 = new TestEntity("Test");
        TestEntity? entity2 = null;

        // Act
        var result1 = (entity1 == entity2);
        var result2 = (entity2 == entity1);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_SameInstance_ShouldReturnTrue()
    {
        // Arrange
        var entity1 = new TestEntity("Test");
        var entity2 = entity1;

        // Act
        var result = (entity1 == entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_SameIdAndType_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id, "Test1");
        var entity2 = new TestEntity(id, "Test2");

        // Act
        var result = (entity1 == entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_BothNull_ShouldReturnFalse()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act
        var result = (entity1 != entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_OneNull_ShouldReturnTrue()
    {
        // Arrange
        TestEntity? entity1 = new TestEntity("Test");
        TestEntity? entity2 = null;

        // Act
        var result1 = (entity1 != entity2);
        var result2 = (entity2 != entity1);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_SameIdAndType_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id, "Test1");
        var entity2 = new TestEntity(id, "Test2");

        // Act
        var result = (entity1 != entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void InequalityOperator_DifferentId_ShouldReturnTrue()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid(), "Test");
        var entity2 = new TestEntity(Guid.NewGuid(), "Test");

        // Act
        // Ensure IDs are different for robust test, though NewGuid should suffice
        if (entity1.Id == entity2.Id) 
        {
             entity2 = new TestEntity(Guid.NewGuid(), "Test"); // Re-assign if collision, highly unlikely
        }
        var result = (entity1 != entity2);

        // Assert
        result.Should().BeTrue();
    }
    
    // Test for BaseEntity aspects inherited by Entity
    [Fact]
    public void BaseEntity_Id_ShouldBeInitializedOnCreation()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act & Assert
        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void BaseEntity_DomainEvents_ShouldBeEmptyInitially()
    {
        // Arrange
        var entity = new TestEntity("Test");

        // Act & Assert
        entity.DomainEvents.Should().NotBeNull();
        entity.DomainEvents.Should().BeEmpty();
    }
}

// Dummy domain event for testing BaseEntity event handling (if needed for future protected methods exposure)
// For now, Entity itself doesn't expose Add/Remove/Clear for DomainEvents publicly.
// These are tested via derived classes like Skill.
file class TestDomainEvent : IDomainEvent 
{ 
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
